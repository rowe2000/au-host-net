using AuHost.Plugins;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Frame = AuHost.Plugins.Frame;

namespace AuHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrameView : ContentView, IItemView<Frame>
    {
        public static BindableProperty ItemProperty { get; } = BindableProperty.Create(
            nameof(Item),
            typeof(Frame), 
            typeof(FrameView),
            defaultBindingMode:BindingMode.OneWay
        );

        private readonly StackLayoutHelper<Frame, RackView, Rack> helper;

        public Frame Item
        {
            get => helper.Item;
            set
            {
                helper.Item = value; 
                OnPropertyChanged();
            }
        }

        public FrameView()
        {
            InitializeComponent();
            helper = new StackLayoutHelper<Frame, RackView, Rack>(RackStack);
            Item = PluginGraph.Instance.Frame;
        }
    }
}