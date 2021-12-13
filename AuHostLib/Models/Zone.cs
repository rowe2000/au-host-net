using System;
using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;

namespace AuHost.Models
{
    public class Zone : Slotable<Strip, Rack>, IPresetable
    {
        public Zone()
        {
            AddStripCmd = new Xamarin.Forms.Command(OnAddNewStrip);
            AddMidiPropCmd = new Xamarin.Forms.Command(() => { });
        }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Interval<int> SplitRange { get; }

        public ICommand AddStripCmd { get; }

        public ICommand AddMidiPropCmd { get; }
        

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

        public void OnMidiMessageReceived(MidiMessage[] midiMessages)
        {
            foreach (var item in Items)
            {
                item.OnMidiMessageReceived(midiMessages);
            }
        }
    }
}