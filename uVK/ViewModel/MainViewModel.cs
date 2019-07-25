using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using uVK.Helpers;
using uVK.Model;
using uVK.View;

namespace uVK.ViewModel
{
    class MainViewModel : ReactiveObject
    {
        #region constructor

        public MainViewModel()
        {
            _settingsPage = new SettingsPage();
            Model.PlayerModel playerM = new PlayerModel();
            _playerPage = new MusicPage() {DataContext = new PlayerViewModel(playerM)};
            _messagePage = new MessagePage();
            CurrentPage = _playerPage;
            Username = UserDatas.Name + " " + UserDatas.Surname;
            UserPhoto = ApiDatas.Api.Users.Get(new[] {UserDatas.UserId}, VkNet.Enums.Filters.ProfileFields.Photo200)[0]
                .Photo200.ToString();
        }

        #endregion

        #region private members

        private readonly Page _messagePage;
        private readonly Page _settingsPage;
        private readonly Page _playerPage;

        #endregion

        #region public properties

        [Reactive] public Visibility BtnCloseMenuVisibility { get; set; } = Visibility.Collapsed;
        [Reactive] public Visibility BtnOpenMenuVisibility { get; set; } = Visibility.Visible;
        [Reactive] public Page CurrentPage { get; set; }
        [Reactive] public string Username { get; set; }
        [Reactive] public string UserPhoto { get; set; }
        [Reactive] public double FillOpacity { get; set; }
        [Reactive] public Visibility FillVisibility { get; set; } = Visibility.Hidden;
        [Reactive] public double Width { get; set; } = 45;

        #endregion

        #region commands

        public RelayCommand MessageClickCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    CurrentPage = _messagePage;
                    CloseMenu();
                });
            }
        }

        public RelayCommand MusicClickCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    CurrentPage = _playerPage;
                    CloseMenu();
                });
            }
        }

        public RelayCommand SettingsClickCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    CurrentPage = _settingsPage;
                    CloseMenu();
                });
            }
        }

        //Команда скрытия меню
        public RelayCommand CloseMenuCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    GetBlackoutAnimation(0.8, 0);
                    CloseMenu();
                });
            }
        }

        // Команда открытия меню
        public RelayCommand OpenMenuCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    GetBlackoutAnimation(0, 0.8);
                    GetOpenMenuAnimation();
                    BtnCloseMenuVisibility = Visibility.Visible;
                    BtnOpenMenuVisibility = Visibility.Collapsed;
                });
            }
        }

        public RelayCommand LogoutCommand
        {
            get { return new RelayCommand((obj) => { SettingsModel.Logout(); }); }
        }

        #endregion

        #region Animation

        private async void GetBlackoutAnimation(double from, double to)
        {
            await Task.Factory.StartNew(() =>
            {
                if (from < to)
                {
                    for (double i = from; i < to; i += 0.1)
                    {
                        FillOpacity = i;
                        Thread.Sleep(20);
                        FillVisibility = Visibility.Visible;
                    }

                    return;
                }

                for (double i = to; i > from; i -= 0.1)
                {
                    FillOpacity = i;
                    Thread.Sleep(15);
                }
            });
        }

        private async void GetCloseMenuAnimation()
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = Width; i >= 45; i -= 5)
                {
                    Width = i;
                    Thread.Sleep(4);
                }
            });
        }

        private async void GetOpenMenuAnimation()
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = Width; i <= 255; i += 5)
                {
                    Width = i;
                    Thread.Sleep(4);
                }
            });
        }

        #endregion

        #region Methods

        private void CloseMenu()
        {
            BtnCloseMenuVisibility = Visibility.Collapsed;
            BtnOpenMenuVisibility = Visibility.Visible;
            FillVisibility = Visibility.Hidden;
            GetCloseMenuAnimation();
        }

        #endregion
    }
}