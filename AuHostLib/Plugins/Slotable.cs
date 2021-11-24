namespace AuHost.Plugins
{
    public abstract class Slotable<TChild, TParent> : Cacheable<TChild, TParent>
        where TParent : class, IContainer
        where TChild : class, IItem
    {
        protected Preset Preset { get; set; }

        protected Slotable() : base(Document.GetNextId())
        {
        }
    }
}