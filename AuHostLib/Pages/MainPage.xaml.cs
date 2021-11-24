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
            BindingContext = new MainViewModel();
        }
    }
}

