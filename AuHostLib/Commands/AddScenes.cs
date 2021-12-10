using System.Collections.Generic;
using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddScenes : Command
    {
        private List<Scene> addedScenes = new List<Scene>();

        public override bool SaveInScene => false;
        public int InsertIndex { get; set; }
        public IEnumerable<string> Names { get; set; }
        public int OwnerSceneId { get; set; }
        
        public override bool Execute()
        {
            var ownerScene = Cache.Instance.GetItem<Scene>(OwnerSceneId);
            var index = InsertIndex;
            
            foreach (var name in Names)
                addedScenes.Add(ownerScene.InsertNewScene(name, index++));

            return base.Execute();
        }

        public override bool Undo()
        {
            foreach (var scene in addedScenes)
                scene.RemoveFromParent();

            return base.Undo();
        }
    }
}