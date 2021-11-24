using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddRack : Command
    {
        private Rack rack;

        public override bool SaveInScene => true;
        public int RackIndex { get; set; }
        public int RackId { get; set; }
        
        public AddRack(int rackIndex)
        {
            RackIndex = rackIndex;
        }

        public override bool Execute()
        {
            rack = Cache.Create<Rack>();
            RackId = rack.Id;
            PluginGraph.Instance.Frame.Insert(rack, RackIndex);

            Push(new SelectRack(rack));

            return base.Execute();
        }

        public override bool Undo()
        {
            rack.RemoveFromParent();

            return base.Undo();
        }
   
    }
}