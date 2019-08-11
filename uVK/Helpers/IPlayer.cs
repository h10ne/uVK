using WMPLib;

namespace uVK.Helpers
{
    public abstract class IPlayer
    {
        public abstract void Stop();
        public abstract void Play();
        public abstract void Pause();
        public string Url;
        public double CurrentPosition;
        public string CurrentPositionString;
        public double Duration;
        public string DurrationString;
        public int Volume;
        public string Status;
    }

    public class WindowsPlayer : IPlayer
    {
        private readonly WMPLib.WindowsMediaPlayer _player = new WindowsMediaPlayer();
        public override void Stop()
        {
            _player.controls.stop();
        }

        public override void Play()
        {
            _player.controls.play();
        }
        public override void Pause()
        {
            _player.controls.pause();
        }
        public new string Url
        {
            get => _player.URL;
            set => _player.URL = value;
        }
        public new double CurrentPosition
        {
            get => _player.controls.currentPosition;
            set => _player.controls.currentPosition = value;
        }
        
        public new string CurrentPositionString
        {
            get => _player.controls.currentPositionString;
        }
        
        public new double Duration => _player.currentMedia.duration;
        public new string DurrationString => _player.currentMedia.durationString;
        public new int Volume
        {
            get => _player.settings.volume;
            set => _player.settings.volume = value;
        }
        public new string Status => _player.status;

    }
}
