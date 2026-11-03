using UdonSharp;
using UdonVR.RecordPlayer;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDKBase;
using VRC.Udon;
using Yamadev.YamaStream;

namespace UdonVR.RecordPlayer.Yamaplayer
{
    public class RecordPlayer_YamaPlayer : RecordPlayer_VideoplayerClass
    {
        public RecordPlayer_YamaPlayer_Listener yamaListener;
        public Controller yamaController;

        private bool ignoreStopEvent = false;
        private bool isLoopingUrl = false;

        private void Start()
        {
            yamaController.AddListener(yamaListener);
            recordPlayer.volumeSlider._Internal_UpdateValue(_GetVolume(), false);
        }

        public override void _LoadUrl(VRCUrl _url)
        {
            ignoreStopEvent = true;
            RecordPlayer_Disk disk = recordPlayer.records[recordPlayer.currentRecord];
            string wow = string.IsNullOrWhiteSpace(disk.trackNames[recordPlayer.currentTrack])
                ? " "
                : $" {disk.trackNames[recordPlayer.currentTrack]} - ";
            yamaController.TakeOwnership();
            yamaController.PlayTrack(TrackUtils.NewTrack(VideoPlayerType.AVProVideoPlayer, $"[RecordPlayer]{wow}{disk.albumName} - {disk.artistName}", _url));
        }

        public void _OnYamaTrackLoaded()
        {
            // Clear ignoreStopEvent after LoadTrack's internal Stop() calls are done
            ignoreStopEvent = false;
            _OnVideoLoad();
        }

        public void _OnYamaVideoStarted()
        {
            if (isLoopingUrl)
            {
                isLoopingUrl = false;
                RecordPlayer_Disk disk = recordPlayer.records[recordPlayer.currentRecord];
                if (disk.CheckChapters(recordPlayer.currentTrack) && recordPlayer.currentChapter > 0)
                {
                    _ChangePlaybackTime(disk.chapterData[recordPlayer.currentTrack].chapterTimeStamps[recordPlayer.currentChapter]);
                }
            }
        }

        public void _OnYamaVideoStopped()
        {
            // Ignore Stop() calls triggered internally by PlayTrack/LoadTrack/SetPlayerType
            if (isExpectingUrl || ignoreStopEvent) return;

            // Delay 1 frame in case AfterVideoStopped was triggered by AfterVideoEnded's internal Handler.Stop()
            SendCustomEventDelayedFrames(nameof(_CheckDeferredStop), 1);
        }

        public void _CheckDeferredStop()
        {
            if (isExpectingUrl || ignoreStopEvent) return;
            // Only stop the Record Player if YamaPlayer is actually still Idle/Stopped and not loading the next track
            if (yamaController.Stopped && !yamaController.IsLoading)
            {
                _End();
            }
        }

        public void _OnYamaVideoEnded()
        {
            ignoreStopEvent = true;
            _PlaybackEnded();
            ignoreStopEvent = false;
        }

        public void _OnYamaVideoError(VideoError videoError)
        {
            // Wait until YamaPlayer finishes its internal retries (IsLoading is false)
            if (!yamaController.IsLoading)
            {
                ignoreStopEvent = true;
                _PlaybackEnded();
                ignoreStopEvent = false;
            }
        }

        public override float _GetPlaybackTime()
        {
            float curtime = yamaController.VideoTime;
            float totalTime = yamaController.Duration;
            return Remap(curtime, 0, totalTime, 0, 1);
        }

        public override float _GetPlaybackDuration_Current()
        {
            return yamaController.VideoTime;
        }

        public override float _GetPlaybackDuration_End()
        {
            return yamaController.Duration;
        }

        public override bool _IsPaused()
        {
            return yamaController.Paused;
        }

        public override bool _CanPlayVideo()
        {
            return true;
        }

        public override bool _IsVideoplayerOwner()
        {
            return Networking.IsOwner(yamaController.gameObject);
        }

        public override void _Stop()
        {
            yamaController.Stop();
        }

        public override void _Pause()
        {
            if (_IsPaused())
            {
                yamaController.Play();
            }
            else
            {
                yamaController.Pause();
            }
        }

        public override void _loopCurrentUrl()
        {
            if (yamaController.Stopped)
            {
                RecordPlayer_Disk disk = recordPlayer.records[recordPlayer.currentRecord];
                isExpectingUrl = true;
                isLoopingUrl = true;
                _LoadUrl(disk.urls[recordPlayer.currentTrack]);
            }
            else
            {
                yamaController.SetTime(0f);
            }
        }

        public override void _SetVolume(float _volume)
        {
            yamaController.Volume = _volume;
        }

        public override float _GetVolume()
        {
            return yamaController.Volume;
        }

        public override void _SetTitle(RecordPlayer_Disk _disk)
        {
        }

        public override void _ChangePlaybackTime(float _time)
        {
            yamaController.SetTime(_time);
        }
    }
}