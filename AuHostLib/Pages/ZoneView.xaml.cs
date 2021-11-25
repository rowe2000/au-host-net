using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZoneView : ContentView, IItemView<Plugins.Zone>
    {
        public static BindableProperty BindableProperty { get; } = BindableProperty.Create(
            nameof(Item),
            typeof(Zone), 
            typeof(ZoneView),
            defaultBindingMode:BindingMode.OneWay
        );
        
        private readonly StackLayoutHelper<Zone, StripView> helper = new StackLayoutHelper<Zone, StripView>();

        public Zone Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public ZoneView()
        {
            InitializeComponent();
            Content = helper.StackLayout;
        }    
    }
}