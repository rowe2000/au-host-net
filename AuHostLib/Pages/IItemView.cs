using AuHost.Plugins;

namespace AuHost.Pages
{
    public interface IItemView<out TItem>
    {
        TItem Item { get; }
    }
}