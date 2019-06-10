using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using uVK.Model;
using VkNet.Model.RequestParams;

namespace uVK.ViewModel
{
    public class PlayerViewModel:BaseViewModel
    {
        public PlayerViewModel()
        {
            PlayerModel.Audio = ApiDatas.api.Audio.Get(new AudioGetParams { Count = ApiDatas.api.Audio.GetCount(UserDatas.User_id) });
            MusicList = new ListBox();
            PlayerModel.AddAudioToList(PlayerModel.Audio, MusicList);
            PlayerModel.Playlist = new Playlist(new OwnAudios());
            PlayerModel.Playlist.SetAudioInfo(this);
            Volume = 30;
            PlayerModel.Player.controls.stop();
            BidloEvent();
        }
        private async void BidloEvent()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    int _volume = Volume;
                    Thread.Sleep(100);
                    if (_volume!=Volume)
                    {
                        PlayerModel.Player.settings.volume = Volume;
                    }
                    CurrentTimePosition = PlayerModel.Player.controls.currentPositionString;
                    CurrentTimePositionValue = PlayerModel.Player.controls.currentPosition;
                }
            });
        }

        #region Private members
        private ListBox _musicList;
        private int _volume;
        private bool _isPlay = false;
        private bool _random = false;
        private bool _repeat;
        private string _title;
        private string _artist;
        private int _selectedIndex;
        private string _currentTimePosition = "00:00";
        private string _maximumTimePosition;
        private double _currentTimePositionValue;
        private double _durrationMaximum;
        #endregion


        #region Public properties
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public  string Artist { get { return _artist; } set { _artist = value; OnPropertyChanged(nameof(Artist)); } }
        public ListBox MusicList { get { return _musicList; } set { _musicList = value; OnPropertyChanged(nameof(MusicList)); } }
        public int Volume { get { return _volume; } set { _volume = value; OnPropertyChanged(nameof(Volume)); } }
        public bool IsPlay { get { return _isPlay; } set { _isPlay = value; OnPropertyChanged(nameof(IsPlay)); } }
        public bool Random { get { return _random; } set { _random = value; OnPropertyChanged(nameof(Random)); } }
        public bool Repeat { get { return _repeat; } set { _repeat = value; OnPropertyChanged(nameof(Repeat)); } }
        public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; OnPropertyChanged(nameof(SelectedIndex)); } }
        public string CurrentTimePosition { get { return _currentTimePosition; } set { _currentTimePosition = value; OnPropertyChanged(nameof(CurrentTimePosition)); } }
        public double CurrentTimePositionValue { get { return _currentTimePositionValue; } set { _currentTimePositionValue = value; OnPropertyChanged(nameof(CurrentTimePositionValue)); } }
        public string MaximumTimePosition { get { return _maximumTimePosition; } set { _maximumTimePosition = value; OnPropertyChanged(nameof(MaximumTimePosition)); } }
        public double DurrationMaximum { get { return _durrationMaximum; } set { _durrationMaximum = value; OnPropertyChanged(nameof(DurrationMaximum)); } }
        #endregion


        #region Commands

        public RelayCommand PlayPauseCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (IsPlay)
                    {
                        IsPlay = false;
                        PlayerModel.Player.controls.stop();
                    }
                    else
                    {
                        PlayerModel.Player.controls.play();
                        IsPlay = true;
                    }
                });
            }
        }
        public RelayCommand MuteCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    Volume = 0;
                }, (obj)=> Volume>0);
            }
        }
        public RelayCommand FullLoudCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    Volume = 100;
                }, (obj) => Volume < 100);
            }
        }
        public RelayCommand NextSong
        {
            get {
                return new RelayCommand((obj) =>
               {
                   PlayerModel.Playlist.NextSong(this);
               });
            }
        }
        public RelayCommand PrevSong
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    PlayerModel.Playlist.PrevSong(this);
                });
            }
        }
        #endregion

    }
}
