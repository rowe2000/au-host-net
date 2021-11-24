using AuHost.Pages;
using AuHost.Plugins;
using Xamarin.Forms;

namespace AuHost
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            PluginGraph.Instance.Load();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

