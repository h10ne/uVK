using System.Windows.Controls;
using uVK.ViewModel;

namespace uVK.View
{
    /// <summary>
    /// Логика взаимодействия для MusicPage.xaml
    /// </summary>
    public partial class MusicPage : Page
    {
        public MusicPage()
        {
            InitializeComponent();
            DataContext = new PlayerViewModel();
        }
    }
}