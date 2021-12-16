using System.Linq;
using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SavePluginPreset : Command
    {
        private Preset preset;
        public override bool SaveInScene => false;

        public Plugin Plugin { get; set; }

        public SavePluginPreset(Plugin plugin)
        {
            Plugin = plugin;
        }

        public override bool Execute()
        {
            preset = Plugin.GetOrCreatePreset();
            PluginGraph.Instance.Document.PresetLibrary.Items.Add(preset);

            return base.Execute();
        }

        public override bool Undo()
        {
            PluginGraph.Instance.Document.PresetLibrary.Items.Remove(preset);
            
            return base.Undo();
        }
    }
}