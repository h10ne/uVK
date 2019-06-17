using System.Windows.Controls;
using uVK.ViewModel;

namespace uVK.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            this.DataContext = new AuthVeiwModel();
        }
    }
}
