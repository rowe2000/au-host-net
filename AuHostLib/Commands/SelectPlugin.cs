using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SelectPlugin : Command
    {
        private Plugin undoPlugin;

        public override bool SaveInScene => false;
        public int PluginId { get; set; }
        
        public override bool Execute()
        {
            var plugin = Cache.GetItem<Plugin>(PluginId);

            if (plugin == null)
                   return false;

            undoPlugin = PluginGraph.Instance.SelectedPlugin;

            PluginGraph.Instance.SelectPlugin(plugin);

            return base.Execute();
        }

        public override bool Undo()
        {
            if (!base.Undo()) 
                return false;
            
            PluginGraph.Instance.SelectPlugin(undoPlugin);
            return true;
        }
    }
}