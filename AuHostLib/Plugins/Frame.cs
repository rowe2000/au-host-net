using System.Windows.Input;
using AuHost.Commands;

namespace AuHost.Plugins
{
    public class Frame : IParent
    {
        IContainer IParent.Items => Items;
        public Container<Rack> Items { get; }
        
        public ICommand AddNewRackCmd { get; }

        public Frame()
        {
            AddNewRackCmd = new Xamarin.Forms.Command(() => AddNewRack());
            Items = new Container<Rack>(this);
        }

        public void AddNewRack(bool before = false)
        {
            var pluginGraph = PluginGraph.Instance;
            var document = pluginGraph.Document;
            
            var currentScene = document.CurrentScene;
            document.Launch(document);

            var rackIndex = pluginGraph.SelectedRack?.Index + (before ? 0 : 1) ?? pluginGraph.Frame.Items.Count;
            var addRack = new AddRack(rackIndex);
            pluginGraph.CommandExecutor.Execute(addRack);

            document.Launch(currentScene);

            addRack.NewRack.AddNewZone();
        }
    }
}