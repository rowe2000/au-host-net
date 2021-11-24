using System.Windows.Input;
using Xamarin.Forms;

namespace AuHost.Plugins
{
    public class Strip : Cacheable<Plugin, Zone>, IPresetable
    {
        public NoteTransform NoteTransform;
        public Preset Preset { get; set; }

        public ICommand AddPluginTask => new Command(() => PluginGraph.Instance.AddPlugin());

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Strip() : base(Document.GetNextId())
        {
        }
    }
}