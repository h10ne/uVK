using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using uVK.Helpers;
using uVK.Model;
using uVK.Pages;
using uVK.View;

namespace uVK.ViewModel
{
    class MainViewModel:BaseViewModel
    {

        #region constructor
        public MainViewModel()
        {
            _settingsPage = new SettingsPage();
            _playerPage = new MusicPage();
            _messagePage = new MessagePage();
            CurrentPage = _playerPage;
            Username = UserDatas.Name + " " + UserDatas.Surname;
            UserPhoto = ApiDatas.Api.Users.Get(new[] { UserDatas.UserId }, VkNet.Enums.Filters.ProfileFields.Photo200)[0].Photo200.ToString();
        }
        #endregion

        #region private members
        private Visibility _btnCloseMenuVisibility = Visibility.Collapsed;
        private Visibility _btnOpenMenuVisibility = Visibility.Visible;
        private Page _currentPage;
        private readonly Page _messagePage;
        private readonly Page _settingsPage;
        private readonly Page _playerPage;
        private string _userPhoto;
        private double _fillOpacity;
        private Visibility _fillVisibility = Visibility.Hidden;
        private double _width = 45;
        #endregion

        #region public properties
        public Visibility BtnCloseMenuVisibility { get => _btnCloseMenuVisibility;
            set { _btnCloseMenuVisibility = value; OnPropertyChanged(nameof(BtnCloseMenuVisibility)); } }
        public Visibility BtnOpenMenuVisibility { get => _btnOpenMenuVisibility;
            set { _btnOpenMenuVisibility = value; OnPropertyChanged(nameof(BtnOpenMenuVisibility)); } }
        public Page CurrentPage { get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }}
        public string Username { get; set; }
        public string UserPhoto { get => _userPhoto;
            set { _userPhoto = value; OnPropertyChanged(nameof(UserPhoto)); } }
        public double FillOpacity { get => _fillOpacity;
            set { _fillOpacity = value; OnPropertyChanged(nameof(FillOpacity)); } }
        public Visibility FillVisibility { get => _fillVisibility;
            set { _fillVisibility = value;OnPropertyChanged(nameof(FillVisibility)); } }
        public double Width { get => _width;
            set { _width = value; OnPropertyChanged(nameof(Width)); } }
        #endregion

        #region commands
        public RelayCommand MessageClickCommand
        {
            get
            {
                return new RelayCommand((obj)=>
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
            get
            {
                return new RelayCommand((obj) =>
                {
                    SettingsModel.Logout();
                });
            }
        }
        #endregion

        #region Animation
        private async void GetBlackoutAnimation(double from, double to)
        {
            
            await Task.Factory.StartNew(() =>
            {      
                if (from<to)
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
                for (double i = Width; i >= 45; i-=5)
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
                for (double i = Width; i <= 255; i+=5)
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
