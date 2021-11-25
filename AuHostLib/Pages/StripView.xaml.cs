using System;
using System.Collections.Specialized;
using System.Linq;
using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StripView : ContentView, IItemView<Plugins.Strip>
    {
        public static BindableProperty BindableProperty { get; } = BindableProperty.Create(
            nameof(Item),
            typeof(Strip), 
            typeof(StripView),
            defaultBindingMode:BindingMode.OneWay
        );
        
        private readonly StackLayoutHelper<Strip, PluginView, Plugin> helper = new StackLayoutHelper<Strip, PluginView, Plugin>();

        public Strip Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public StripView()
        {
            InitializeComponent();
            Content = helper.StackLayout;
        }    
    }
}