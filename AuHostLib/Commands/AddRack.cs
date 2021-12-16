using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddRack : Command
    {
        public override bool SaveInScene => true;
        public int RackIndex { get; }
        public int RackId { get; set; }
        public Rack NewRack { get; private set; }

        public AddRack(int rackIndex = -1)
        {
            RackIndex = rackIndex;
            RackId = Cache.Instance.GetNextId();
        }

        public override bool Execute()
        {
            NewRack = Cache.Instance.CreateWithId<Rack>(RackId);
            var frame = PluginGraph.Instance.Frame;
            var rackIndex = RackIndex < 0 ? frame.Items.Count : RackIndex;
            frame.Items.Insert(rackIndex, NewRack);

            Push(new SelectRack(NewRack));

            return base.Execute();
        }

        public override bool Undo()
        {
            NewRack.RemoveFromParent();

            return base.Undo();
        }
   
    }
}