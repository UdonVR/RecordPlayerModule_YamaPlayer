
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDKBase;
using VRC.Udon;
using Yamadev.YamaStream;

namespace UdonVR.RecordPlayer.Yamaplayer
{

    public class RecordPlayer_YamaPlayer_Listener : YamaPlayerListener
    {
        public RecordPlayer_YamaPlayer recordPlayerProxy;

        private void Start()
        {
            recordPlayerProxy.yamaController.AddListener(this);
            recordPlayerProxy.recordPlayer.volumeSlider._Internal_UpdateValue(recordPlayerProxy._GetVolume(), false);
        }
        
        #region Core Events

        public override void AfterVideoReady()
        {
            recordPlayerProxy.recordPlayer.volumeSlider._Internal_UpdateValue(recordPlayerProxy._GetVolume(), false);
        }

        public override void AfterVideoStarted()
        {
        }

        public override void AfterVideoErrorOccurred(VideoError videoError)
        {
            recordPlayerProxy._PlaybackEnded();
        }

        public override void AfterVideoLooped()
        {
        }

        public override void AfterVideoPlayed()
        {
        }

        public override void AfterVideoPaused()
        {
        }

        public override void AfterVideoEnded()
        {
            recordPlayerProxy._PlaybackEnded();
        }

        public override void AfterVideoStopped()
        {
            recordPlayerProxy._End();
        }

        public override void AfterVideoReloaded()
        {
        }

        #endregion

        #region Playback Events

        public override void AfterPlayerHandlerChanged(VideoPlayerType playerType)
        {
        }

        public override void AfterTimeChanged(float time)
        {
        }

        public override void AfterLoopChanged(bool loop)
        {
        }

        public override void AfterShufflePlayChanged(bool shufflePlay)
        {
        }

        public override void AfterSpeedChanged(float speed)
        {
        }

        public override void AfterRepeatChanged(ulong repeat)
        {
        }

        public override void AfterLocalDelayChanged(float localDelay)
        {
        }

        #endregion

        #region Screen Events

        //public override void AfterTextureUpdated(Texture texture) { }
        //public override void AfterMirrorFlipChanged(bool mirrorFlip) { }
        //public override void AfterBrightnessChanged(float brightness) { }
        //public override void AfterMaxResolutionChanged(int maxResolution) { }

        #endregion

        #region Audio Events

        public override void AfterVolumeChanged(float volume)
        {
            recordPlayerProxy.recordPlayer.volumeSlider._Internal_UpdateValue(volume, false);
        }

        public override void AfterMuteChanged(bool mute)
        {
        }

        #endregion

        #region Other Events

        public override void AfterTrackUpdated()
        {
        }

        public override void AfterQueueUpdated()
        {
        }

        public override void AfterHistoryUpdated()
        {
        }

        public override void AfterPlaylistsUpdated()
        {
        }

        public override void AfterTrackSynced()
        {
        }

        public override void AfterTrackLoaded()
        {
            recordPlayerProxy._OnVideoLoad();
        }

        public override void AfterOwnerChanged()
        {
        }

        public override void AfterVideoRetry()
        {
        }

        #endregion
    }
}