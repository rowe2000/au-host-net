using System;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SplitZone : Command
    {
        private Zone newZone;

        public override bool SaveInScene => true;
        public int SplitNote { get; set; }
        public int ZoneId { get; set; }
        public int StripIndex { get; set; }


        SplitZone(Zone zone, int stripIndex)
        {
            var prevSplit = zone.SplitRange.Start;
            if (prevSplit == 0)
                prevSplit = 30;

            var nextSplit = 90;
            var nextZone = zone.GetNextSibling<Zone>();
            if (nextZone != null)
                nextSplit = nextZone.SplitRange.End;

            StripIndex = stripIndex;
            ZoneId = zone.Id;
            SplitNote = (nextSplit + prevSplit) / 2;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var zone = Cache.Instance.GetItem<Zone>(ZoneId);
            if (zone == null) 
                return false;
            
            var zoneIndex = zone.Index;

            if (StripIndex == 0 && zoneIndex > 0)
            {
                // this should to be glued with previous zone instead of splitting
                throw new Exception("this should to be glued with previous zone instead of splitting");
            }
            
            var rack = zone.Parent;
            if (rack == null)
                return false;
            
            var addZone = new AddZone(rack, zoneIndex + 1);
            Push(addZone);
            newZone = addZone.NewZone;
            newZone.SetSplitNote(SplitNote);

            zone.Items.Move(StripIndex, newZone.Items);

            if (StripIndex == 0)
                Push(new AddStrip(zone, StripIndex, StripType.Instrument));

            return base.Execute();

        }
        
        public override bool Undo()
        {
            if (!base.Undo()) 
                return false;
            
            return true;
        }
    }
}