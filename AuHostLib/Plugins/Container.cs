using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AuHost.Annotations;
using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.Plugins
{
    public class Container<TItem> : INotifyPropertyChanged, IContainer, IList<TItem> where TItem : class, IItem
    {
        public ObservableRangeCollection<TItem> Items { get; } = new ObservableRangeCollection<TItem>();

        public event Action<ChangedEvent<TItem>> Changed;

        public Container()
        {
            Items.CollectionChanged += (_, e) =>
            {
                OnCollectionChanged(e);
                OnChanged(new ChangedEvent<TItem>(e.Action, e.NewItems.OfType<TItem>().ToArray(), null));
            };
        }

        public T GetItemDeep<T>(int id) where T : class
        {
            foreach (var item in Items)
            {
                if (item.Id == id)
                    return item as T;

                if (!(item is IContainer container)) 
                    continue;
                
                var foundItem = container.GetItemDeep<T>(id);
                if (foundItem != null)
                    return foundItem;
            }

            return null;
        }

        public bool HasItems() => Items.Any();

        public List<T> GetPath<T>() where T : class, IContainer
        {
            var item = this as T;
            var path = new List<T>();

            while (item != null)
            {
                path.Insert(0, item);
                item = (item as IItem)?.Parent as T;
            }

            return path;
        }

        public int DeepCount<T>() => Items.Count + Items.Sum(item => (item as IContainer)?.DeepCount<T>()) ?? 0;

        public void Add(TItem item) => AddRange(item.ToMany());
        public void AddRange(IEnumerable<TItem> items)
        {
            var index = Items.Count;
            var array = items.Where(o => o != null).ToArray();
            foreach (var item in array)
            {
                item.Parent = this;
                item.Index = index;
            }
            
            Items.AddRange(array);
            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Add, array, index));
        }

        public void Clear()
        {
            foreach (var item in Items)
            {
                item.Parent = null;
                item.Index = -1;
            }

            Items.Clear();
            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Reset, null, null));
        }

        public bool Contains(TItem item) => Items.Contains(item);

        public void CopyTo(TItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public bool Remove(TItem item)
        {
            if (item == null) 
                return false;
            
            item.Parent = null;
            item.Index = -1;
            
            var removed = Items.Remove(item);
            if (removed)
                OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Remove, item.ToMany(), null));
            
            return removed;
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;
        public int IndexOf(TItem item) => Items.IndexOf(item);

        public void Insert(int index, TItem item) => InsertRange(index, item.ToMany());
        public void InsertRange(int index, IEnumerable<TItem> items)
        {
            var array = items.Where(o => o != null).ToArray();
            var i = index;
            
            foreach (var item in array)
            {
                item.Parent = this;
                item.Index = index;
                Items.Insert(i++, item);
            }

            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Add, array, index));
        }

        public void RemoveAt(int index)
        {
            if (index < 0 && index >= Items.Count)
                throw new ArgumentOutOfRangeException();
            
            var item = Items[index];
            item.Parent = null;
            item.Index = -1;
            
            Items.RemoveAt(index);
            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Remove, item.ToMany(), index));
        }

        public void RemoveRange(IEnumerable<TItem> items)
        {
            var array = items.Where(o => o != null).ToArray();
            
            foreach (var item in array)
            {
                item.Parent = null;
                item.Index = -1;
            }
            
            Items.RemoveRange(array);

            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Remove, array, null));
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0 && count < 0 && index + count >= Items.Count)
                throw new ArgumentOutOfRangeException();
            
            var items = Items.Skip(index).Take(count).ToArray();
            foreach (var item in items)
            {
                item.Parent = null;
                item.Index = -1;
            }
            RemoveRange(items);
        }

        public TItem this[int index]
        {
            get => Items[index];
            set
            {
                var old = Items[index];
                old.Index = -1;
                old.Parent = null;
                
                value.Index = index;
                value.Parent = this;
                Items[index] = value;

                OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Replace, value));
            }
        }

        public void Move(int index, int newIndex)
        {
            var item = Items[index];

            Items.RemoveAt(index);
            Items.Insert(newIndex, item);
            item.Index = newIndex;

            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Move, item.ToMany(), index));
        }

        public virtual void OnChanged(ChangedEvent<TItem> obj) => Changed?.Invoke(obj);

        public void FixIndices()
        {
            for (var i = 0; i < Items.Count; i++) 
                Items[i].Index = i;
        }

        public void Move(int fromIndex, IContainer newOwner)
        {
            // Move items with index >= fromIndex
            var items = Items.Skip(fromIndex).ToArray();
            RemoveRange(fromIndex, items.Length);
            newOwner.AddItems(items);
        }
        
        public ICacheable GetLastItem() => Items.LastOrDefault() as ICacheable;
        public ICacheable GetFirstItem() => Items.FirstOrDefault() as ICacheable;
        public T GetItem<T>(int index) where T : class => index > -1 && index < Items.Count ? Items[index] as T : null;
        public bool Remove(IItem item) => Items.Remove(item as TItem);
        public void Add(IItem item) => Items.Add(item as TItem);
        public void Insert(IItem item, int index) => Items.Insert(index, item as TItem);

        public void Move(IItem item, int newIndex)
        {
            if (Items != null
                && newIndex != item.Index
                && newIndex > -1
                && newIndex < Items.Count)
            {
                Move(item.Index, newIndex);
            }
        }

        public void AddItems(IEnumerable items) => Items.AddRange(items.OfType<TItem>());
        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}