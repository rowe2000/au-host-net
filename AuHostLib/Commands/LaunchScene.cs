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
            undoScene = PluginGraph.Instance.Document.CurrentScene;
            PluginGraph.Instance.Document?.Launch(Cache.GetItem<Scene>(SceneId));

            return base.Execute();
        }

        public override bool Undo()
        {
            PluginGraph.Instance.Document?.Launch(undoScene);

            return base.Undo();
        }

    }
}