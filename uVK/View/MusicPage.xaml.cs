using System.Windows.Controls;
using uVK.ViewModel;

namespace uVK.Pages
{
    /// <summary>
    /// Логика взаимодействия для MusicPage.xaml
    /// </summary>
    public partial class MusicPage : Page
    {
        public MusicPage()
        {
            InitializeComponent();
            this.DataContext = new PlayerViewModel();
        }

        private void ListView_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            
        }
    }
}
