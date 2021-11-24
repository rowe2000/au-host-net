using System;
using System.Collections.Generic;

namespace AuHost.Plugins
{
    public class Document : Scene
    {
        public event Action<SceneChangeArgument> SceneChanged;
        public static int MaxId { get; private set; }

        public PresetLibrary PresetLibrary { get; } = new PresetLibrary();

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

            OnSceneChanged(new SceneChangeArgument(scene, oldScene));
        }

        public static int GetNextId()
        {
            MaxId++;
            return MaxId;
        }

        public Scene GetInitialScene()
        {
            return GetItem<Scene>(CurrentSceneId) ?? this;
        }

        public int CurrentSceneId { get; set; }

        protected virtual void OnSceneChanged(SceneChangeArgument obj)
        {
            SceneChanged?.Invoke(obj);
        }
    }
}