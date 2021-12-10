using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SelectRack : Command
    {
        private Rack undoRack;

        public override bool SaveInScene => false;
        public int RackId { get; set; }


        public SelectRack(Rack rack)
        {
            RackId = rack.Id;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var rack = Cache.Instance.GetItem<Rack>(RackId);
            if (rack == null)
                return false;

            undoRack = pluginGraph.SelectedRack;

            pluginGraph.SelectRack(rack);

            return base.Execute();
        }

        public override bool Undo()
        {
            if (!base.Undo()) 
                return false;
            
            PluginGraph.Instance.SelectRack(undoRack);
            return true;

        }   
    }
}