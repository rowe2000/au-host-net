using AVFoundation;

namespace AuHost.Plugins
{
    public class Connector
    {
        public Connector(AVAudioUnit avAudioUnit, int channelIndex)
        {
            AVAudioUnit = avAudioUnit;
            ChannelIndex = channelIndex;
        }

        public int ChannelIndex { get; }
        public AVAudioUnit AVAudioUnit { get; }
    }
}