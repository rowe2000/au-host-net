using AuHost.Plugins;

namespace AuHost.Commands
{
    public class LaunchNextScene : Command
    {
        private Scene undoScene;

        public override bool SaveInScene => false;
        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            undoScene = pluginGraph.Document.CurrentScene;
            pluginGraph.Document?.Launch((Scene)undoScene.GetNextDeep());

            return base.Execute();
        }

        public override bool Undo()
        {
            PluginGraph.Instance.Document?.Launch(undoScene);
            return base.Undo();
        }
    }
}