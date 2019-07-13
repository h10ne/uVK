using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using uVK.Helpers;
using uVK.Model;
using VkNet.Model.RequestParams;
using uVK.States;
using uVK.Styles.AudioStyles;
using System.Collections.ObjectModel;

namespace uVK.ViewModel
{
    public class PlayerViewModel : BaseViewModel
    {
        public PlayerViewModel()
        {
            MusicList = new ListBox();
            SaveAudiosList = new ListBox();
            PlayLists = new ObservableCollection<PlayList>();
            AlbumAudiosList = new ListBox();
            SaveAudios.AddCache();
            PlayerModel.AddCacheToList(SaveAudiosList);
            if (SaveAudiosList.Items.Count != 0)
                NoSaveMusic = Visibility.Hidden;

            PlayerModel.Audio = ApiDatas.api.Audio.Get(new AudioGetParams { Count = ApiDatas.api.Audio.GetCount(UserDatas.User_id) }).ToList();
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

            var playlists = ApiDatas.api.Audio.GetPlaylists(UserDatas.User_id).ToList();
            foreach(var pl in playlists)
            {
                PlayList playList = new PlayList(pl);
                PlayLists.Add(playList);
            }

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
        public Visibility _noSaveMusic = Visibility.Visible;
        private int currentPlayList;
        private ListBox _saveAudiosList;
        private ListBox _musicList;
        private ListBox _albumAudiosList;
        private int _volume;
        private string _imageSource = @"/Images/ImageMusic.png";
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
        private string _selectedSaveItem;
        private int _selectedSaveIndex;
        private string _selectedAlbumAudiosItem;
        private int _selectedAlbumAudiosIndex;
        private ObservableCollection<PlayList> _playLists;
        private int _currentPlaylist = -1;
        private Visibility _textChooseAlbumVisibility = Visibility.Visible;
        #endregion

        #region Public properties
        public Visibility NoSaveMusic { get { return _noSaveMusic; } set { _noSaveMusic = value; OnPropertyChanged(nameof(NoSaveMusic)); } }
        public string ImageSource { get { return _imageSource; } set { _imageSource = value; OnPropertyChanged(nameof(ImageSource)); } }
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public string Artist { get { return _artist; } set { _artist = value; OnPropertyChanged(nameof(Artist)); } }
        public ListBox MusicList { get { return _musicList; } set { _musicList = value; OnPropertyChanged(nameof(MusicList)); } }
        public ListBox AlbumAudiosList { get { return _albumAudiosList; } set { _albumAudiosList = value; OnPropertyChanged(nameof(AlbumAudiosList)); } }
        public ListBox SaveAudiosList { get { return _saveAudiosList; } set { _saveAudiosList = value; OnPropertyChanged(nameof(SaveAudiosList)); } }
        public Visibility TextChooseAlbumVisibility { get { return _textChooseAlbumVisibility; } set { _textChooseAlbumVisibility = value; OnPropertyChanged(nameof(TextChooseAlbumVisibility)); } }
        public int CurrentPlaylistIndex { get { return _currentPlaylist; }
            set
            {
                int beforeValue = _currentPlaylist;
                _currentPlaylist = value;

                if (PlayerModel.state != PlayerModel.State.album)
                {
                    PlayerModel.state = PlayerModel.State.album;
                }
                foreach (var pl in PlayLists)
                {
                    pl.isPlay.IsChecked = false;
                }
                TextChooseAlbumVisibility = Visibility.Hidden;
                PlayerModel.Playlist = new Playlist(new AlbumAudios(PlayLists[_currentPlaylist].Audios, this));
                PlayLists[_currentPlaylist].isPlay.IsChecked = true;
                PlayerModel.Playlist.SetAudioInfo(this);
                OnPropertyChanged(nameof(CurrentPlaylistIndex));
            }
        }
        //Кнопки
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
        //Время
        public string CurrentTimePosition { get { return _currentTimePosition; } set { _currentTimePosition = value; OnPropertyChanged(nameof(CurrentTimePosition)); } }
        public double CurrentTimePositionValue { get { return _currentTimePositionValue; }
            set
            {
                _currentTimePositionValue = value;
                int sec = (int) _currentTimePositionValue % 60;
                int min = (int)_currentTimePositionValue / 60;
                string secStr;
                string minStr;
                if (sec > 9)
                    secStr = sec.ToString();
                else
                    secStr = "0" + sec.ToString();

                if (min > 9)
                    minStr = min.ToString();
                else
                    minStr = "0" + min.ToString();
                CurrentTimePosition = $"{minStr}:{secStr}";
                OnPropertyChanged(nameof(CurrentTimePositionValue));
            } }
        public string MaximumTimePosition { get { return _maximumTimePosition; } set { _maximumTimePosition = value; OnPropertyChanged(nameof(MaximumTimePosition)); } }
        public double DurrationMaximum { get { return _durrationMaximum; } set { _durrationMaximum = value; OnPropertyChanged(nameof(DurrationMaximum)); } }
        //Выбранное
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
        public int SelectedSaveIndex { get { return _selectedSaveIndex; } set { _selectedSaveIndex = value; OnPropertyChanged(nameof(SelectedSaveIndex)); } }
        public string SelectedSaveItem { get { return _selectedSaveItem; } set { _selectedSaveItem = value; OnPropertyChanged(nameof(SelectedSaveItem)); } }
        public int SelectedAlbumAudiosIndex { get { return _selectedAlbumAudiosIndex; } set { _selectedAlbumAudiosIndex = value; OnPropertyChanged(nameof(SelectedAlbumAudiosIndex)); } }
        public string SelectedAlbumAudiosItem { get { return _selectedAlbumAudiosItem; } set { _selectedAlbumAudiosItem = value; OnPropertyChanged(nameof(SelectedAlbumAudiosItem)); } }

        public ObservableCollection<PlayList> PlayLists { get { return _playLists; } set { _playLists = value; OnPropertyChanged(nameof(PlayLists)); } }
        #endregion

        #region Notification

        private string _marginNotification = "0,10,0,0";
        public string MarginNotification { get { return _marginNotification; } set { _marginNotification = value; OnPropertyChanged(nameof(MarginNotification)); } }

        private string _notificationText = "Downloading";
        public string NotificationText { get { return _notificationText; } set { _notificationText = value; OnPropertyChanged(nameof(NotificationText)); GetAnimation(); } }

        private async void GetAnimation()
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 10; i > -120; i -= 10)
                {
                    MarginNotification = $"0,{i},0,0";
                    Thread.Sleep(15);
                }
                Thread.Sleep(1000);
                for (int i = -120; i < 10; i += 10)
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

        public RelayCommand SetAlbumAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    PlayerModel.Playlist.SetAudioInfo(this, fromClick: true);
                    IsPlay = true;
                });
            }
        }
        public RelayCommand SetSaveAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (PlayerModel.state != PlayerModel.State.save)
                        PlayerModel.state = PlayerModel.State.save;
                    PlayerModel.Playlist = new Playlist(new SavesAudios());
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
            SaveAudios.AddCache();
            PlayerModel.AddCacheToList(SaveAudiosList);
        }
        #endregion

    }
}
