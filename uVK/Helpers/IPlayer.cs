using WMPLib;

namespace uVK.Helpers
{
    public abstract class IPlayer
    {
        public abstract void Stop();
        public abstract void Play();
        public abstract void Pause();
        public virtual string Url { get; set; }
        public virtual double CurrentPosition { get; set; }
        public virtual string CurrentPositionString { get; set; }
        public virtual double Duration { get; set; }
        public virtual string DurrationString { get; set; }
        public virtual int Volume { get; set; }
        public virtual string Status { get; set; }
    }

    public class WindowsPlayer : IPlayer
    {
        public WindowsPlayer()
        {
            _player = new WindowsMediaPlayer();
        }

        private readonly WMPLib.WindowsMediaPlayer _player;
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
        public override string Url
        {
            get => _player.URL;
            set => _player.URL = value;
        }
        public override double CurrentPosition
        {
            get => _player.controls.currentPosition;
            set => _player.controls.currentPosition = value;
        }

        public override string CurrentPositionString
        {
            get => _player.controls.currentPositionString;
        }

        public override double Duration => _player.currentMedia.duration;
        public override string DurrationString => _player.currentMedia.durationString;
        public override int Volume
        {
            get => _player.settings.volume;
            set => _player.settings.volume = value;
        }
        public override string Status => _player.status;

    }
}
