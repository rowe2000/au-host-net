namespace AuHost.Plugins
{
    public abstract class Slotable<TChild, TParent> : Cacheable<TChild, TParent>
        where TParent : class, IContainer
        where TChild : class, IItem
    {
        public Preset Preset { get; set; }
    }
}