using System;
using System.Collections.Specialized;
using System.Linq;
using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Frame = AuHost.Plugins.Frame;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrameView : ContentView, IItemView<Plugins.Frame>
    {
        public static BindableProperty BindableProperty { get; } = BindableProperty.Create(
            nameof(Item),
            typeof(Frame), 
            typeof(FrameView),
            defaultBindingMode:BindingMode.OneWay
        );

        private readonly StackLayoutHelper<Frame, RackView> helper = new StackLayoutHelper<Frame, RackView>();

        public Frame Item
        {
            get => helper.Item;
            set => helper.Item = value;
        }

        public FrameView()
        {
            InitializeComponent();
            Content = helper.StackLayout;
        }
    }
}