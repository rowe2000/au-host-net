using AppKit;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace AuHost
{
    [Register(nameof(AppDelegate))  ]           
    public class AppDelegate : FormsApplicationDelegate
    {
        readonly NSWindow window;
        public override NSWindow MainWindow => window;

        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(100, 100, 1024, 768);
            window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            window.Title = "TEST";
            window.TitleVisibility = NSWindowTitleVisibility.Hidden;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        //private List<AudioComponent> GetAudioComponents()
        //{
        //    AudioComponentDescription desc = new AudioComponentDescription();

        //    AudioComponent audioComponent = null;
        //    audioComponent = AudioComponent.FindNextComponent(audioComponent, ref desc);

        //    var audioComponents = new List<AudioComponent>();
        //    while (audioComponent != null)
        //    {
        //        audioComponents.Add(audioComponent);
        //        audioComponent = AudioComponent.FindNextComponent(audioComponent, ref desc);
        //    }

        //    return audioComponents;
        //}
    }
}
