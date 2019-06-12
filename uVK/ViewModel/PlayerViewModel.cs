using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using uVK.Model;
using VkNet.Model.RequestParams;

namespace uVK.ViewModel
{
    public class PlayerViewModel : BaseViewModel
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
            DurrationTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 20)
            };
            DurrationTimer.Tick += DurrationTimer_Tick;
        }

        private void DurrationTimer_Tick(object sender, EventArgs e)
        {
            CurrentTimePosition = PlayerModel.Player.controls.currentPositionString;
            CurrentTimePositionValue = PlayerModel.Player.controls.currentPosition;
            if (PlayerModel.Player.status == "Остановлено")
            {
                if (!Repeat)
                    PlayerModel.Playlist.NextSong(this);
                PlayerModel.Player.controls.play();
            }
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
        private bool isDownloading = false;
        #endregion


        #region Public properties
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public string Artist { get { return _artist; } set { _artist = value; OnPropertyChanged(nameof(Artist)); } }
        public ListBox MusicList { get { return _musicList; } set { _musicList = value; OnPropertyChanged(nameof(MusicList)); } }
        public int Volume { get { return _volume; } set { _volume = value; PlayerModel.Player.settings.volume = Volume; OnPropertyChanged(nameof(Volume)); } }
        public bool IsPlay
        {
            get { return _isPlay; }
            set
            {
                _isPlay = value;
                if (!_isPlay)
                {
                    DurrationTimer.Stop();
                    PlayerModel.Player.controls.pause();
                }
                else
                {
                    DurrationTimer.Start();
                    PlayerModel.Player.controls.play();
                }
                OnPropertyChanged(nameof(IsPlay));
            }
        }
        public bool Random { get { return _random; } set { _random = value; OnPropertyChanged(nameof(Random)); } }
        public bool Repeat { get { return _repeat; } set { _repeat = value; OnPropertyChanged(nameof(Repeat)); } }
        public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; OnPropertyChanged(nameof(SelectedIndex)); } }
        public string CurrentTimePosition { get { return _currentTimePosition; } set { _currentTimePosition = value; OnPropertyChanged(nameof(CurrentTimePosition)); } }
        public double CurrentTimePositionValue { get { return _currentTimePositionValue; } set { _currentTimePositionValue = value; OnPropertyChanged(nameof(CurrentTimePositionValue)); } }
        public string MaximumTimePosition { get { return _maximumTimePosition; } set { _maximumTimePosition = value; OnPropertyChanged(nameof(MaximumTimePosition)); } }
        public double DurrationMaximum { get { return _durrationMaximum; } set { _durrationMaximum = value; OnPropertyChanged(nameof(DurrationMaximum)); } }
        public string SelectedItem { get { return _selectedItem; } set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); } }
        public string SearchRequest
        {
            get { return _searchRequest; }
            set
            {
                _searchRequest = value;

                if (SearchRequest == "")
                {
                    //PlayerModel.state = PlayerModel.State.own;
                    PlayerModel.AddAudioToList(PlayerModel.Audio, MusicList);
                }
                OnPropertyChanged(nameof(SearchRequest));
            }
        }
        #endregion

        #region Notification
        private string _marginNotification = "0,430,0,0";
        public string MarginNotification { get { return _marginNotification; } set { _marginNotification = value; OnPropertyChanged(nameof(MarginNotification)); } }

        private string _notificationText = "Downloading";
        public string NotificationText { get { return _notificationText; } set { _notificationText = value; OnPropertyChanged(nameof(NotificationText)); GetAnimation(); } }

        private async void GetAnimation()
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 430; i > 270; i -= 10)
                {
                    MarginNotification = $"0,{i},0,0";
                    Thread.Sleep(15);
                }
                Thread.Sleep(1000);
                for (int i = 270; i < 430; i += 10)
                {
                    MarginNotification = $"0,{i},0,0";
                    Thread.Sleep(10);
                }
            });
        }

        #endregion

        #region Commands
        public RelayCommand MouseDown
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    DurrationTimer.Stop();
                });
            }
        }
        public RelayCommand MouseUp
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    PlayerModel.Player.controls.currentPosition = CurrentTimePositionValue;
                    CurrentTimePosition = PlayerModel.Player.controls.currentPositionString;
                    DurrationTimer.Start();
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
                }, (obj) => Volume > 0);
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
            get
            {
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
                    if (SearchRequest != "")
                    {
                        PlayerModel.Playlist = new Playlist(new SearchAudios());
                    }
                    if (PlayerModel.state != PlayerModel.State.own && SearchRequest == "")
                    {
                        PlayerModel.Playlist = new Playlist(new OwnAudios());
                        PlayerModel.OffsetOwn = 0;
                        PlayerModel.OffsetSearch = 0;
                    }
                    PlayerModel.Playlist.SetAudioInfo(this, fromClick: true);
                    IsPlay = true;
                });
            }
        }
        public RelayCommand SaveAudio
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    isDownloading = true;
                    NotificationText = "Загрузка";
                    //GetAnimation();
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                    webClient.DownloadFileAsync(new Uri(PlayerModel.Player.URL), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\SaveAudios\\" + Artist + "↨" + Title);
                }, (obj) => !isDownloading);
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //GetAnimation();
            NotificationText = "Завершено";
            isDownloading = false;
        }
        #endregion

    }
}
