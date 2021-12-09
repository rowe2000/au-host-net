namespace AuHost.Plugins
{
    public abstract class Slotable<TChild, TParent> : Item<TChild, TParent>
        where TParent : class, IParent
        where TChild : class, IItem
    {
        public Preset Preset { get; set; }
    }
}