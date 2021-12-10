using AuHost.Models;

namespace AuHost.Plugins
{
    public interface IPresetable
    {
        Preset Preset { get; set; }
        Preset GetOrCreatePreset();
    }
}