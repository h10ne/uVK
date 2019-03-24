using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Timers;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;
using WMPLib;
using VkNet.Model;
using VkNet.AudioBypassService;
using VkNet.Enums.Filters;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;

namespace uVK
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WMPLib.WindowsMediaPlayer player;
        public Timer DurrationTimer;
        public string Token = null;
        public IVkApi api;
        public string code = null;
        public Random rnd;
        private int clr;
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
                Token = File.ReadAllText("auth.dat");
                GetAuth();
                gridMain.Visibility = Visibility.Visible;
                gridLogin.Visibility = Visibility.Hidden;
            }
            player = new WindowsMediaPlayer();
            DurrationTimer = new Timer(700);
            DurrationTimer.Stop();
            DurrationTimer.Elapsed += DurrationTimer_Tick;
            vkDatas.Audio = api.Audio.Get(new AudioGetParams { Count = api.Audio.GetCount(vkDatas.user_id) });
            playlist.SetAudioInfo(this);
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
            api.Authorize(new ApiAuthParams
            {
                Login = login,
                Password = password,
                Settings = Settings.Offline,
                TwoFactorAuthorization = () =>
                {
                    while (code == null)
                    {
                        code = File.ReadAllText("someFile.tempdat");
                    }
                    System.IO.File.Delete("someFile.tempdat");
                    return code;
                }
            });
            vkDatas.user_id = api.UserId.GetHashCode();
            File.WriteAllText("user_id.dat", vkDatas.user_id.ToString());
            File.WriteAllText("auth.dat", api.Token);
            Show();
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
                    if (!api.IsAuthorized)
                        AuthLogPass(login, password);
                }
                catch
                {
                    //MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK);
                }

            }
            rnd = new Random();
            isAuth = true;
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


        private void DurrationTimer_Tick(object sender, ElapsedEventArgs e)
        {
            //AllTimeDur.Text = player.currentMedia.durationString;
            DurrationSlider.Maximum = (int)player.currentMedia.duration;
            DurrationSlider.Value = (int)player.controls.currentPosition;
           // currentTimeDur.Text = player.controls.currentPositionString;
        }
    }
}
