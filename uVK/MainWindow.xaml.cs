using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;
using WMPLib;
using VkNet.Model;
using VkNet.Enums.Filters;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.Windows.Threading;

namespace uVK
{
    public partial class MainWindow : Window
    {
        public WMPLib.WindowsMediaPlayer player;
        public DispatcherTimer DurrationTimer;
        public string Token = null;
        public IVkApi api;
        public Random rnd;
        Playlist playlist;
        public Switches VkBools;
        public VkDatas vkDatas;
        public bool isAuth = false;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel(this);
            VkBools = new Switches();
            vkDatas = new VkDatas();
            playlist = new Playlist(new OwnAudios());
            if (File.Exists("auth.dat"))
            {
                gridLogin.Visibility = Visibility.Hidden;
                Token = File.ReadAllText("auth.dat");
                GetAuth();
                gridLogin.Visibility = Visibility.Hidden;
                DoAfterLogin();
            }            
        }

        private void DoAfterLogin()
        {
            player = new WindowsMediaPlayer();
            DurrationTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 300)
            };
            DurrationTimer.Stop();
            DurrationTimer.Tick += DurrationTimer_Tick;
            VolumeSlider.Maximum = 100;
            VolumeSlider.Value = 30;
            vkDatas.Audio = api.Audio.Get(new AudioGetParams { Count = api.Audio.GetCount(vkDatas.user_id) });
            playlist.SetAudioInfo(this);
            player.controls.stop();
            DownloadFriendList();
            
        }

        private void DownloadFriendList()
        {
            var friends = api.Friends.Get(new FriendsGetParams
            {
                Fields = ProfileFields.All,
                //Order = Order.Name
            });
            foreach (var friend in friends)
                this.FrendList.Items.Add($"{friend.FirstName} {friend.LastName}");
        }

        private void SwitchStatesOff()
        {
            VkBools.IsHot = false;
            VkBools.IsOwn = false;
            VkBools.IsRecommend = false;
            VkBools.IsSearch = false;
        }

        private void SetState(string state)
        {
            state = state.ToUpper();
            switch (state)
            {
                case "OWN":
                    SwitchStatesOff();
                    MusicList.Items.Clear();
                    VkBools.IsOwn = true;
                    playlist = new Playlist(new OwnAudios());
                    AddAudioToList(vkDatas.Audio);
                    MusicList.SelectedIndex = vkDatas._offset;
                    break;
                case "HOT":
                    SwitchStatesOff();
                    MusicList.Items.Clear();
                    VkBools.IsHot = true;
                    playlist = new Playlist(new HotAudio());
                    vkDatas.HotAudios = api.Audio.GetPopular(false, null, 35, null);
                    foreach (var audio in vkDatas.HotAudios)
                        MusicList.Items.Add($"{audio.Artist} - {audio.Title}");
                    break;
                case "SEARCH":
                    SwitchStatesOff();
                    MusicList.Items.Clear();
                    VkBools.IsSearch = true;
                    playlist = new Playlist(new SearchAudios());
                    MusicList.Items.Clear();
                    try
                    {
                        vkDatas.SearchAudios = api.Audio.Search(new AudioSearchParams
                        {
                            Query = MusicSearch.Text,
                            Autocomplete = true,
                            SearchOwn = true,
                            Count = 20,
                            PerformerOnly = false
                        });
                        AddAudioToList(vkDatas.SearchAudios);
                    }
                    catch { }
                    break;
                case "RECOM":
                    SwitchStatesOff();
                    MusicList.Items.Clear();
                    VkBools.IsRecommend = true;
                    playlist = new Playlist(new RecommendedAudio());
                    vkDatas.RecommendedAudio = api.Audio.GetRecommendations(null, null, 50, null, true);
                    AddAudioToList(vkDatas.RecommendedAudio);
                    break;               

            }
        }

        private void DurrationTimer_Tick(object sender, EventArgs e)
        {

            LongTimeText.Text = player.currentMedia.durationString;
            DurrationSlider.Maximum = (int)player.currentMedia.duration;
            DurrationSlider.Value = (int)player.controls.currentPosition;
            PassedTimeText.Text = player.controls.currentPositionString;
            if (player.status == "Остановлено")
            {
                if (RepeatAudioButton.IsChecked.Value)
                    playlist.NextSong(this);
                player.controls.play();
            }
        }

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

        private void AuthToken()
        {
            api.Authorize(new ApiAuthParams
            {
                AccessToken = Token,
                Settings = Settings.Offline
            });
            vkDatas.user_id = long.Parse(File.ReadAllText("user_id.dat"));
        }

        private void Auth2Fact(string login, string password)
        {
            string trueCode;
            bool needCode = false;
            try
            {
                api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        needCode = true;
                        return "0";
                    }
                });
            }
            catch
            {
                if (!needCode)
                    return;
                var input = new InputBoxWindow();
                input.ShowDialog();
                trueCode = File.ReadAllText("someFile.tempdat");

                api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        string code = File.ReadAllText("someFile.tempdat");                        
                        return code;
                    }
                });
                System.IO.File.Delete("someFile.tempdat");
            }

        }

        public void GetAuth(string login = null, string password = null)
        {

            vkDatas.service = new ServiceCollection();
            vkDatas.service.AddAudioBypass();
            api = new VkApi(vkDatas.service);
            if (Token != null)
            {
                AuthToken();
            }
            else
            {
                Auth2Fact(login, password);
                if (api.IsAuthorized)
                {
                    rnd = new Random();
                    isAuth = true;
                }
            }
        }


        public void AddAudioToList(VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> audios)
        {
            MusicList.Items.Clear();
            foreach (var audio in audios)
            {
                Object title = new object();
                title = $"{audio.Artist} - {audio.Title}";
                MusicList.Items.Add(title);

            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (PauseButton.IsChecked.Value)
            {
                player.controls.play();
            }
            else
            {
                player.controls.pause();
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.settings.volume = (int) (sender as Slider).Value;
        }

        private void VolumeSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && player.settings.volume + 2 <= 100)
            {
                VolumeSlider.Value += 2;
            }
            else if (e.Delta < 0 && player.settings.volume - 2 >= 0)
            {
                VolumeSlider.Value -= 2;
            }
        }

        private void NextAudioButton_Click(object sender, RoutedEventArgs e)
        {
            playlist.NextSong(this);
            PauseButton.IsChecked = true;
        }

        private void BackAudioButton_Click(object sender, RoutedEventArgs e)
        {
            playlist.PrevSong(this);
            PauseButton.IsChecked = true;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetAuth(tbLogin.Text, tbPassword.Password);
            }
            catch
            { }
            if (isAuth == true)
            {
                File.WriteAllText("user_id.dat", api.UserId.Value.ToString());
                File.WriteAllText("auth.dat", api.Token);
                vkDatas.user_id = api.UserId.GetHashCode();
                Token = api.Token;
                vkDatas.user_id = api.UserId.Value;
                gridLogin.Visibility = Visibility.Hidden;
                DoAfterLogin();
            }
            else
            {
                Error.Text = "Неправильный логин или пароль";
            }
        }

        private void MusicList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            playlist.SetAudioInfo(this);
            PauseButton.IsChecked = true;
        }

        private void MaxVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            VolumeSlider.Value = 100;
        }

        private void MinVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            VolumeSlider.Value = 0;
        }

        private void MusicSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetState("search");
            }
        }

        private void MusicSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(MusicSearch.Text))
            {
                SetState("own");
            }
        }
    }
}
