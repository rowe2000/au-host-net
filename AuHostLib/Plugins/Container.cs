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
    public class Container<TItem> : IContainer, IList<TItem> where TItem : class, IItem
    {
        private readonly ObservableRangeCollection<TItem> items = new ObservableRangeCollection<TItem>();

        public IEnumerable<TItem> Items => items.ToArray();
        public event Action<ChangedEvent<TItem>> Changed;

        public Container()
        {
            items.CollectionChanged += (_, e) =>
            {
                OnCollectionChanged(e);
                OnChanged(new ChangedEvent<TItem>(e.Action, e.NewItems.OfType<TItem>().ToArray()));
            };
        }

        public T GetItemDeep<T>(int id) where T : class
        {
            foreach (var item in items)
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

        public bool HasItems() => items.Any();

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

        public int DeepCount<T>() => items.Count + items.Sum(item => (item as IContainer)?.DeepCount<T>()) ?? 0;

        public void Add(TItem item) => AddRange(item.ToMany());
        public void AddRange(IEnumerable<TItem> addItems)
        {
            var index = items.Count;
            var array = addItems.Where(o => o != null).ToArray();
            foreach (var item in array)
            {
                item.Parent = this;
                item.Index = index;
            }
            
            items.AddRange(array);
            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Add, array, index));
        }

        public void Clear()
        {
            foreach (var item in items)
            {
                item.Parent = null;
                item.Index = -1;
            }

            items.Clear();
            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(TItem item) => items.Contains(item);

        public void CopyTo(TItem[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public bool Remove(TItem item)
        {
            if (item == null) 
                return false;
            
            item.Parent = null;
            item.Index = -1;
            
            var removed = items.Remove(item);
            if (removed)
                OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Remove, item));
            
            return removed;
        }

        public int Count => items.Count;
        public bool IsReadOnly => false;
        public int IndexOf(TItem item) => items.IndexOf(item);

        public void Insert(int index, TItem item) => InsertRange(index, item.ToMany());
        public void InsertRange(int index, IEnumerable<TItem> insertItems)
        {
            var array = insertItems.Where(o => o != null).ToArray();
            var i = index;
            
            foreach (var item in array)
            {
                item.Parent = this;
                item.Index = index;
                items.Insert(i++, item);
            }

            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Add, array, index));
        }

        public void RemoveAt(int index)
        {
            if (index < 0 && index >= items.Count)
                throw new ArgumentOutOfRangeException();
            
            var item = items[index];
            item.Parent = null;
            item.Index = -1;
            
            items.RemoveAt(index);
            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Remove, item, index));
        }

        public bool RemoveRange(IEnumerable<TItem> removeItems)
        {
            var array = removeItems.Where(o => o != null).ToArray();
            
            foreach (var item in array)
            {
                item.Parent = null;
                item.Index = -1;
            }
            
            items.RemoveRange(array);

            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Remove, array));
            return true;
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0 && count < 0 && index + count >= items.Count)
                throw new ArgumentOutOfRangeException();
            
            var array = items.Skip(index).Take(count).ToArray();
            foreach (var item in array)
            {
                item.Parent = null;
                item.Index = -1;
            }
            
            RemoveRange(array);
        }

        public TItem this[int index]
        {
            get => items[index];
            set
            {
                var old = items[index];
                old.Index = -1;
                old.Parent = null;
                
                value.Index = index;
                value.Parent = this;
                items[index] = value;

                OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Replace, value, index));
            }
        }

        public void Move(int index, int newIndex)
        {
            var item = items[index];

            items.RemoveAt(index);
            items.Insert(newIndex, item);
            item.Index = newIndex;

            OnChanged(new ChangedEvent<TItem>(NotifyCollectionChangedAction.Move, item, index));
        }

        public virtual void OnChanged(ChangedEvent<TItem> obj) => Changed?.Invoke(obj);

        public void FixIndices()
        {
            for (var i = 0; i < items.Count; i++) 
                items[i].Index = i;
        }

        public void Move(int fromIndex, IContainer newOwner)
        {
            // Move items with index >= fromIndex
            var array = items.Skip(fromIndex).ToArray();
            RemoveRange(fromIndex, array.Length);
            newOwner.AddItems(array);
        }
        
        public ICacheable GetLastItem() => items.LastOrDefault() as ICacheable;
        public ICacheable GetFirstItem() => items.FirstOrDefault() as ICacheable;
        public T GetItem<T>(int index) where T : class => index > -1 && index < items.Count ? items[index] as T : null;
        public bool Remove(IItem item) => RemoveRange((item as TItem).ToMany());
        public void Add(IItem item) => AddRange((item as TItem).ToMany());
        public void Insert(IItem item, int index) => InsertRange(index, (item as TItem).ToMany());

        public void Move(IItem item, int newIndex)
        {
            if (items?.Any() == true
                && newIndex != item.Index
                && newIndex > -1
                && newIndex < items.Count)
            {
                Move(item.Index, newIndex);
            }
        }

        public void AddItems(IEnumerable addItems) => AddRange(addItems.OfType<TItem>());
        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
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