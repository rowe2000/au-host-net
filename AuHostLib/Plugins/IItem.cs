namespace AuHost.Plugins
{
    public interface IItem : IParent
    {
        int Id { get; set; }
        string Name { get; set; }
        IParent Parent { get; set; }
        int Index { get; set; }

        T GetNextSibling<T>() where T : class;
        T GetPreviousSibling<T>() where T : class;
        T GetParent<T>() where T : class, IParent;
        
        IItem GetPreviousDeep();

        IItem GetNextDeep();
    }
    
    public interface IItem<TContainer> : IItem
    {
        new TContainer Parent { get; set;}
   } 
}