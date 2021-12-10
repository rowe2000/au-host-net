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

        public AddRack(int rackIndex)
        {
            RackIndex = rackIndex;
            RackId = Cache.Instance.GetNextId();
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            NewRack = Cache.Instance.CreateWithId<Rack>(RackId);
            pluginGraph.Frame.Items.Insert(RackIndex, NewRack);

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