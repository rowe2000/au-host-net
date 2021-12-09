// using System;
// using AuHost.Plugins;
// using Xamarin.Forms;
// using Xamarin.Forms.Xaml;
//
// namespace AuHost.Views
// {
//     [XamlCompilation(XamlCompilationOptions.Compile)]
//     public partial class StripView : ContentView, IItemView<Plugins.Strip>
//     {
//         private readonly StackLayoutHelper<Strip, PluginView, Plugin> helper;
//
//         public Strip Item
//         {
//             get => helper.Item;
//             set => helper.Item = value;
//         }
//
//         public StripView()
//         {
//             InitializeComponent();
//             helper = new StackLayoutHelper<Strip, PluginView, Plugin>(PluginStack);
//         }
//
//         private void OnAddPluginClicked(object sender, EventArgs e)
//         {
//         }
//     }
// }