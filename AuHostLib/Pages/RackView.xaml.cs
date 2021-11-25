using System;
using System.Collections.Specialized;
using System.Linq;
using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RackView : ContentView, IItemView<Rack>
    {
        public static BindableProperty BindableProperty { get; } = BindableProperty.Create(
            nameof(Item),
            typeof(Rack),
            typeof(RackView),
            defaultBindingMode:BindingMode.OneWay
        );

        private readonly StackLayoutHelper<Rack, ZoneView, Zone> helper = new StackLayoutHelper<Rack, ZoneView, Zone>();

        public Rack Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public RackView()
        {
            InitializeComponent();
            Content = helper.StackLayout;
        }
    }
}