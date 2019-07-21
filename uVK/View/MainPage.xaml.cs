using System.Windows.Controls;
using uVK.ViewModel;

namespace uVK.View
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        //private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonCloseMenu.Visibility = Visibility.Visible;
        //    ButtonOpenMenu.Visibility = Visibility.Collapsed;
        //}

        //private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonCloseMenu.Visibility = Visibility.Collapsed;
        //    ButtonOpenMenu.Visibility = Visibility.Visible;
        //}
    }
}
