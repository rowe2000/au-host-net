namespace AuHost.Plugins
{
    public class SceneChangeArgument
    {
        public Scene NewScene { get; }
        public Scene OldScene { get; }

        public SceneChangeArgument(Scene newScene, Scene oldScene)
        {
            NewScene = newScene;
            OldScene = oldScene;
        }
    }
}