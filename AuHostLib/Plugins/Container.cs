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
    public sealed class Container<TItem> : ObservableRangeCollection<TItem>, IContainer
        where TItem : class, IItem
    {
        private readonly IParent owner;

        public Container(IParent owner)
        {
            this.owner = owner;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            
            if (e.OldItems != null)
                foreach (TItem item in e.OldItems)
                {
                    item.Index = -1;
                    item.Parent = null;
                }

            var index = 0;
            foreach (var item in this)
            {
                item.Id = index++;
                item.Parent = owner;
            }
        }

        public T GetItemDeep<T>(int id) where T : class
        {
            foreach (var item in this)
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

        public bool HasItems() => this.Any();

        public List<T> GetPath<T>() where T : class, IParent
        {
            var item = owner as T;
            var path = new List<T>();

            while (item != null)
            {
                path.Insert(0, item);
                item = (item as IItem)?.Parent as T;
            }

            return path;
        }

        public int DeepCount<T>() => Count + this.Sum(item => (item as IContainer)?.DeepCount<T>()) ?? 0;

        public void InsertRange(int index, IEnumerable<TItem> insertItems)
        {
            var i = index;
            
            foreach (var item in insertItems)
            {
                if (item is null)
                    continue;
                
                item.Parent = owner;
                item.Index = index;
                base.Insert(i++, item);
            }
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0 && count < 0 && index + count >= Count)
                throw new ArgumentOutOfRangeException();

            RemoveRange(this.Skip(index).Take(count));
        }

        public new TItem this[int index]
        {
            get => base[index];
            set
            {
                var old = base[index];
                old.Index = -1;
                old.Parent = null;
                
                value.Index = index;
                value.Parent = owner;
                base[index] = value;
            }
        }

        public void FixIndices()
        {
            for (var i = 0; i < Count; i++) 
                base[i].Index = i;
        }

        public void Move(int fromIndex, IContainer newOwner)
        {
            // Move this with index >= fromIndex
            var array = this.Skip(fromIndex).ToArray();
            RemoveRange(fromIndex, array.Length);
            newOwner.AddItems(array);
        }
        
        public IItem GetLastItem() => this.LastOrDefault();
        public IItem GetFirstItem() => this.FirstOrDefault();
        public T GetItem<T>(int index) where T : class => index > -1 && index < Count ? base[index] as T : null;
        public bool Remove(IItem item)
        {
            if (item is TItem t) base.Remove(t);
            return true;
        }

        public void Add(IItem item)
        {
            if (item is TItem t) base.Add(t);
        }

        public void Insert(int index, IItem item)
        {
            if (item is TItem t) base.Insert(index, t);
        }

        public void Move(IItem item, int newIndex)
        {
            if (this.Any()
                && newIndex != item.Index
                && newIndex > -1
                && newIndex < Count)
            {
                Move(item.Index, newIndex);
            }
        }

        public void AddItems(IEnumerable addItems) => AddRange(addItems.OfType<TItem>());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}