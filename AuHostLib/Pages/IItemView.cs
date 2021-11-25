using AuHost.Plugins;

namespace AuHost.Pages
{
    public interface IItemView<TItem>
    {
        TItem Item { get; set; }
    }
}