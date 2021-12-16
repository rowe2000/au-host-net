using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuHost.Annotations;
using AuHost.Models;
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
        public ObservableRangeCollection<AudioComponentModel> AudioComponentModels { get; } 

        public ObservableRangeCollection<Grouping<string, AudioComponentModel>> Manufactures { get; }

        public MainPageViewModel()
        {
            PluginGraph = PluginGraph.Instance;
            AudioComponentModels = PluginGraph.AudioUnitManager.AudioUnitComponentModels;
            Manufactures = new ObservableRangeCollection<Grouping<string, AudioComponentModel>>(
                AudioComponentModels
                    .GroupBy(o => o.Manufacture)
                    .Select(o => new Grouping<string, AudioComponentModel>(o.Key, o.ToArray())));
        }

        public Zone SelectedZone => PluginGraph.SelectedZone;

        public SceneBar SceneBar { get; } = new SceneBar();
        public ICommand[] ToolBar { get; } = { null, null, null, null, null, null, null, null };

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
            PluginGraph.LaunchDocument();
            PluginGraph.Frame.AddNewRack();
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