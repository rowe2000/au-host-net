using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class LaunchPreviousScene : Command
    {
        private Scene undoScene;

        public override bool SaveInScene => false;
        public override bool Execute()
        {
            undoScene = PluginGraph.Instance.Document.CurrentScene;
            PluginGraph.Instance.Document?.Launch((Scene)undoScene.GetPreviousDeep());

            return base.Execute();
        }

        public override bool Undo()
        {
            PluginGraph.Instance.Document?.Launch(undoScene);

            return base.Undo();
        }
    }
}