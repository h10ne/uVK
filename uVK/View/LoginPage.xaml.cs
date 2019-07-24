using uVK.ViewModel;

namespace uVK.View
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new AuthVeiwModel();
        }
    }
}