using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AuHost.Plugins
{
    public abstract class Cacheable<TItem, TParent> : Item<TParent>, ICacheable
        where TItem : class, IItem
        where TParent : class, IContainer
    {
        public new Container<TItem> Items => (Container<TItem>) base.Items;

        protected Cacheable() : base(Cache.Instance.GetNextId())
        {
            base.Items = new Container<TItem>();
            Items.CollectionChanged += OnCollectionChanged;
            Items.PropertyChanged += OnPropertyChanged;
        }

        public ICacheable GetPreviousDeep()
        {
            if (Parent == null)
                return Items.GetLastItem();

            var item = GetPreviousSibling<ICacheable>();
            if (item == null)
                return Parent as ICacheable;

            while (item?.HasItems() == false)
                item = item.GetLastItem();

            return item;
        }

        public ICacheable GetNextDeep()
        {
            var item = Items.GetFirstItem();
            if (item != null) 
                return item;
            
            item = this;
            while (item?.Parent != null)
            {
                var nextSibling = item.GetNextSibling<ICacheable>();
                if (nextSibling != null)
                    return nextSibling;

                item = item.Parent as Cacheable<TItem, TParent>;
            }

            return item;
        }

        public virtual T GetItem<T>(int index) where T : class => Items.GetItem<T>(index);

        public bool Remove(IItem item) => Items.Remove(item);

        public List<T> GetPath<T>() where T : class, IContainer => Items.GetPath<T>();

        public int DeepCount<T>() => Items.DeepCount<T>();

        public T GetItemDeep<T>(int id) where T : class => Items.GetItemDeep<T>(id);

        public bool HasItems() => Items.HasItems();

        public ICacheable GetLastItem() => Items.GetLastItem();

        public ICacheable GetFirstItem() => Items.GetFirstItem();

        public void Add(IItem item) => Items.Add(item);

        public void Insert(IItem item, int index) => Items.Insert(item, index);

        public void Move(IItem item, int index) => Items.Move(item, index);

        public void AddItems(IEnumerable addItems) => Items.AddItems(addItems);
        public void Move(int fromIndex, IContainer newOwner) => Items.Move(fromIndex, newOwner);
        public void Clear() => Items.Clear();

        public int Count => Items.Count;
        
        public void Move(int index, int newIndex) => Items.Move(index, newIndex);

        public void FixIndices() => Items.FixIndices();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}