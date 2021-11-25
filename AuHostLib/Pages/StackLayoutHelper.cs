using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using AuHost.Plugins;
using Xamarin.Forms;

namespace AuHost.Pages
{
    public class StackLayoutHelper<TItem, TItemView> where TItem : INotifyCollectionChanged
    {
        private static readonly ConstructorInfo ViewConstructorInfo = typeof(TItemView)
            .GetConstructors()
            .First(o => o.GetParameters()
                .SingleOrDefault()?.ParameterType == typeof(TItem));

        public StackLayout StackLayout { get; } = new StackLayout();

        private TItem item;
        public TItem Item
        {
            get => item;
            set
            {
                if (item != null)
                    item.CollectionChanged -= ItemsCollectionChanged;
                item = value;
                if (item != null)
                    item.CollectionChanged += ItemsCollectionChanged;
            }
        }
        
        private View CreateView(IItem newItem)
        {
            return (View)ViewConstructorInfo?.Invoke(this, new object[] { newItem });
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IItem newItem in e.NewItems)
                    {
                        StackLayout.Children.Insert(newItem.Index, CreateView(newItem));
                    }

                    break;
                
                case NotifyCollectionChangedAction.Move:
                    foreach (IItem newItem in e.NewItems)
                    {
                        var view = StackLayout.Children.First(o => Equals(((IItemView<Rack>)o).Item, newItem));
                        StackLayout.Children.Remove(view);
                        StackLayout.Children.Insert(newItem.Index, view);
                    }
                    
                    break;
                
                case NotifyCollectionChangedAction.Remove:
                    foreach (IItem newItem in e.NewItems)
                    {
                        var view = StackLayout.Children.First(o => Equals(((IItemView<Rack>)o).Item, newItem));
                        StackLayout.Children.Remove(view);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (IItem newItem in e.NewItems)
                    {
                        var view = StackLayout.Children.First(o => Equals(((IItemView<Rack>)o).Item, newItem));
                        StackLayout.Children.Remove(view);
                        StackLayout.Children.Insert(newItem.Index, view);
                    }

                    break;
                
                case NotifyCollectionChangedAction.Reset:
                    StackLayout.Children.Clear();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}