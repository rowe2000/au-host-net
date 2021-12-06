using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;

namespace AuHost.Plugins
{
    public sealed class PluginGraph
    {
        public static PluginGraph Instance { get; } = new PluginGraph();

        private readonly ConcurrentDictionary<AVAudioUnit, Plugin> plugins = new ConcurrentDictionary<AVAudioUnit, Plugin>();
        public AVAudioEngine AudioEngine { get; } = new AVAudioEngine();
        public AudioUnitManager AudioUnitManager { get; } = new AudioUnitManager();
        public MidiDeviceManager MidiDeviceManager { get; } = new MidiDeviceManager();
        public CommandExecutor CommandExecutor { get; } = new CommandExecutor();

        public Frame Frame { get; } = new Frame();
        public Document Document { get; set; }
        public Rack SelectedRack { get; private set; }
        public Zone SelectedZone { get; private set; }
        public Strip SelectedStrip { get; private set; }
        public Plugin SelectedPlugin { get; private set; }

        public void SelectRack(Rack rack)
        {
            SelectedRack = rack;
            SelectedZone = null;
            SelectedStrip = null;
            SelectedPlugin = null;
        }

        public void SelectZone(Zone zone)
        {
            SelectedRack = zone?.Parent;
            SelectedZone = zone;
            SelectedStrip = null;
            SelectedPlugin = null;
        }

        public void SelectStrip(Strip strip)
        {
            SelectZone(strip?.Parent);
            SelectedStrip = strip;
            SelectedPlugin = null;
        }

        public void SelectPlugin(Plugin plugin)
        {
            SelectStrip(plugin?.Parent);
            SelectedPlugin = plugin;
        }

        public PluginGraph()
        {
            AudioEngine.Reset();
            Document = new Document();
            Document.CurrentScene = Document;
            CommandExecutor.SetDocument(Document);
        }

        public void RemoveConnection(Plugin srcPlugin, int srcChannel, Plugin dstPlugin, int dstChannel)
        {
            if (srcChannel != 4096 && dstChannel != 4096)
            {
                AudioEngine?.DisconnectNodeOutput(srcPlugin.AVAudioUnit);
                AudioEngine?.DisconnectNodeInput(dstPlugin.AVAudioUnit);
            }
            else if (srcChannel == 4096 && dstChannel == 4096)
                AudioEngine?.DisconnectMidi(srcPlugin.AVAudioUnit, dstPlugin.AVAudioUnit);
        }

        public void AddConnection(Plugin srcPlugin, int srcChannel, Plugin dstPlugin, int dstChannel)
        {
            if (srcChannel != 4096 && dstChannel != 4096)
                AudioEngine?.Connect(srcPlugin.AVAudioUnit, dstPlugin.AVAudioUnit, null);
            else if (srcChannel == 4096 && dstChannel == 4096)
                AudioEngine?.ConnectMidi(srcPlugin.AVAudioUnit, dstPlugin.AVAudioUnit, null, null);
            else
                throw new Exception("Can't connect Midi to Audio");
        }

        public Plugin GetPluginByAvAudioUnit(AVAudioUnit avAudioUnit)
        {
            return plugins.TryGetValue(avAudioUnit, out var plugin) ? plugin : null;
        }

        public void Register(Plugin plugin)
        {
            plugins[plugin.AVAudioUnit] = plugin;
        }

        public async Task<AVAudioUnit> Fetch(string name, AVAudioUnit avAudioUnit)
        {
            avAudioUnit = await AudioUnitManager.Fetch(name, avAudioUnit);
            if (avAudioUnit == null)
                return null;
            
            AudioEngine.AttachNode(avAudioUnit);
            AudioEngine.Connect(avAudioUnit, AudioEngine.MainMixerNode, null);
            if (AudioEngine.Running) 
                return avAudioUnit;
            
            AudioEngine.Prepare();
            AudioEngine.StartAndReturnError(out var error);
            Console.WriteLine(error);

            return avAudioUnit;
                    
        }
        
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

        public void LaunchDocument(Document doc)
        {
            Document.Launch(null);
            Document.Dispose();
            
            Frame.Clear();

            Document = doc;
            Document.Launch(Document.GetInitialScene());
            CommandExecutor.SetDocument(doc);
        }

    }
}