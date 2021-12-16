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
            AddStripCmd = new Xamarin.Forms.Command(AddNewStrip);
            AddMidiPropCmd = new Xamarin.Forms.Command(() => { });
        }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Interval<int> SplitRange { get; }

        public ICommand AddStripCmd { get; }

        public ICommand AddMidiPropCmd { get; }


        public void AddNewStrip() => PluginGraph
            .Instance
            .CommandExecutor
            .Execute<AddStrip>(this, StripType.Instrument, -1);

        public void SetSplitNote(int startNote)
        {
            //TODO: Create NoteTransform class
            SplitRange.Start = startNote;
            foreach(var strip in Items)
            	 strip.NoteTransform.zoneSplit = SplitRange;

            var endNote = startNote - 1;

            var previousZone = GetPreviousSibling<Zone>();
            previousZone.SplitRange.End = endNote;
            foreach(var strip in previousZone.Items)
            	strip.NoteTransform.zoneSplit = previousZone.SplitRange;
        }

        public void OnMidiMessageReceived(MidiMessage[] midiMessages)
        {
            foreach (var item in Items)
            {
                //TODO: Split notes by zones
                item.OnMidiMessageReceived(midiMessages);
            }
        }
    }
}