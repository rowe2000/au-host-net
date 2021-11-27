using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddRack : Command
    {
        public override bool SaveInScene => true;
        public int RackIndex { get; set; }
        public int RackId { get; set; }
        public Rack NewRack { get; set; }

        public AddRack(int rackIndex)
        {
            RackIndex = rackIndex;
        }

        public override bool Execute()
        {
            NewRack = Cache.Create<Rack>();
            RackId = NewRack.Id;
            
            PluginGraph.Instance.Frame.Insert(NewRack, RackIndex);

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