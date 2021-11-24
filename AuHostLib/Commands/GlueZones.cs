using AuHost.Plugins;

namespace AuHost.Commands
{
    public class GlueZones : Command
    {
        public override bool SaveInScene => true;
        public int ZoneId { get; set; }
        public override bool Execute()
        {
            var zone = Cache.GetItem<Zone>(ZoneId);
            if (zone != null)
            {
                if (zone.Index == 0)
                    return false;

                var glueZone = zone.GetPreviousSibling<Zone>();

                zone.Move(0, glueZone);

                Push(new RemoveZone(zone));

                return base.Execute();
            }

            return false;
        }

        public override bool Undo()
        {
            if (!base.Undo()) 
                return false;
            
            return true;
        }

    }
}