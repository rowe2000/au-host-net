using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuHost.Annotations;
using AuHost.Plugins;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Frame = AuHost.Plugins.Frame;

namespace AuHost.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public ObservableRangeCollection<AudioComponent> AudioComponents { get; }

        public ObservableRangeCollection<Grouping<string, AudioComponent>> Manufactures { get; }

        public MainViewModel()
        {
            AudioComponents = PluginGraph.Instance.AudioUnitManager.ViewModelAudioComponents;
            Manufactures = new ObservableRangeCollection<Grouping<string, AudioComponent>>(
                AudioComponents
                    .GroupBy(o => o.Manufacture)
                    .Select(o => new Grouping<string, AudioComponent>(o.Key, o.ToArray())));
            
            Frame.Items.CollectionChanged += OnCollectionChanged;
        }


        public ObservableRangeCollection<Preset> Presets { get; } = new ObservableRangeCollection<Preset>();
        public Frame Frame { get; } = PluginGraph.Instance.Frame;
        public SceneBar SceneBar { get; } = new SceneBar();
        public ToolBar ToolBar { get; } = new ToolBar();

        public AudioComponent SelectedAudioComponent { get; set; }

        public ICommand AddRackTask { get; } = new Command(async () => await PluginGraph.Instance.AddNewRack());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }
    }
}