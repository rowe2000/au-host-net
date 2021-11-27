using System;
using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZoneView : ContentView, IItemView<Plugins.Zone>
    {
        private readonly StackLayoutHelper<Zone, StripView, Strip> helper;

        public Zone Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public ZoneView()
        {
            InitializeComponent();
            helper = new StackLayoutHelper<Zone, StripView, Strip>(StripStack);
        }

        private void OnAddStripClicked(object sender, EventArgs e)
        {
            PluginGraph.Instance.AddNewStrip(Item);
        }
    }
}