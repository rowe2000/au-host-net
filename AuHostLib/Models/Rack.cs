using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;

namespace AuHost.Models
{
    public class Rack : Slotable<Zone, Frame>, IPresetable
    {
        public ICommand AddZoneCmd { get; }

        public object MidiInputs
        {
            get { throw new System.NotImplementedException(); }
        }

        public ICommand AddMidiPropCmd { get; }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Rack()
        {
            AddZoneCmd = new Xamarin.Forms.Command(() => AddNewZone());
            AddMidiPropCmd = new Xamarin.Forms.Command(() => {});
        }

        public void AddNewZone(bool before = false)
        {
            var zoneIndex = PluginGraph.Instance.SelectedZone?.Index + (before ? 0 : 1) ?? Items.Count;
            var addZone = new AddZone(this, zoneIndex);
            PluginGraph.Instance.CommandExecutor.Execute(addZone);

            addZone.NewZone.OnAddNewStrip();
        }
    }
}