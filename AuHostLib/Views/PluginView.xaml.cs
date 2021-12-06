using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PluginView : ContentView, IItemView<Plugins.Plugin>
    {
        public static BindableProperty BindableProperty { get; } = BindableProperty.Create(
            nameof(Item),
            typeof(Container<Zone>), 
            typeof(PluginView),
            defaultBindingMode:BindingMode.OneWay
        );
        
        public Plugin Item { get; set; }

        public PluginView()
        {
            InitializeComponent();
        }        
    }
}