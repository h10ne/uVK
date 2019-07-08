using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            CurrentPage = _playerPage;
            Username = UserDatas.Name + " " + UserDatas.Surname;
        }
        #endregion

        #region private members
        private Visibility _btnCloseMenuVisibility = Visibility.Collapsed;
        private Visibility _btnOpenMenuVisibility = Visibility.Visible;
        private Page _currentPage;
        private Page _settingsPage { get; set; }
        private Page _playerPage { get; set; }
        #endregion

        #region public properties
        public Visibility BtnCloseMenuVisibility { get { return _btnCloseMenuVisibility; } set { _btnCloseMenuVisibility = value; OnPropertyChanged(nameof(BtnCloseMenuVisibility)); } }
        public Visibility BtnOpenMenuVisibility { get { return _btnOpenMenuVisibility; } set { _btnOpenMenuVisibility = value; OnPropertyChanged(nameof(BtnOpenMenuVisibility)); } }
        public Page CurrentPage { get { return _currentPage; } set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }}
        public string Username { get; set; }
        #endregion

        #region commands
        //Команда октрытия меню
        public RelayCommand CloseMenuCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    BtnCloseMenuVisibility = Visibility.Collapsed;
                    BtnOpenMenuVisibility = Visibility.Visible;
                });
            }
        }

        // Команда скрытия меню
        public RelayCommand OpenMenuCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    BtnCloseMenuVisibility = Visibility.Visible;
                    BtnOpenMenuVisibility = Visibility.Collapsed;
                });
            }
        }
        #endregion
    }
}
