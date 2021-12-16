using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuHost.Annotations;
using AuHost.Plugins;
using Foundation;
using IContainer = AuHost.Plugins.IContainer;

namespace AuHost.Models
{
    public abstract class Item : NSObject, INotifyPropertyChanged
    {
        private int id = -1;
        private int index = -1;
        private string name = "";
        private IParent parent;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => id;
            set
            {
                if (id >= 0) 
                    throw new Exception($"Id is already set for {GetType().Name}");
                
                if (id == value)
                    return;
                
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        
        public int Index
        {
            get => index;
            set
            {
                if (index == value)
                    return;
                
                index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name == value)
                    return;
                
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public IParent Parent
        {
            get => parent;
            protected set
            {
                if (parent == value)
                    return;
                
                parent = value; 
                OnPropertyChanged(nameof(Parent));
            }
        }

        public T GetParent<T>() where T : class, IParent
        {
            return Parent as T ?? (Parent as IItem)?.GetParent<T>();
        }

        public static void SetId(Item instance, int id)
        {
            instance.Id = id;
        }
    }

    public abstract class Item<TParent> : Item, IItem 
        where TParent : class, IParent
    {
        private IContainer items;
        public IContainer Items
        {
            get => items;
            protected set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public new TParent Parent
        {
            get => base.Parent as TParent;
            private set => base.Parent = value;
        }

        public T GetNextSibling<T>() where T : class => Parent?.Items.GetItem<T>(Index + 1);
        public T GetPreviousSibling<T>() where T : class => Parent?.Items.GetItem<T>(Index - 1);
        public void MoveTo(int newIndex) => Parent?.Items.Move(this, newIndex);

        public bool RemoveFromParent() => Parent?.Items.Remove(this) ?? false;
        IParent IItem.Parent
        {
            get => Parent;
            set => Parent = value as TParent;
        }

        public IItem GetPreviousDeep()
        {
            if (Parent == null)
                return Items.GetLastItem();

            var item = GetPreviousSibling<IItem>();
            if (item == null)
                return Parent as IItem;

            while (item?.Items.HasItems() == false)
                item = item.Items.GetLastItem();

            return item;
        }

        public IItem GetNextDeep()
        {
            var item = Items.GetFirstItem();
            if (item != null) 
                return item;
            
            item = this;
            while (item?.Parent != null)
            {
                var nextSibling = item.GetNextSibling<IItem>();
                if (nextSibling != null)
                    return nextSibling;

                item = item.Parent as Item<IItem, TParent>;
            }

            return item;
        }
    }
    
    public abstract class Item<TSubItem, TParent> : Item<TParent>
        where TSubItem : class, IItem
        where TParent : class, IParent
    {
        public new Container<TSubItem> Items => base.Items as Container<TSubItem>;

        protected Item()
        {
            base.Items = new Container<TSubItem>(this);
            base.Items.CollectionChanged += (o, a) => GetParent<Frame>()?.FixStripNumbers();
        }
    }
}
    