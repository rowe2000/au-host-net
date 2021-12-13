using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;

namespace AuHost.Models
{
    public class Frame : IParent
    {
        IContainer IParent.Items => Items;
        public Container<Rack> Items { get; }
        
        public ICommand AddNewRackCmd { get; }
        public ICommand AddMidiPropCmd { get; }

        public Frame()
        {
            AddNewRackCmd = new Xamarin.Forms.Command(() => AddNewRack());
            AddMidiPropCmd = new Xamarin.Forms.Command(() => {});
            
            Items = new Container<Rack>(this);
        }

        public void FixStripNumbers()
        {
            var number = 1;
            foreach (var rack in Items)
            foreach (var zone in rack.Items)
            foreach (var strip in zone.Items)
                strip.Number = number++;
        }

        public void AddNewRack(bool before = false)
        {
            var pluginGraph = PluginGraph.Instance;
            var document = pluginGraph.Document;
            
            var currentScene = document.CurrentScene;
            document.Launch(document);

            var rackIndex = Items.Count;
            var addRack = new AddRack(rackIndex);
            pluginGraph.CommandExecutor.Execute(addRack);

            document.Launch(currentScene);

            addRack.NewRack.AddNewZone();
        }
    }
}