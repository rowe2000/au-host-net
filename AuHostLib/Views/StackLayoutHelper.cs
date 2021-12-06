using System;
using System.Collections.Specialized;
using System.Linq;
using AuHost.Plugins;
using Xamarin.Forms;

namespace AuHost.Views
{
    public class StackLayoutHelper<TItem, TItemView, TSubItem> where TItem : INotifyCollectionChanged 
        where TItemView : View, IItemView<TSubItem>, new()
    {
        public StackLayout StackLayout { get; }

        private TItem item;

        public StackLayoutHelper(StackLayout stackLayout)
        {
            StackLayout = stackLayout;
        }

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

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IItem newItem in e.NewItems)
                    {
                        StackLayout.Children.Insert(newItem.Index, new TItemView { Item = (TSubItem)newItem });
                    }

                    break;
                
                case NotifyCollectionChangedAction.Move:
                    foreach (IItem newItem in e.NewItems)
                    {
                        var view = StackLayout.Children.First(o => Equals(((IItemView<TSubItem>)o).Item, newItem));
                        StackLayout.Children.Remove(view);
                        StackLayout.Children.Insert(newItem.Index, view);
                    }
                    
                    break;
                
                case NotifyCollectionChangedAction.Remove:
                    foreach (IItem newItem in e.NewItems)
                    {
                        var view = StackLayout.Children.First(o => Equals(((IItemView<TSubItem>)o).Item, newItem));
                        StackLayout.Children.Remove(view);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (IItem newItem in e.NewItems)
                    {
                        var view = StackLayout.Children.First(o => Equals(((IItemView<TSubItem>)o).Item, newItem));
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
