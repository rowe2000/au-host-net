using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.ViewModels
{
    public class Zone : Item
    {
        public ObservableRangeCollection<Strip> Strips { get; } = new ObservableRangeCollection<Strip>();
    }
}