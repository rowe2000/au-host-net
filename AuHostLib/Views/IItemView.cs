namespace AuHost.Views
{
    public interface IItemView<TItem>
    {
        TItem Item { get; set; }
    }
}