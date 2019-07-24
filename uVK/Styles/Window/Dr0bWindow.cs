using System.Windows;

namespace uVK.Styles.Window
{
    public partial class WindowStyle : ResourceDictionary
    {
        public WindowStyle()
        {
            InitializeComponent();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var window = (System.Windows.Window) ((FrameworkElement) sender).TemplatedParent;
            window.Close();
        }

        private void MaximizeRestoreClick(object sender, RoutedEventArgs e)
        {
            var window = (System.Windows.Window) ((FrameworkElement) sender).TemplatedParent;
            window.WindowState = window.WindowState == System.Windows.WindowState.Normal
                ? System.Windows.WindowState.Maximized
                : System.Windows.WindowState.Normal;
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            var window = (System.Windows.Window) ((FrameworkElement) sender).TemplatedParent;
            window.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}