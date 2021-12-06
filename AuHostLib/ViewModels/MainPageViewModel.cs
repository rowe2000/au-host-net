using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuHost.Annotations;
using AuHost.Commands;
using AuHost.Plugins;
using CoreMidi;
using Foundation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace AuHost.ViewModels
{
    public sealed class MainPageViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public PluginGraph PluginGraph { get; }
        public ObservableRangeCollection<AudioComponent> AudioComponents { get; } 

        public ObservableRangeCollection<Grouping<string, AudioComponent>> Manufactures { get; }

        public MainPageViewModel()
        {
            PluginGraph = PluginGraph.Instance;
            AudioComponents = PluginGraph.AudioUnitManager.ViewModelAudioComponents;
            Manufactures = new ObservableRangeCollection<Grouping<string, AudioComponent>>(
                AudioComponents
                    .GroupBy(o => o.Manufacture)
                    .Select(o => new Grouping<string, AudioComponent>(o.Key, o.ToArray())));
            
            AddNewRackCmd = new Xamarin.Forms.Command(AddNewRack);
            AddNewZoneCmd = new Command<Rack>(AddNewZone);
            AddNewStripCmd = new Command<Zone>(AddNewStrip);
        }


        public ObservableRangeCollection<Preset> Presets { get; } = new ObservableRangeCollection<Preset>();

        public Rack SelectedRack => PluginGraph.SelectedRack;
        public Zone SelectedZone => PluginGraph.SelectedZone;
        public Strip SelectedStrip => PluginGraph.SelectedStrip;



        public SceneBar SceneBar { get; } = new SceneBar();
        public ToolBar ToolBar { get; } = new ToolBar();
        public AudioComponent SelectedAudioComponent { get; set; }

        public ICommand AddNewRackCmd { get; }
        public Command<Rack> AddNewZoneCmd { get; }
        
        public ICommand AddNewStripCmd { get; }

        public ObservableRangeCollection<MidiPort> MidiInputs { get; } = new ObservableRangeCollection<MidiPort>();

        public bool SaveDocument(string fileName)
        {
            var data = NSJsonSerialization.Serialize(PluginGraph.Document, NSJsonWritingOptions.PrettyPrinted, out var error);
            return data.Save(fileName, NSDataWritingOptions.Atomic, out error);
        }
        
        public bool LoadDocument(string fileName)
        {
            var data = File.ReadAllText(fileName);
            if (NSJsonSerialization.Deserialize(data, NSJsonReadingOptions.FragmentsAllowed, out var error) is Document doc)
            {
                PluginGraph.LaunchDocument(doc);
                return true;
            }
            
            NewDocument();
            return false;
        }

        public void NewDocument()
        {
            PluginGraph.LaunchDocument(new Document { Name = "New document" });
            AddNewRack();
        }

        private void AddNewRack()
        {
            var currentScene = PluginGraph.Document.CurrentScene;
            PluginGraph.Document.Launch(PluginGraph.Document);

            var rackIndex = SelectedRack?.Index ?? PluginGraph.Frame.Count;
            var addRack = new AddRack(rackIndex);
            PluginGraph.CommandExecutor.Execute(addRack);

            PluginGraph.Document.Launch(currentScene);

            AddNewZone(addRack.NewRack);
        }

        private void AddNewZone(Rack rack)
        {
            rack = rack ?? SelectedRack;
            var zoneIndex = SelectedZone?.Index + (false ? 0 : 1) ?? rack.Count;
            var addZone = new AddZone(rack, zoneIndex);
            PluginGraph.CommandExecutor.Execute(addZone);

            AddNewStrip(addZone.NewZone);
        }

        private void AddNewStrip(Zone zone)
        {
            var stripIndex = zone.Count;
            PluginGraph.CommandExecutor.Execute(new AddStrip(zone, stripIndex, StripType.Instrument));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }
    }
}