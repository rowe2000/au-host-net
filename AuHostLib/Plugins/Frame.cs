using System.Windows.Input;
using AuHost.Commands;

namespace AuHost.Plugins
{
    public class Frame : Container<Rack>
    {
        public Frame()
        {
            AddNewRackCmd = new Xamarin.Forms.Command(() => AddNewRack());
        }

        public ICommand AddNewRackCmd { get; }

        public void AddNewRack(bool before = false)
        {
            var pluginGraph = PluginGraph.Instance;
            var document = pluginGraph.Document;
            
            var currentScene = document.CurrentScene;
            document.Launch(document);

            var rackIndex = pluginGraph.SelectedRack?.Index + (before ? 0 : 1) ?? pluginGraph.Frame.Count;
            var addRack = new AddRack(rackIndex);
            pluginGraph.CommandExecutor.Execute(addRack);

            document.Launch(currentScene);

            addRack.NewRack.AddNewZone();
        }
    }
}