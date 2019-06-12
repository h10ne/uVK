using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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
            PlayerModel.state = PlayerModel.State.own;
            PlayerModel.AddAudioToList(PlayerModel.Audio, MusicList);
            PlayerModel.Playlist = new Playlist(new OwnAudios());
            PlayerModel.Playlist.SetAudioInfo(this);
            Volume = 30;
            PlayerModel.Player.controls.stop();
            BidloEventForLoud();
            DurrationTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 800)
            };
            DurrationTimer.Tick += DurrationTimer_Tick;
            DurrationTimer.Start();
            
        }

        private void DurrationTimer_Tick(object sender, EventArgs e)
        {
            CurrentTimePosition = PlayerModel.Player.controls.currentPositionString;
            CurrentTimePositionValue = PlayerModel.Player.controls.currentPosition;
        }

        private async void BidloEventForLoud()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        int _volume = Volume;
                        Thread.Sleep(50);
                        if (_volume != Volume)
                        {
                            PlayerModel.Player.settings.volume = Volume;
                        }
                    }
                    catch { }
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
        private string _selectedItem;
        private System.Windows.Threading.DispatcherTimer DurrationTimer;
        private string _searchRequest = "";
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
        public string SelectedItem { get { return _selectedItem; } set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); } }
        public string SearchRequest { get { return _searchRequest; }
            set
            {
                _searchRequest = value;
                if (SearchRequest == "" && PlayerModel.state == PlayerModel.State.search)
                {
                    PlayerModel.state = PlayerModel.State.own;
                    PlayerModel.AddAudioToList(PlayerModel.Audio, MusicList);
                }
                OnPropertyChanged(nameof(SearchRequest));
            }
        }
        #endregion


        #region Commands

        public RelayCommand PlayPauseCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (!IsPlay)
                    {
                        //IsPlay = false;
                        DurrationTimer.Stop();
                        PlayerModel.Player.controls.pause();
                    }
                    else
                    {
                        DurrationTimer.Start();
                        PlayerModel.Player.controls.play();
                        //IsPlay = true;
                    }
                });
            }
        }   
        public RelayCommand SearchCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    PlayerModel.Search(SearchRequest, MusicList);
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
                   IsPlay = true;
               });
            }
        }
        public RelayCommand PrevSong
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    IsPlay = true;
                    PlayerModel.Playlist.PrevSong(this);
                });
            }
        }
        public RelayCommand SetAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    IsPlay = true;
                    PlayerModel.Playlist.SetAudioInfo(this, fromClick: true);
                });
            }
        }
        #endregion

    }
}
