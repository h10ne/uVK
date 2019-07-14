using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using uVK.Model;
using uVK.Pages;
using uVK.View;

namespace uVK
{
    public class WindowViewModel : BaseViewModel
    {
        #region Private window Member
        static private Window mWindow;
        #endregion

        #region Window public  Properties
        public double WindowMinimumWidth { get; set; } = 500;
        public double WindowMinimumHeight { get; set; } = 135;
        public Page AuthPage = new LoginPage();
        public Page MainPage { get; set; }
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }
        }
        public double Opacity { get { return _opacity; } set { _opacity = value; OnPropertyChanged(nameof(Opacity)); } }
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
                mWindow.WindowState = WindowState.Minimized;
            });
            MaximizeCommand = new RelayCommand((obj) =>
            {
                if (mWindow.Width == 900)
                {
                    mWindow.Width = 500;
                    mWindow.Height = 135;
                }
                else
                {
                    mWindow.Width = 900;
                    mWindow.Height = 550;
                }
            }, (obj) => CurrentPage == MainPage);
            CloseCommand = new RelayCommand((obj) => mWindow.Close());

        }

        #endregion

        private double _opacity = 1;
        private Page _currentPage;
        

        private async void Minimize()
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = 1; i > 0; i-=0.1)
                {
                    Opacity = i;
                    Thread.Sleep(50);
                }
            });
        }
        private async void Maxiize()
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = 0; i < 1; i += 0.1)
                {
                    Opacity = i;
                }
            });
        }
    }
}
