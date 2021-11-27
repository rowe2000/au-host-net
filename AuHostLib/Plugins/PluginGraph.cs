using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuHost.Commands;
using AVFoundation;
using Foundation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.Plugins
{
    public sealed class PluginGraph
    {
        public static PluginGraph Instance { get; } = new PluginGraph();

        public event Action Changed;

        private static readonly ConcurrentDictionary<AVAudioUnit, Plugin> plugins = new ConcurrentDictionary<AVAudioUnit, Plugin>();
        public AVAudioEngine Engine { get; }
        public AudioUnitManager AudioUnitManager { get; } = new AudioUnitManager();

        public Frame Frame { get; } = new Frame();
        public Document Document { get; set; } = new Document();
        public Rack SelectedRack { get; private set; }
        public Zone SelectedZone { get; private set; }
        public Strip SelectedStrip { get; private set; }
        public Plugin SelectedPlugin { get; set; }

        public void SelectRack(Rack rack) => SelectedRack = rack;
        public void SelectZone(Zone zone) => SelectedZone = zone;
        public void SelectStrip(Strip strip) => SelectedStrip = strip;
        public void SelectPlugin(Plugin plugin) => SelectedPlugin = plugin;

        public List<Command> NonSceneCommands { get; } = new List<Command>();

        public CommandStack Commands { get; } = new CommandStack();
        public ObservableRangeCollection<string> MidiInputs { get; set; }

        public PluginGraph()
        {
            Engine = new AVAudioEngine();
            Engine.Reset();
            Document.CurrentScene = Document;
        }
        public void Execute(Command command)
        {
            if (command == null) 
                return;
            
            Commands.Execute(command);

            if (command.SaveInScene)
                Document.CurrentScene.AddCommand(command);
            else
                NonSceneCommands.Add(command);

            OnChanged();
        }

        public void Undo()
        {
            var command = Commands.Undo();
            if (command == null) 
                return;
            
            if (command.SaveInScene)
                Document.CurrentScene.RemoveCommand(command);
            else
                NonSceneCommands.Remove(command);

            OnChanged();
        }

        public void OnChanged()
        {
            Changed?.Invoke();
        }

        public bool LoadDocument(string fileName)
        {
            var data = File.ReadAllText(fileName);
            if (NSJsonSerialization.Deserialize(data, NSJsonReadingOptions.FragmentsAllowed, out var error) is Document doc)
            {
                LaunchDocument(doc);
                return true;
            }
            
            NewDocument();
            return false;
        }

        private void LaunchDocument(Document doc)
        {
            Document.Launch(null);
            Document.Dispose();
            
            Frame.Clear();

            Document = doc;
            Document.Launch(Document.GetInitialScene());
        }

        public void NewDocument()
        {
            LaunchDocument(new Document { Name = "New document" });
            AddNewRack(false);
        }

        public void AddNewRack(bool beforeSelectedRack = false)
        {
            var currentScene = Document.CurrentScene;
            Document.Launch(Document);

            var rackIndex = SelectedRack?.Index + (beforeSelectedRack ? 0 : 1) ?? Frame.Count;
            var addRack = new AddRack(rackIndex);
            Execute(addRack);

            Document.Launch(currentScene);

            AddNewZone(addRack.NewRack);
        }

        public void AddNewZone(Rack rack = null, bool beforeSelectedZone = false)
        {
            rack = rack ?? SelectedRack;
            var zoneIndex = SelectedZone?.Index + (beforeSelectedZone ? 0 : 1) ?? rack.Count;
            var addZone = new AddZone(rack, zoneIndex);
            Execute(addZone);

            AddNewStrip(addZone.NewZone);
        }

        public void AddNewStrip(Zone zone = null, bool beforeSelectedStrip = false)
        {
            int stripIndex;
            if (zone == null)
            {
                zone = SelectedZone;
                stripIndex = SelectedStrip?.Index + (beforeSelectedStrip ? 0 : 1) ?? SelectedZone.Count;
            }
            else
            {
                stripIndex = zone.Count;
            }

            Execute(new AddStrip(zone, stripIndex, StripType.Instrument));
        }

        public bool SaveDocument(string fileName)
        {
            var data = NSJsonSerialization.Serialize(Document, NSJsonWritingOptions.PrettyPrinted, out var error);
            return data.Save(fileName, NSDataWritingOptions.Atomic, out error);
        }
        
        public void RemoveConnection(Plugin srcPlugin, int srcChannel, Plugin dstPlugin, int dstChannel)
        {
            if (srcChannel != 4096 && dstChannel != 4096)
            {
                Engine?.DisconnectNodeOutput(srcPlugin.AVAudioUnit);
                Engine?.DisconnectNodeInput(dstPlugin.AVAudioUnit);
            }
            else if (srcChannel == 4096 && dstChannel == 4096)
                Engine?.DisconnectMidi(srcPlugin.AVAudioUnit, dstPlugin.AVAudioUnit);
        }

        public void AddConnection(Plugin srcPlugin, int srcChannel, Plugin dstPlugin, int dstChannel)
        {
            if (srcChannel != 4096 && dstChannel != 4096)
                Engine?.Connect(srcPlugin.AVAudioUnit, dstPlugin.AVAudioUnit, null);
            else if (srcChannel == 4096 && dstChannel == 4096)
                Engine?.ConnectMidi(srcPlugin.AVAudioUnit, dstPlugin.AVAudioUnit, null, null);
        }

        public static Plugin GetPluginByAvAudioUnit(AVAudioUnit avAudioUnit)
        {
            return plugins.TryGetValue(avAudioUnit, out var plugin) ? plugin : null;
        }

        public static void Register(Plugin plugin)
        {
            plugins[plugin.AVAudioUnit] = plugin;
        }

        public async Task<AVAudioUnit> Fetch(string name, AVAudioUnit avAudioUnit)
        {
            avAudioUnit = await AudioUnitManager.Fetch(name, avAudioUnit);
            if (avAudioUnit == null)
                return null;
            
            Engine.AttachNode(avAudioUnit);
            Engine.Connect(avAudioUnit, Engine.MainMixerNode, null);
            if (Engine.Running) 
                return avAudioUnit;
            
            Engine.Prepare();
            Engine.StartAndReturnError(out var error);
            Console.WriteLine(error);

            return avAudioUnit;
                    
        }
        
        private AVAudioUnit midiInDev;

        public MidiDeviceManager MidiDeviceManager { get; } = new MidiDeviceManager();

        public async void Load()
        {
            // AUAudioUnit.RegisterSubclass(
            //     new Class(typeof(AuMidiConnector)),
            //     AuMidiConnector.componentDescription,
            //     "Midi in connector",
            //     int.MaxValue
            // );

            
            var plugins = new[] {"Pianoteq 7", "M1", "Blue3"};

            foreach (var task in plugins.Select(async name => await PluginGraph.Instance.Fetch(name, null)))
            {
                var av = await task;
                av.AUAudioUnit.RequestViewController(view =>
                {
                    // View.Frame = view.View.Frame;
                    // View.AddSubview(view.View);
                });
                
                MidiDeviceManager.MessagesReceived += m =>
                {
                    foreach (var midiMessage in m)
                    {
                        av.AudioUnit.MusicDeviceMIDIEvent(midiMessage.StatusByte, midiMessage.Byte1, midiMessage.Byte2 ?? 0);
                        Console.WriteLine($@"{av.Name}, {midiMessage}");
                    }
                };
            }
        }
        
        private void MidiMessagesReceived(MidiMessage[] m, AVAudioUnit av)
        {
            foreach (var midiMessage in m)
                av.AudioUnit.MusicDeviceMIDIEvent(midiMessage.StatusByte, midiMessage.Byte1, midiMessage.Byte2 ?? 0);
        }

        private int Tap(long eventsampletime, byte cable, nint length, IntPtr midiBytes, AVAudioUnit av)
        {
            if (length > 0)
            {
                var midi = new MidiMessage(length, midiBytes);
                av.AudioUnit.MusicDeviceMIDIEvent(midi.StatusByte, midi.Byte1, midi.Byte2.Value);
                Console.Write($@": TAP on {cable}, ");
                Console.WriteLine(midi.ToString());
            }

            return 0;
        }

        public void AddPlugin()
        {
        }
    }
}