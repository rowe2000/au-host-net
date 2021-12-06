using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuHost.Annotations;
using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.ViewModels
{
    public class BaseViewModel<TItem, TSubItem> : INotifyPropertyChanged
    {
        public TItem Item
        {
            get => item;
            set
            {
                item = value;
                OnPropertyChanged();
            }
        }

        private ObservableRangeCollection<TSubItem> items;
        private TItem item;

        public ObservableRangeCollection<TSubItem> Items
        {
            get => items;
            set
            {
                items = value; 
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}