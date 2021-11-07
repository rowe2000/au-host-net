using System;
using AppKit;
using CoreMidi;
using CoreAudioKit;
using Foundation;
using AudioUnit;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Globalization;
using AVFoundation;
using Mono;

namespace MacTest
{

    class AuController : NSViewController
    {
        public AuController(IntPtr handle) : base(handle)
        {
           
        }   
    }
    
    [Register(nameof(AppDelegate))  ]           
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {

        }

        public override void DidFinishLaunching(NSNotification notification)
        {

        }

        private List<AudioComponent> GetAudioComponents()
        {
            AudioComponentDescription desc = new AudioComponentDescription();

            AudioComponent audioComponent = null;
            audioComponent = AudioComponent.FindNextComponent(audioComponent, ref desc);

            var audioComponents = new List<AudioComponent>();
            while (audioComponent != null)
            {
                audioComponents.Add(audioComponent);
                audioComponent = AudioComponent.FindNextComponent(audioComponent, ref desc);
            }

            return audioComponents;
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }



    }
}
