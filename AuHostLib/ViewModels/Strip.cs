using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.ViewModels
{
    public class Strip : Item
    {
        public ObservableRangeCollection<Plugin> Plugins { get; } = new ObservableRangeCollection<Plugin>();
    }
}