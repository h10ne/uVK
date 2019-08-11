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
using uVK.Interfaces;

namespace uVK.ViewModel
{
    public class PlayerViewModel : ReactiveObject
    {
        
        public enum PlaylistState
        {
            Own,
            Save,
            Search,
            Album,
            IdAudios,
            Null
        }

        public IPlayerModel Model;
        public PlayerViewModel(IPlayerModel _model)
        {
            this.Model = _model;
            Player = new WindowsPlayer();
            Helpers.SaveAudios.AddCache();
            Model.AddCacheToList(SaveAudiosList);
            if (SaveAudiosList.Items.Count != 0)
                NoSaveMusic = Visibility.Hidden;
            Model.GetUserAudio();
            State = PlaylistState.Own;
            //Асинхронная загрузка аудио пользователя
            var sourceOwnMusic = new SourceList<OneAudioViewModel>();
            sourceOwnMusic.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(UserAudios).DisposeMany().Subscribe();
            Model.AddAudioToListAsync(ApiDatas.Audio, sourceOwnMusic);
            //Установка параментров плеера
            Playlist = new Playlist(new OwnAudios(this));
            Playlist.SetAudioInfo(this);
            Volume = 30;
            Player.Stop();
            //Асинхронное получение плейлистов
            var sourceAlbums = new SourceList<AlbumViewModel>();
            sourceAlbums.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(PlayLists).DisposeMany().Subscribe();
            Model.GetPlaylistsAsync(UserDatas.UserId, sourceAlbums);
            //Ассинхронная загрузка друзей
            var sourceFriendsMusic = new SourceList<FriendsMusicViewModel>();
            sourceFriendsMusic.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(FriendsMusic).DisposeMany()
                .Subscribe();
            Model.DownloadFriendsWithOpenAudioAsync(sourceFriendsMusic);
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
            CurrentTimePosition = Player.CurrentPositionString;
            CurrentTimePositionValue = Player.CurrentPosition;
            if (Player.Status == "Остановлено")
            {
                if (!Repeat)
                    Playlist.NextSong(this);
                Player.Play();
            }
        }


        #region Private members

        private PlaylistState _state;
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

        public List<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
        public WindowsPlayer Player { get; set; }
        private Playlist Playlist { get; set;   }

        public PlaylistState State
        {
            get => _state;
            set
            {
                _state = value;
                if (value != PlaylistState.Album)
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
                State = PlaylistState.Album;
                Playlist =
                    new Playlist(new AlbumAudios(FriendsMusicAlbums[FriendsMusicAlbumSelectedIndex].Audios, this));
                Playlist.SetAudioInfo(this);
                Player.Play();
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
                Model.AddAudioToListAsync(null, source, 600, FriendsMusic[value].Id);
                FriendsMusicAlbums.Clear();
                var sourceAlbums = new SourceList<AlbumViewModel>();
                sourceAlbums.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(FriendsMusicAlbums)
                    .DisposeMany().Subscribe();
                Model.GetPlaylistsAsync(FriendsMusic[FriendsMusicSelectedIndex].Id, sourceAlbums);

                State = PlaylistState.Null;
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

                if (State != PlaylistState.Album)
                {
                    State = PlaylistState.Album;
                }

                TextChooseAlbumVisibility = Visibility.Hidden;
                Playlist = new Playlist(new AlbumAudios(PlayLists[_currentPlaylist].Audios, this));
                Playlist.SetAudioInfo(this);
            }
        }

        //Кнопки
        public int Volume
        {
            get => _volume;
            set
            {
                Player.Volume = value;
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
                        Player.Pause();
                    }
                    else
                    {
                        Player.Play();
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
                    Model.AddAudioToList(ApiDatas.Audio, UserAudios);
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
                    Player.CurrentPosition = CurrentTimePositionValue;
                    CurrentTimePosition = Player.CurrentPositionString;
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
                    State = PlaylistState.Null;
                    Model.Search(SearchRequest, UserAudios, this);
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
                    Playlist.NextSong(this);
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
                    Playlist.PrevSong(this);
                });
            }
        }

        public RelayCommand SetAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    var selIndex = SelectedIndex;
                    if (SearchRequest != "" && State!=PlaylistState.Search)
                    {
                        Playlist = new Playlist(new SearchAudios(this, SearchAudios));
                    }
                    selIndex = SelectedIndex;
                    if (State != PlaylistState.Own && SearchRequest == "")
                    {
                        Playlist = new Playlist(new OwnAudios(this));
                    }

                    Playlist.SetAudioInfo(this, fromClick: true);
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
                    if (State != PlaylistState.IdAudios)
                    {
                        State = PlaylistState.IdAudios;
                        Playlist = new Playlist(new IdAudios());
                    }

                    Playlist.SetAudioInfo(this, fromClick: true);
                });
            }
        }

        public RelayCommand SetAlbumAudioFromClick
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (State == PlaylistState.Album)
                    {
                        Playlist.SetAudioInfo(this, fromClick: true);
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
                    if (State != PlaylistState.Save)
                        State = PlaylistState.Save;
                    Playlist = new Playlist(new SavesAudios(this));
                    Playlist.SetAudioInfo(this, fromClick: true);
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
                    webClient.DownloadFileAsync(new Uri(Player.Url),
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
            Helpers.SaveAudios.AddCache();
            Model.AddCacheToList(SaveAudiosList);
        }

        #endregion
    }
}