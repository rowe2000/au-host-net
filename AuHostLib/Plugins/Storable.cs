namespace AuHost.Plugins
{
    public class Storable<TChild, TParent> : Cacheable<TChild, TParent> 
        where TChild : class, IItem 
        where TParent : class, IContainer
    {
        public Storable() : base(Document.GetNextId())
        {
        }
    }
}