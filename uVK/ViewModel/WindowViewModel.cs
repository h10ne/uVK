using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using uVK.Helpers;
using uVK.Model;
using uVK.View;
using LoginPage = uVK.View.LoginPage;

namespace uVK.ViewModel
{
    public class WindowViewModel : ReactiveObject
    {
        #region Private window Member
        private static Window _mWindow;
        private Page _mainPage;
        #endregion

        #region Window public  Properties
        [Reactive] public double WindowMinimumWidth { get; set; } = 500;
        [Reactive] public double WindowMinimumHeight { get; set; } = 135;
        public Page AuthPage = new LoginPage();
        [Reactive]
        public Page CurrentPage { get; set; }

        [Reactive] public double Opacity { get; set; } = -1;
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
                _mainPage = new MainPage();
                CurrentPage = _mainPage;
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
            }, (obj) => CurrentPage == _mainPage);
            CloseCommand = new RelayCommand((obj) => _mWindow.Close());

        }

        #endregion

    }
}
