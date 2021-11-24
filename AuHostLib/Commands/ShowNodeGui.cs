using AuHost.Plugins;

namespace AuHost.Commands
{
    public class ShowNodeGui : Command
    {
        private bool undoState;
        private Plugin plugin;

        public override bool SaveInScene => false;
        public int PluginId { get; set; }
        public override bool Execute()
        {
            plugin = Cache.GetItem<Plugin>(PluginId);
            undoState = plugin.IsWindowShowing();

            if (Show)
                plugin.ShowWindow(true);
            else if (!undoState)
                plugin.ShowWindow(false);

            return base.Execute();
        }

        public bool Show { get; set; }

        public override bool Undo()
        {
            plugin.ShowWindow(undoState);

            return base.Undo();
        }
    }
}