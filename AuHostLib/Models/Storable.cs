using AuHost.Plugins;

namespace AuHost.Models
{
    public class Storable<TChild, TParent> : Item<TChild, TParent> 
        where TChild : class, IItem 
        where TParent : class, IParent
    {
    }
}