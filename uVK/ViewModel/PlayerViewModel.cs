using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using uVK.Helpers;
using uVK.Model;
using VkNet.Model.RequestParams;
using uVK.States;
using uVK.Styles.AudioStyles;
using System.Windows.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData.Binding;
using DynamicData;
using System.Reactive.Linq;

namespace uVK.ViewModel
{
    public class PlayerViewModel : ReactiveObject
    {
        int qwe = 0;
        public PlayerViewModel()
        {
            SaveAudios.AddCache();
            PlayerModel.AddCacheToList(SaveAudiosList);
            if (SaveAudiosList.Items.Count != 0)
                NoSaveMusic = Visibility.Hidden;
            PlayerModel.Audio = ApiDatas.api.Audio.Get(new AudioGetParams { Count = ApiDatas.api.Audio.GetCount(UserDatas.User_id) }).ToList();
            State = PlayerModel.PlaylistState.own;
            // PlayerModel.AddAudioToList(PlayerModel.Audio, UserAudios);
            // PlayerModel.Playlist = new Playlist(new OwnAudios(this));
            // PlayerModel.Playlist.SetAudioInfo(this);
            Volume = 30;
            PlayerModel.Player.controls.stop();

            DurrationTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 20)
            };
            DurrationTimer.Tick += DurrationTimer_Tick;

            //PlayerModel.Getplaylists(UserDatas.User_id, PlayLists);

            var source = new SourceList<FriendsMusicViewModel>();
            var canc = source.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(FriendsMusic).DisposeMany().Subscribe();

            //FriendsMusic = PlayerModel.DownloadFriendsWithOpenAudio();
            PlayerModel.DownloadFriendsWithOpenAudioAsync(source);           


            DurrationTimer.Start();
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
        private PlayerModel.PlaylistState _state;
        private int _volume;
        private bool _isPlay = false;
        private double _currentTimePositionValue;
        private DispatcherTimer DurrationTimer;
        private string _searchRequest = "";
        private bool isDownloading = false;
        private int _currentPlaylist = -1;
        #endregion

        #region Public properties
        public PlayerModel.PlaylistState State { get { return _state; } set { _state = value; if (value != PlayerModel.PlaylistState.album) CurrentPlaylistIndex = -1; } }
        //Коллекции
        [Reactive] public ObservableCollectionExtended<PlayList> PlayLists { get; set; } = new ObservableCollectionExtended<PlayList>();
        [Reactive] public ObservableCollectionExtended<PlayList> FriendsMusicAlbums { get; set; } = new ObservableCollectionExtended<PlayList>();
        [Reactive] public ObservableCollectionExtended<FriendsMusicViewModel> FriendsMusic { get; set; } = new ObservableCollectionExtended<FriendsMusicViewModel>();
        [Reactive] public ObservableCollectionExtended<AudioList> FriendsMusicAudios { get; set; } = new ObservableCollectionExtended<AudioList>();
        [Reactive] public ObservableCollectionExtended<AudioList> UserAudios { get; set; } = new ObservableCollectionExtended<AudioList>();
        [Reactive] public ObservableCollectionExtended<AudioList> AlbumAudios { get; set; } = new ObservableCollectionExtended<AudioList>();

        [Reactive] public Visibility NoSaveMusic { get; set; } = Visibility.Visible;
        [Reactive] public string ImageSource { get; set; } = @"/Images/ImageMusic.png";
        [Reactive] public string Title { get; set; }
        [Reactive] public string Artist { get; set; }
        [Reactive] public ListBox SaveAudiosList { get; set; } = new ListBox();
        [Reactive] public Visibility TextChooseAlbumVisibility { get; set; } = Visibility.Visible;
        public int CurrentPlaylistIndex
        {
            get { return _currentPlaylist; }
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPlaylist, value);
                if (value == -1)
                {
                    AlbumAudios.Clear();
                    return;
                }

                if (State != PlayerModel.PlaylistState.album)
                {
                    State = PlayerModel.PlaylistState.album;
                }
                TextChooseAlbumVisibility = Visibility.Hidden;
                PlayerModel.Playlist = new Playlist(new AlbumAudios(PlayLists[_currentPlaylist].Audios, this));
                PlayerModel.Playlist.SetAudioInfo(this);
            }
        }
        //Кнопки
        public int Volume { get { return _volume; } set { PlayerModel.Player.settings.volume = value; this.RaiseAndSetIfChanged(ref _volume, value); } }
        [Reactive]
        public bool IsPlay
        {
            get { return _isPlay; }
            set
            {
                this.RaiseAndSetIfChanged(ref _isPlay, value);
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
            }
        }
        [Reactive] public bool Random { get; set; }
        [Reactive] public bool Repeat { get; set; }
        [Reactive] public int SelectedIndex { get; set; }
        //Время
        [Reactive] public string CurrentTimePosition { get; set; } = "00:00";
        [Reactive]
        public double CurrentTimePositionValue
        {
            get { return _currentTimePositionValue; }
            set
            {
                this.RaiseAndSetIfChanged(ref _currentTimePositionValue, value);
                CurrentTimePosition = Helpers.Decoder.ConvertTimeToString((int)_currentTimePositionValue);
            }
        }
        [Reactive] public string MaximumTimePosition { get; set; }
        [Reactive] public double DurrationMaximum { get; set; }
        //Выбранное
        [Reactive] public AudioList SelectedItem { get; set; }
        [Reactive]
        public string SearchRequest
        {
            get { return _searchRequest; }
            set
            {
                this.RaiseAndSetIfChanged(ref _searchRequest, value);
                if (SearchRequest == "")
                {
                    PlayerModel.AddAudioToList(PlayerModel.Audio, UserAudios);
                }
            }
        }
        [Reactive] public int SelectedSaveIndex { get; set; }
        [Reactive] public string SelectedSaveItem { get; set; }
        [Reactive] public int SelectedAlbumAudiosIndex { get; set; }
        [Reactive] public string SelectedAlbumAudiosItem { get; set; }

        #endregion

        #region Notification

        [Reactive] public string MarginNotification { get; set; } = "0,10,0,0";
        [Reactive] public string NotificationText { get; set; } = "Downloading";

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
                    State = PlayerModel.PlaylistState.search;
                    PlayerModel.Search(SearchRequest, UserAudios);
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
                        PlayerModel.Playlist = new Playlist(new SearchAudios(this));
                    }
                    if (State != PlayerModel.PlaylistState.own && SearchRequest == "")
                    {
                        PlayerModel.Playlist = new Playlist(new OwnAudios(this));
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
                    if (State == PlayerModel.PlaylistState.album)
                    {
                        PlayerModel.Playlist.SetAudioInfo(this, fromClick: true);
                        IsPlay = true;
                    }
                    else
                    {
                        AlbumAudios.Clear();
                        NotificationText = "Choose playlist";
                    }
                });
            }
        }
        public RelayCommand SetSaveAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (State != PlayerModel.PlaylistState.save)
                        State = PlayerModel.PlaylistState.save;
                    PlayerModel.Playlist = new Playlist(new SavesAudios(this));
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
