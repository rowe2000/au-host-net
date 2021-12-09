using System.Windows.Input;
using AuHost.Plugins;
using CoreMidi;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace AuHost.ViewModels
{
    public class RackViewModel : BaseViewModel<Rack, Zone>
    {
        public ICommand AddNewRackCmd { get; } = new Command<Rack>(rack => rack.AddNewZone());

        public ObservableRangeCollection<MidiPort> MidiInputs => PluginGraph.Instance.MidiDeviceManager.Inputs;
    }
}