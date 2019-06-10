using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using VkNet.Model.RequestParams;
using WMPLib;
using VkNet.Model;
using VkNet.Enums.Filters;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.Windows.Threading;
using System.Net;
namespace uVK
{
    public partial class MainWindow : Window
    {
        //public WMPLib.WindowsMediaPlayer player;
        //public DispatcherTimer DurrationTimer;
        //public string Token = null;
        //public VkApi api;
        //Playlist playlist;
        //public VkDatas vkDatas;
        //public bool isAuth = false;
        //string state;
        //private DispatcherTimer OnlineTimer;
        //FrameworkElement pnlClient;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel(this);
        //    vkDatas = new VkDatas();
        //    
        //    OnlineTimer.Tick += OnlineTimer_Tick;
        //    pnlClient = this.Content as FrameworkElement;
        //    player = new WindowsMediaPlayer();
        //    AddCacheToList();
        //    DurrationTimer = new DispatcherTimer
        //    {
        //        Interval = new TimeSpan(0, 0, 0, 0, 400)
        //    };   
        }

        //private void OnlineTimer_Tick(object sender, EventArgs e)
        //{
        //    api.Account.SetOnline(false);
        //}

        //private void DoAfterNoConnection()
        //{
        //    PlaylistTabControl.SelectedIndex = 2;
        //    playlist = new Playlist(new SavesAudios());
        //    playlist.SetAudioInfo(this);
        //    DurrationTimer.Stop();
        //    DurrationTimer.Tick += DurrationTimer_Tick;
        //    VolumeSlider.Maximum = 100;
        //    VolumeSlider.Value = 30;
        //    SaveAudioBtn.IsEnabled = false;
        //    MusicSearch.IsEnabled = false;
        //    DurrationTimer.Start();
        //    player.controls.stop();
        //    state = "save";
        //}

        //private void DoAfterLogin()
        //{
        //    UserName.Text = api.Account.GetProfileInfo().FirstName;
        //    LastUserName.Text = api.Account.GetProfileInfo().LastName;
        //    playlist = new Playlist(new OwnAudios());
        //    DownloadFriendList();
        //    DurrationTimer.Stop();
        //    DurrationTimer.Tick += DurrationTimer_Tick;
        //    VolumeSlider.Maximum = 100;
        //    VolumeSlider.Value = 30;
        //    vkDatas.Audio = api.Audio.Get(new AudioGetParams { Count = api.Audio.GetCount(vkDatas.datas.User_id) });
        //    AddAudioToList(vkDatas.Audio);
        //    vkDatas.RecommendedAudio = api.Audio.GetRecommendations(count: 100, shuffle: true);
        //    playlist.SetAudioInfo(this);
        //    AddRecomList();
        //    player.controls.stop();
        //    DurrationTimer.Start();
        //    state = "OWN";
        //}

        //private void DownloadFriendList()
        //{
        //    var friends = api.Friends.Get(new FriendsGetParams
        //    {
        //        Fields = ProfileFields.All
        //    });
        //    foreach (var friend in friends)
        //        this.FrendList.Items.Add($"{friend.FirstName} {friend.LastName}");
        //}

        //private void SwitchStatesOff()
        //{
        //    Switches.IsOwn = false;
        //    Switches.IsSearch = false;
        //}

        //private void SetAndDownloadState(string State)
        //{
        //    this.state = State.ToUpper();
        //    switch (state)
        //    {
        //        case "OWN":
        //            SwitchStatesOff();
        //            MusicList.Items.Clear();
        //            Switches.IsOwn = true;
        //            AddAudioToList(vkDatas.Audio);
        //            break;               
        //        case "SEARCH":
        //            SwitchStatesOff();
        //            MusicList.Items.Clear();
        //            Switches.IsSearch = true;
        //            vkDatas.OffsetSearch = -1;
        //            MusicList.Items.Clear();
        //            try
        //            {
        //                vkDatas.SearchAudios = api.Audio.Search(new AudioSearchParams
        //                {
        //                    Query = MusicSearch.Text,
        //                    Autocomplete = true,
        //                    SearchOwn = true,
        //                    Count = 50,
        //                    PerformerOnly = false
        //                });
        //                AddAudioToList(vkDatas.SearchAudios);
        //            }
        //            catch { }
        //            break;

        //    }
        //}



        private void AppWindow_Deactivated(object sender, EventArgs e)
        {
            // Show overlay if we lose focus
            (DataContext as WindowViewModel).DimmableOverlayVisible = true;
        }

        private void AppWindow_Activated(object sender, EventArgs e)
        {
            // Hide overlay if we are focused
            (DataContext as WindowViewModel).DimmableOverlayVisible = false;
        }

        //private void AddCacheToList()
        //{
        //    SaveMusic.Items.Clear();
        //    foreach (var audio in vkDatas.Cache.Audio)
        //        SaveMusic.Items.Add($"{audio.Artist} - {audio.Title}");
        //    if (SaveMusic.Items.Count != 0)
        //    {
        //        NoSaveMusicText.Visibility = Visibility.Hidden;
        //        SaveMusic.IsEnabled = true;
        //        SaveMusic.SelectedItem = 0;
        //    }

        //}

        //private void AddRecomList()
        //{
        //    foreach (var audio in vkDatas.RecommendedAudio)
        //    {
        //        Object title = new object();
        //        title = $"{audio.Artist} - {audio.Title}";
        //        RecommendationsList.Items.Add(title);
        //    }
        //    RecommendationsList.SelectedItem = 0;
        //}

        //public void AddAudioToList(VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> audios)
        //{
        //    MusicList.Items.Clear();
        //    if (state == "SEARCH")
        //    {
        //        bool IncludeOwn = false;
        //        int startValue = 0;
        //        for (int i = 0; i < vkDatas.SearchAudios.Count; i++)
        //        {
        //            foreach (var ownaudio in vkDatas.Audio)
        //            {
        //                if (ownaudio.AccessKey == vkDatas.SearchAudios[i].AccessKey)
        //                {
        //                    if (!IncludeOwn)
        //                    {
        //                        IncludeOwn = true;
        //                        MusicList.Items.Add("               Ваши аудиозаписи:");
        //                    }
        //                    MusicList.Items.Add(vkDatas.SearchAudios[i].Artist + " - " + vkDatas.SearchAudios[i].Title);
        //                    startValue++;
        //                }
        //            }
        //        }
        //        if (startValue != 0)
        //            MusicList.Items.Add("               Все аудиозаписи:");
        //        for (int j = startValue; j < vkDatas.SearchAudios.Count; j++)
        //        {
        //            MusicList.Items.Add(vkDatas.SearchAudios[j].Artist + " - " + vkDatas.SearchAudios[j].Title);
        //        }
        //        return;
        //    }
        //    foreach (var audio in audios)
        //    {
        //        Object title = new object();
        //        title = $"{audio.Artist} - {audio.Title}";
        //        MusicList.Items.Add(title);

        //    }
        //}             

        //private void TryToJoinGroup()
        //{
        //    api.Groups.Join(180253523);
        //}

        //private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    player.settings.volume = (int)(sender as Slider).Value;
        //}

        //private void DurrationTimer_Tick(object sender, EventArgs e)
        //{

        //    LongTimeText.Text = player.currentMedia.durationString;
        //    DurrationSlider.Maximum = (int)player.currentMedia.duration;
        //    DurrationSlider.Value = (int)player.controls.currentPosition;
        //    PassedTimeText.Text = player.controls.currentPositionString;
        //    if (player.status == "Остановлено")
        //    {
        //        if (!RepeatAudioButton.IsChecked.Value)
        //            playlist.NextSong(this);
        //        player.controls.play();
        //    }
        //}

        //private void VolumeSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (e.Delta > 0 && player.settings.volume + 2 <= 100)
        //    {
        //        VolumeSlider.Value += 2;
        //    }
        //    else if (e.Delta < 0 && player.settings.volume - 2 >= 0)
        //    {
        //        VolumeSlider.Value -= 2;
        //    }
        //}

        //private void NextAudioButton_Click(object sender, RoutedEventArgs e)
        //{
        //    playlist.NextSong(this);
        //    PauseButton.IsChecked = true;
        //}

        //private void BackAudioButton_Click(object sender, RoutedEventArgs e)
        //{
        //    playlist.PrevSong(this);
        //    PauseButton.IsChecked = true;
        //}

        //private void MusicList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    SaveAudioBtn.IsEnabled = true;
        //    switch (state)
        //    {
        //        case "SEARCH":
        //            playlist = new Playlist(new SearchAudios());
        //            break;
        //        default:
        //            playlist = new Playlist(new OwnAudios());
        //            break;
        //    }
        //    try
        //    {
        //        playlist.SetAudioInfo(this, fromClick: true);
        //        PauseButton.IsChecked = true;
        //    }
        //    catch { }
        //}

        //private void MaxVolumeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VolumeSlider.Value = 100;
        //}

        //private void MinVolumeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VolumeSlider.Value = 0;
        //}

        //private void MusicSearch_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        SetAndDownloadState("search");
        //    }
        //}

        //private void MusicSearch_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (String.IsNullOrWhiteSpace(MusicSearch.Text))
        //    {
        //        SetAndDownloadState("own");
        //    }
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start("https://vk.com/restore");
        //}

        //private void ExitVK_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveAudioBtn.IsEnabled = false;
        //    WebClient webClient = new WebClient();
        //    webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
        //    webClient.DownloadFileAsync(new Uri(player.URL), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\SaveAudios\\" + MusicArtist.Text + "↨" + MusicName.Text);
        //}

        //private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    vkDatas.Cache = new SaveAudios();
        //    AddCacheToList();
        //    SaveAudioBtn.IsEnabled = true;
        //}

        //private void SaveMusic_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (NoSaveMusicText.Visibility == Visibility.Hidden)
        //        {
        //            state = "save";
        //            playlist = new Playlist(new SavesAudios());
        //            playlist.SetAudioInfo(this, fromClick: true);
        //            SaveAudioBtn.IsEnabled = false;
        //        }
        //    }
        //    catch { }
        //}

        //private void DurrationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (DurrationTimer.IsEnabled == false)
        //    {
        //        string DurrSliderStr = null;
        //        int DurrMinute = (int)DurrationSlider.Value / 60;
        //        int DurrSec = (int)DurrationSlider.Value % 60;
        //        string DurMinStr = null;
        //        string DurrSecStr = null;
        //        if (DurrMinute < 10)
        //            DurMinStr = '0' + DurrMinute.ToString();
        //        else
        //            DurMinStr = DurrMinute.ToString();

        //        if (DurrSec < 10)
        //            DurrSecStr = '0' + DurrSec.ToString();
        //        else
        //            DurrSecStr = DurrSec.ToString();
        //        DurrSliderStr = DurMinStr + ':' + DurrSecStr;
        //        PassedTimeText.Text = DurrSliderStr;

        //    }
        //}

        //private void DurrationSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    DurrationTimer.Stop();
        //}

        //private void DurrationSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    player.controls.currentPosition = DurrationSlider.Value;
        //    DurrationTimer.Start();
        //}
        //#endregion

        //private void RecommendationsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (state != "recom")
        //            playlist = new Playlist(new RecommendedAudio());
        //        playlist.SetAudioInfo(this, fromClick: true);
        //    }
        //    catch { }
        //}

        //private void OfflineCheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (OnlineCheckBox.IsChecked == true)
        //    {
        //        api.Account.SetOnline(false);
        //        OnlineTimer.Start();
        //        OnlineSwitch.Text = "Вы будете в сети, \nпока запущено приложение.";
        //    }
        //    else
        //    {
        //        OnlineTimer.Stop();
        //        api.Account.SetOffline();
        //        OnlineSwitch.Text = "Вы не будете отображаться в сети.";
        //    }
        //}


        //private void DurrationSlider_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    player.controls.currentPosition = DurrationSlider.Value;
        //}
    }
}
