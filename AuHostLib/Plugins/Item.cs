using Foundation;

namespace AuHost.Plugins
{
    public abstract class Item<TParent> : NSObject, IItem<TParent> where TParent : class, IContainer
    {
        protected Item(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public TParent Parent { get; set; }

        protected IContainer Items { get; set; }

        IContainer IItem.Items => Items;

        public int Index { get; set; } = -1;
        public T GetNextSibling<T>() where T : class => Parent.GetItem<T>(Index + 1);
        public T GetPreviousSibling<T>() where T : class => Parent.GetItem<T>(Index - 1);

        public void MoveTo(int newIndex) => Parent?.Move(this, newIndex);

        public bool RemoveFromParent() => Parent?.Remove(this) ?? false;
        IContainer IItem.Parent
        {
            get => Parent;
            set => Parent = value as TParent;
        }
        
        public T GetParent<T>() where T : class, IContainer
        {
            return Parent as T ?? (Parent as IItem)?.GetParent<T>();
        }

    }
}
    