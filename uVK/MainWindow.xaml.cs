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
                Interval = new TimeSpan(0, 0, 0, 0, 400)
            };
            DurrationTimer.Stop();
            DurrationTimer.Tick += DurrationTimer_Tick;
            VolumeSlider.Maximum = 100;
            VolumeSlider.Value = 30;
            vkDatas.Audio = api.Audio.Get(new AudioGetParams { Count = api.Audio.GetCount(vkDatas.user_id) });
            playlist.SetAudioInfo(this);
            player.controls.stop();
        }

        private void DurrationTimer_Tick(object sender, EventArgs e)
        {

            LongTimeText.Text = player.currentMedia.durationString;
            DurrationSlider.Maximum = (int)player.currentMedia.duration;
            DurrationSlider.Value = (int)player.controls.currentPosition;
            PassedTimeText.Text = player.controls.currentPositionString;
            if (player.status == "Остановлено")
            {
                if (!VkBools.repeat)
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
            try
            {
                api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        return "0";
                    }
                });
            }
            catch
            {
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

        private void AuthLogPass(string login, string password)
        {
            api.Authorize(new ApiAuthParams
            {
                Login = login,
                Password = password,
                Settings = Settings.Offline
            });
            vkDatas.user_id = api.UserId.GetHashCode();
            File.WriteAllText("user_id.dat", vkDatas.user_id.ToString());
            File.WriteAllText("auth.dat", api.Token);
            Show();
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
                try
                {
                    Auth2Fact(login, password);
                    if (api.IsAuthorized)
                    {
                        rnd = new Random();
                        isAuth = true;
                    }
                }
                catch
                {
                    //MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK);
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
            if (!VkBools.isPlay)
            {
                player.controls.play();
                VkBools.isPlay = true;
            }
            else
            {
                player.controls.pause();
                VkBools.isPlay = false;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.settings.volume = (int) (sender as Slider).Value;
        }

        private void DurrationSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            player.controls.currentPosition = DurrationSlider.Value;
            DurrationTimer.Start();
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
        }

        private void BackAudioButton_Click(object sender, RoutedEventArgs e)
        {
            playlist.PrevSong(this);
        }

        private void RandomAudioButton_Click(object sender, RoutedEventArgs e)
        {
            if (VkBools.random)
                VkBools.random = false;
            else
                VkBools.random = true;
        }

        private void PovtorAudioButton_Click(object sender, RoutedEventArgs e)
        {
            if (VkBools.repeat)
                VkBools.repeat = false;
            else
                VkBools.repeat = true;
        }

        private void ExitVK_Click(object sender, RoutedEventArgs e)
        {
            
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
        }

        private void MaxVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            VolumeSlider.Value = 100;
        }

        private void MinVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            VolumeSlider.Value = 0;
        }
    }
}
