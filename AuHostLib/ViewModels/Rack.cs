using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.ViewModels
{
    public class Rack : Item
    {
        public ObservableRangeCollection<Zone> Zones { get; } = new ObservableRangeCollection<Zone>();

    }
}