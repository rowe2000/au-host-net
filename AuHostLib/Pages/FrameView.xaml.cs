using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuHost.Plugins;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuHost.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrameView : ContentView
    {
        public static BindableProperty BindableProperty { get; } = BindableProperty.Create(
            nameof(Items), 
            typeof(Container<Rack>), 
            typeof(FrameView),
            defaultBindingMode:BindingMode.OneWay
        );

        public Container<Rack> Items { get; set; }
        public Container<RackView> ItemViews { get; set; }

        public FrameView()
        {
            InitializeComponent();
            if (Items != null) Items.CollectionChanged += ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Rack newItem in e.NewItems)
                    {
                        ItemViews.Add(new RackView { Rack = newItem });
                        
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Rack newItem in e.NewItems)
                    {
                        var viewItem = ItemViews.First(o => o.Rack.Equals(newItem)) ;
                        ItemViews.Remove(viewItem);
                        Children
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    
                    break;
                case NotifyCollectionChangedAction.Reset:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}