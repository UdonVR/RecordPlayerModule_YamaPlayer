
using UdonSharp;
using UdonVR.RecordPlayer;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Yamadev.YamaStream;

namespace UdonVR.RecordPlayer.Yamaplayer
{

    public class RecordPlayer_YamaPlayer : RecordPlayer_VideoplayerClass
    {
        public RecordPlayer_YamaPlayer_Listener yamaListener;
        public Controller yamaController;

        public override void _LoadUrl(VRCUrl _url)
        {
            yamaController.TakeOwnership();
            yamaController.PlayTrack(TrackUtils.NewTrack(VideoPlayerType.AVProVideoPlayer,"", _url));
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