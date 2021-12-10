using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;

namespace AuHost.Models
{
    public class Zone : Slotable<Strip, Rack>, IPresetable
    {
        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Interval<int> SplitRange { get; }

        public ICommand AddNewStrip => new Xamarin.Forms.Command(OnAddNewStrip);

        public void OnAddNewStrip()
        {
            var addStrip = new AddStrip(this, Items.Count, StripType.Instrument);
            PluginGraph.Instance.CommandExecutor.Execute(addStrip);
        }

        public void SetSplitNote(int startNote)
        {
            //TODO: Create NoteTransform class
            SplitRange.Start = startNote;
            foreach(Strip item in Items)
            	 item.NoteTransform.zoneSplit = SplitRange;

            var endNote = startNote - 1;

            var previousZone = GetPreviousSibling<Zone>();
            previousZone.SplitRange.End = endNote;
            foreach(Strip item in previousZone.Items)
            	item.NoteTransform.zoneSplit = previousZone.SplitRange;
        }
    }
}