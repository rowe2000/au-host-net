using System;
using AuHost.ViewModels;
using Xamarin.Forms;

namespace AuHost.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var mainPageViewModel = new MainPageViewModel();
            BindingContext = mainPageViewModel;
            FrameView.Item = mainPageViewModel.Frame;
        }
    }
}

