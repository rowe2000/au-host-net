using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class LoadPluginPreset : Command
    {
        private Plugin plugin;
        private Preset previousPreset;

        public override bool SaveInScene => true;
        public int PluginId { get; set; }
        public int PluginPresetId { get; set; }
        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            plugin = Cache.Instance.GetItem<Plugin>(PluginId);
            var preset = Cache.Instance.GetItem<Preset>(PluginPresetId);

            previousPreset = plugin.GetOrCreatePreset();
            plugin.Preset = preset;

            return base.Execute();
        }

        public override bool Undo()
        {
            PopAll();

            plugin.Preset = previousPreset;

            return base.Undo();
        }

    }
}