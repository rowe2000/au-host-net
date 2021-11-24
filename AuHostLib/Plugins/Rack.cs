using System.Windows.Input;
using Xamarin.Forms;

namespace AuHost.Plugins
{
    public class Rack : Cacheable<Zone, Frame>, IPresetable
    {
        public Preset Preset { get; set; }

        public ICommand AddZoneTask { get; }

        public int RackHeight => Items.Count * 50;

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Rack() : base(Document.GetNextId())
        {
            AddZoneTask = new Command(() => PluginGraph.Instance.AddNewZone(this));
        }
    }
}