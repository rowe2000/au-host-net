namespace AuHost.Plugins
{
    public interface ICacheable : IItem, IContainer
    {
        ICacheable GetPreviousDeep();
        ICacheable GetNextDeep();
    }
}