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
using System.Windows.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData.Binding;
using DynamicData;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace uVK.ViewModel
{
    public class PlayerViewModel : ReactiveObject
    {
        public PlayerViewModel()
        {
            SaveAudios.AddCache();
            PlayerModel.AddCacheToList(SaveAudiosList);
            if (SaveAudiosList.Items.Count != 0)
                NoSaveMusic = Visibility.Hidden;
            PlayerModel.GetUserAudio();
            State = PlayerModel.PlaylistState.Own;
            //Асинхронная загрузка аудио пользователя
            var sourceOwnMusic = new SourceList<OneAudioViewModel>();
            sourceOwnMusic.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(UserAudios).DisposeMany().Subscribe();
            PlayerModel.AddAudioToListAsync(PlayerModel.Audio, sourceOwnMusic);
            //Установка параментров плеера
            PlayerModel.Playlist = new Playlist(new OwnAudios(this));
            PlayerModel.Playlist.SetAudioInfo(this);
            Volume = 30;
            PlayerModel.Player.controls.stop();

            //Асинхронное получение плейлистов
            var sourceAlbums = new SourceList<AlbumViewModel>();
            sourceAlbums.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(PlayLists).DisposeMany().Subscribe();
            PlayerModel.GetPlaylistsAsync(UserDatas.UserId, sourceAlbums);
            //Ассинхронная загрузка друзей
            var sourceFriendsMusic = new SourceList<FriendsMusicViewModel>();
            sourceFriendsMusic.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(FriendsMusic).DisposeMany()
                .Subscribe();
            PlayerModel.DownloadFriendsWithOpenAudioAsync(sourceFriendsMusic);
            FriendsMusicSelectedIndex = -1;


            _durrationTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 20)
            };
            _durrationTimer.Tick += DurrationTimer_Tick;
            _durrationTimer.Start();
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
        private int _friendsMusicSelectedIndex;
        private double _currentTimePositionValue;
        private readonly DispatcherTimer _durrationTimer;
        private string _searchRequest = "";
        private bool _isDownloading;
        private int _currentPlaylist = -1;
        private string _notificationText = "Downloading";
        private int _friendsMusicAlbumSelectedIndex = -1;

        #endregion

        #region Public properties

        public PlayerModel.PlaylistState State
        {
            get => _state;
            set
            {
                _state = value;
                if (value != PlayerModel.PlaylistState.Album)
                    CurrentPlaylistIndex = -1;
            }
        }

        //Коллекции
        [Reactive]
        public ObservableCollectionExtended<AlbumViewModel> PlayLists { get; set; } =
            new ObservableCollectionExtended<AlbumViewModel>();

        [Reactive]
        public ObservableCollectionExtended<AlbumViewModel> FriendsMusicAlbums { get; set; } =
            new ObservableCollectionExtended<AlbumViewModel>();

        [Reactive]
        public ObservableCollectionExtended<FriendsMusicViewModel> FriendsMusic { get; set; } =
            new ObservableCollectionExtended<FriendsMusicViewModel>();

        [Reactive]
        public ObservableCollectionExtended<OneAudioViewModel> FriendsMusicAudios { get; set; } =
            new ObservableCollectionExtended<OneAudioViewModel>();

        [Reactive]
        public ObservableCollectionExtended<OneAudioViewModel> UserAudios { get; set; } =
            new ObservableCollectionExtended<OneAudioViewModel>();

        [Reactive]
        public ObservableCollectionExtended<OneAudioViewModel> AlbumAudios { get; set; } =
            new ObservableCollectionExtended<OneAudioViewModel>();

        public int FriendsMusicAlbumSelectedIndex
        {
            get => _friendsMusicAlbumSelectedIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _friendsMusicAlbumSelectedIndex, value);
                if (value == -1)
                    return;
                State = PlayerModel.PlaylistState.Album;
                PlayerModel.Playlist =
                    new Playlist(new AlbumAudios(FriendsMusicAlbums[FriendsMusicAlbumSelectedIndex].Audios, this));
                PlayerModel.Playlist.SetAudioInfo(this);
                PlayerModel.Player.controls.play();
            }
        }

        public int FriendsMusicSelectedIndex
        {
            get => _friendsMusicSelectedIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _friendsMusicSelectedIndex, value);
                if (value == -1)
                    return;
                var source = new SourceList<OneAudioViewModel>();
                source.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(FriendsMusicAudios).DisposeMany()
                    .Subscribe();
                FriendsMusicAudios.Clear();
                PlayerModel.AddAudioToListAsync(null, source, 600, FriendsMusic[value].Id);
                FriendsMusicAlbums.Clear();
                var sourceAlbums = new SourceList<AlbumViewModel>();
                sourceAlbums.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(FriendsMusicAlbums)
                    .DisposeMany().Subscribe();
                PlayerModel.GetPlaylistsAsync(FriendsMusic[FriendsMusicSelectedIndex].Id, sourceAlbums);

                State = PlayerModel.PlaylistState.Null;
            }
        }

        [Reactive] public int FriendsMusicAudiosSelectedIndex { get; set; }
        [Reactive] public Visibility NoSaveMusic { get; set; } = Visibility.Visible;
        [Reactive] public string ImageSource { get; set; } = @"/Images/ImageMusic.png";
        [Reactive] public string Title { get; set; }
        [Reactive] public string Artist { get; set; }
        [Reactive] public ListBox SaveAudiosList { get; set; } = new ListBox();
        [Reactive] public Visibility TextChooseAlbumVisibility { get; set; } = Visibility.Visible;

        public int CurrentPlaylistIndex
        {
            get => _currentPlaylist;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPlaylist, value);
                if (value == -1)
                {
                    AlbumAudios.Clear();
                    return;
                }

                if (State != PlayerModel.PlaylistState.Album)
                {
                    State = PlayerModel.PlaylistState.Album;
                }

                TextChooseAlbumVisibility = Visibility.Hidden;
                PlayerModel.Playlist = new Playlist(new AlbumAudios(PlayLists[_currentPlaylist].Audios, this));
                PlayerModel.Playlist.SetAudioInfo(this);
            }
        }

        //Кнопки
        public int Volume
        {
            get => _volume;
            set
            {
                PlayerModel.Player.settings.volume = value;
                this.RaiseAndSetIfChanged(ref _volume, value);
            }
        }

        [Reactive] public bool IsPlay { get; set; }

        public RelayCommand PlayPause
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (!IsPlay)
                    {
                        PlayerModel.Player.controls.pause();
                        //DurrationTimer.Stop();
                    }
                    else
                    {
                        PlayerModel.Player.controls.play();
                        _durrationTimer.Start();
                    }
                });
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
            get => _currentTimePositionValue;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentTimePositionValue, value);
                CurrentTimePosition = Decoder.ConvertTimeToString((int) _currentTimePositionValue);
            }
        }

        [Reactive] public string MaximumTimePosition { get; set; }

        [Reactive] public double DurrationMaximum { get; set; }

        //Выбранное
        [Reactive] public OneAudioViewModel SelectedItem { get; set; }

        public string SearchRequest
        {
            get => _searchRequest;
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

        [Reactive]
        public string NotificationText
        {
            get => _notificationText;
            set
            {
                _notificationText = value;
                GetAnimation();
            }
        }

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
            get { return new RelayCommand((obj) => { _durrationTimer.Stop(); }); }
        }

        public RelayCommand MouseUp
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    PlayerModel.Player.controls.currentPosition = CurrentTimePositionValue;
                    CurrentTimePosition = PlayerModel.Player.controls.currentPositionString;
                    _durrationTimer.Start();
                });
            }
        }

        public RelayCommand SearchCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    State = PlayerModel.PlaylistState.Search;
                    PlayerModel.Search(SearchRequest, UserAudios);
                });
            }
        }

        public RelayCommand MuteCommand
        {
            get { return new RelayCommand((obj) => { Volume = 0; }, (obj) => Volume > 0); }
        }

        public RelayCommand FullLoudCommand
        {
            get { return new RelayCommand((obj) => { Volume = 100; }, (obj) => Volume < 100); }
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

                    if (State != PlayerModel.PlaylistState.Own && SearchRequest == "")
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

        public RelayCommand SetFriendsAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (State != PlayerModel.PlaylistState.IdAudios)
                    {
                        State = PlayerModel.PlaylistState.IdAudios;
                        PlayerModel.Playlist = new Playlist(new IdAudios());
                    }

                    PlayerModel.Playlist.SetAudioInfo(this, fromClick: true);
                });
            }
        }

        public RelayCommand SetAlbumAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (State == PlayerModel.PlaylistState.Album)
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
                    if (State != PlayerModel.PlaylistState.Save)
                        State = PlayerModel.PlaylistState.Save;
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
                    _isDownloading = true;
                    NotificationText = "Загрузка";
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                    webClient.DownloadFileAsync(new Uri(PlayerModel.Player.URL),
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\SaveAudios\\" +
                        Artist + "↨" + Title);
                }, (obj) => !_isDownloading);
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //GetAnimation();
            NotificationText = "Завершено";
            _isDownloading = false;
            SaveAudios.AddCache();
            PlayerModel.AddCacheToList(SaveAudiosList);
        }

        #endregion
    }
}