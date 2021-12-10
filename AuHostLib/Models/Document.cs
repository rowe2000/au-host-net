using System;
using System.Collections.Generic;
using AuHost.Plugins;

namespace AuHost.Models
{
    public sealed class Document : Scene
    {
        private static int docCount;

        public int MaxId { get; set; }
        public int CurrentSceneId { get; set; }

        public PresetLibrary PresetLibrary { get; } = new PresetLibrary();
        public Scene CurrentScene { get; set; }

        public event Action<SceneChangeArgument> SceneChanged;

        public Document()
        {
            Name = "New GigRig " + ++docCount;
        }

        public void Launch(Scene newScene)
        {
            var newScenes = newScene?.GetPath() ?? new List<Scene>();
            var launchedScenes = CurrentScene?.GetPath() ?? new List<Scene>();

            // Parent Scenes that are the same should not be affected
            var pathIndex = 0;
            while (pathIndex < newScenes.Count
                   && pathIndex < launchedScenes.Count
                   && newScenes[pathIndex] == launchedScenes[pathIndex])
            {
                pathIndex++;
            }

            // Unlaunch current scenes that are different to new path
            for (var i = launchedScenes.Count - 1; i >= pathIndex; --i)
            {
                launchedScenes[i].UnLaunch();
            }

            // Launch new scenes
            var scene = newScene;
            for (; pathIndex < newScenes.Count; ++pathIndex)
            {
                scene = newScenes[pathIndex];
                scene.Launch();
            }

            var oldScene = CurrentScene;
            CurrentScene = scene;
            CurrentSceneId = scene?.Id ?? 0;

            OnSceneChanged(new SceneChangeArgument(scene, oldScene));
        }
        
        public Scene GetInitialScene()
        {
            return Items.GetItem<Scene>(CurrentSceneId) ?? this;
        }
        
        private void OnSceneChanged(SceneChangeArgument obj)
        {
            SceneChanged?.Invoke(obj);
        }
    }
}