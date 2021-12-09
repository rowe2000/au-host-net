using System.Collections.Generic;
using System.Linq;
using AuHost.Commands;

namespace AuHost.Plugins
{
    public class Scene : Storable<Scene, Scene>
    {
        public List<Command> Commands { get; } = new List<Command>();
        
        public int Bar { get; set; }
        public string Beat { get; set; }
        public float Tempo { get; set; }
        public string  Key { get; set; }
        public int Transpose { get; set; }

        public List<Scene> GetPath() => Items.GetPath<Scene>();

        public void Launch()
        {
            foreach (var command in Commands) 
                command.Execute();
        }

        public void UnLaunch()
        {
            for (var i = Commands.Count - 1; i >= 0; i--)
            {
                Commands[i].Undo();
            }
        }

        public Scene InsertNewScene(string name, int index)
        {
            var pluginGraph = PluginGraph.Instance;
            
            var scene = Cache.Instance.Create<Scene>();

            var texts = name.Split(',');
            if (texts.Any())
                scene.Name = texts[0];

            foreach (var text in texts.Skip(1))
            {
                if (text.Contains("/") || text.Contains(":"))
                    scene.Beat = text;
                else if (text.ContainsOnly("0123456789."))
                    scene.Tempo = float.TryParse(text, out var f) ? f : 0;
                else if (text.ContainsOnly("ABCDEFGHb#m-"))
                    scene.Key = text;
            }

            Items.Insert(index, scene);

            return scene;        }

        public bool RemoveCommand(Command command)
        {
            return Commands.Remove(command);
        }

        public void AddCommand(Command command)
        {
            Commands.Add(command);
        }
    }
}