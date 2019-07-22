using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using uVK.Helpers;
using uVK.Model;
using uVK.View;
using LoginPage = uVK.View.LoginPage;

namespace uVK.ViewModel
{
    public class WindowViewModel : BaseViewModel
    {
        #region Private window Member
        private static Window _mWindow;
        #endregion

        #region Window public  Properties
        public double WindowMinimumWidth { get; set; } = 500;
        public double WindowMinimumHeight { get; set; } = 135;
        public Page AuthPage = new LoginPage();
        public Page MainPage { get; set; }
        public Page CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }
        }
        public double Opacity { get => _opacity;
            set { _opacity = value; OnPropertyChanged(nameof(Opacity)); } }
        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand SettingCommand { get; set; }

        #endregion

        #region Constructor

        public WindowViewModel()
        {
            CurrentPage = AuthPage;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin"))
            {
                AuthModel.GetAuth();
                MainPage = new MainPage();
                CurrentPage = MainPage;
            }
            MinimizeCommand = new RelayCommand((obj) => 
            {
                //Minimize();
                //Thread.Sleep(500);
                _mWindow.WindowState = WindowState.Minimized;
            });
            MaximizeCommand = new RelayCommand((obj) =>
            {
                if (_mWindow.Width == 900)
                {
                    _mWindow.Width = 500;
                    _mWindow.Height = 135;
                }
                else
                {
                    _mWindow.Width = 900;
                    _mWindow.Height = 550;
                }
            }, (obj) => CurrentPage == MainPage);
            CloseCommand = new RelayCommand((obj) => _mWindow.Close());

        }

        #endregion

        private double _opacity = 1;
        private Page _currentPage;
    }
}
