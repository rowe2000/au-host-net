using AuHost.Plugins;

namespace AuHost.Commands
{
    public class RemoveRack : Command
    {
        private Rack rack;
        private int rackIndex;
        private Frame frame;

        public override bool SaveInScene => true;
        public int RackId { get; set; }
        
        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            rack = Cache.Instance.GetItem<Rack>(RackId);
            rackIndex = rack.Index;
            frame = rack.Parent;
            frame.Items.Remove(rack);

            return base.Execute();
        }

        public override bool Undo()
        {
            frame.Items.Insert(rackIndex, rack);
            rack = null;
         
            return base.Undo();
        }
    }
}