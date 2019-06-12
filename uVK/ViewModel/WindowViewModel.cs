using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using uVK.Model;
using uVK.View;

namespace uVK
{
    public class WindowViewModel : BaseViewModel
    {
        #region Private window Member
        static private Window mWindow;
        static private int mOuterMarginSize = 5;
        static private int mWindowRadius = 5;
        static private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;
        #endregion

        #region Window public  Properties

        public double WindowMinimumWidth { get; set; } = 500;

        public double WindowMinimumHeight { get; set; } = 135;

        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        public int ResizeBorder { get { return Borderless ? 0 : 3; } }

        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        public int OuterMarginSize
        {
            get
            {
                return Borderless ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        public int WindowRadius
        {
            get
            {
                return Borderless ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }


        public int TitleHeight { get; set; } = 30;

        /// <summary>
        /// True if we should have a dimmed overlay on the window
        /// such as when a popup is visible or the window is not focused
        /// </summary>
        public bool DimmableOverlayVisible { get; set; }

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand SettingCommand { get; set; }

        #endregion

        #region Constructor

        public WindowViewModel(Window window)
        {
            mWindow = window;

            CurrentPage = AuthPage;

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin"))
            {
                AuthModel.GetAuth();
                SettingsPage = new SettingsView();
                PlayerPage = new PlayerView();
                CurrentPage = PlayerPage;
            }

            mWindow.StateChanged += (sender, e) =>
            {
                WindowResized();
            };

            MinimizeCommand = new RelayCommand((obj) => mWindow.WindowState = WindowState.Minimized);
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
            }, (obj) => PlayerPage != null && CurrentPage==PlayerPage);
            CloseCommand = new RelayCommand((obj) => mWindow.Close());

            SettingCommand = new RelayCommand((obj) =>
            {
                if (CurrentPage == SettingsPage)
                    CurrentPage = PlayerPage;
                else
                    CurrentPage = SettingsPage;

            },(obj) => SettingsPage != null);

            var resizer = new WindowResizer(mWindow);

            resizer.WindowDockChanged += (dock) =>
            {
                mDockPosition = dock;

                WindowResized();
            };
        }

        #endregion

        #region Private Helpers

        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(mWindow);

            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }

        private void WindowResized()
        {
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }


        #endregion
               
        private Page _currentPage;
        public Page AuthPage = new AuthView();
        public Page SettingsPage;
        public Page PlayerPage;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }
        }
    }
}
