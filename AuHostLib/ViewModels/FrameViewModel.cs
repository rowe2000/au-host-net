using System.Windows.Input;
using AuHost.Models;
using AuHost.Plugins;
using Xamarin.Forms;
using Frame = AuHost.Models.Frame;

namespace AuHost.ViewModels
{
    public class FrameViewModel : BaseViewModel<Frame, Rack>
    {
        public FrameViewModel()
        {
            Item = PluginGraph.Instance.Frame;
        }

        public ICommand AddNewRackCmd { get; } = new Command<Frame>(frame => frame.AddNewRack());
    }
}