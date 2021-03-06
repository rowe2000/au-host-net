using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AuHost.Plugins;
using CoreMidi;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RackView : ContentView, IItemView<Rack>
    {
        private readonly StackLayoutHelper<Rack, ZoneView, Zone> helper;

        public Rack Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public static ObservableRangeCollection<MidiPort> MidiInputs => PluginGraph.Instance.MidiDeviceManager.Inputs;

        public string SelectedMidiInput
        {
            get => "";
            set { }
        }

        public RackView()
        {
            InitializeComponent();
            helper = new StackLayoutHelper<Rack, ZoneView, Zone>(ZoneStack);
            MidiInPicker.ItemsSource = PluginGraph.Instance.MidiDeviceManager.Inputs;
            PluginGraph.Instance.MidiDeviceManager.Inputs.CollectionChanged += InputsOnCollectionChanged;
        }

        private void InputsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MidiInPicker.ItemsSource = PluginGraph.Instance.MidiDeviceManager.Inputs;
        }

        private void OnAddZoneClicked(object sender, EventArgs e)
        {
            PluginGraph.Instance.AddNewZone(Item);
        }
    }

    public class MidiConnector
    {
        public MidiPort Port { get; }
        public string Name => Port.PortName;
        public MidiConnector(MidiPort port)
        {
            Port = port;
        }
    }
}