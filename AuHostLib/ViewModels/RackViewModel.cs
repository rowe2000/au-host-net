using System.Windows.Input;
using AuHost.Plugins;
using Xamarin.Forms;

namespace AuHost.ViewModels
{
    public class RackViewModel : BaseViewModel<Rack, Zone>
    {
        public ICommand AddNewRackCmd { get; } = new Command<Rack>(rack => rack.AddNewZone());
    }
}