using AuHost.Plugins;

namespace AuHost.Commands
{
    public class LaunchScene : Command
    {
        private Scene undoScene;

        public override bool SaveInScene => false;
        public int SceneId { get; set; }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            undoScene = pluginGraph.Document.CurrentScene;
            pluginGraph.Document?.Launch(Cache.Instance.GetItem<Scene>(SceneId));

            return base.Execute();
        }

        public override bool Undo()
        {
            PluginGraph.Instance.Document?.Launch(undoScene);

            return base.Undo();
        }

    }
}