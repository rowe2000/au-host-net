using System;
using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrameView : ContentView, IItemView<Plugins.Frame>
    {
        private readonly StackLayoutHelper<Plugins.Frame, RackView, Rack> helper;

        public Plugins.Frame Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public FrameView()
        {
            InitializeComponent();
            helper = new StackLayoutHelper<Plugins.Frame, RackView, Rack>(RackStack);
        }

        private void OnAddRackClicked(object sender, EventArgs e)
        {
            PluginGraph.Instance.AddNewRack();
        }
    }
}