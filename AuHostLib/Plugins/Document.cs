using System;
using System.Collections.Generic;

namespace AuHost.Plugins
{
    public sealed class Document : Scene
    {
        public PresetLibrary PresetLibrary { get; } = new PresetLibrary();

        public event Action<SceneChangeArgument> SceneChanged;
        public int MaxId { get; set; }

        public Scene CurrentScene { get; set; }

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
            return GetItem<Scene>(CurrentSceneId) ?? this;
        }

        public int CurrentSceneId { get; set; }

        private void OnSceneChanged(SceneChangeArgument obj)
        {
            SceneChanged?.Invoke(obj);
        }
    }
}