using System.Windows.Input;
using Xamarin.Forms;

namespace AuHost.Plugins
{
    public class Zone : Cacheable<Strip, Rack>, IPresetable
    {
        public Zone() : base(Document.GetNextId())
        {
        }
        
        public Preset Preset { get; set; }
        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Interval<int> SplitRange { get; }

        public ICommand AddStripTask => new Command(() => PluginGraph.Instance.AddNewStrip(this));

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