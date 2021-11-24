namespace AuHost.Plugins
{
    public interface IItem
    {
        int Id { get; set; }
        string Name { get; set; }
        IContainer Parent { get; set; }
        IContainer Items { get; }
        int Index { get; set; }
        
        T GetNextSibling<T>() where T : class;
        T GetPreviousSibling<T>() where T : class;
        T GetParent<T>() where T : class, IContainer;
    }
    
    public interface IItem<TContainer> : IItem
    {
        new TContainer Parent { get; set; }
    }

}