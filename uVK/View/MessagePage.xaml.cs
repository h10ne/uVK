using System.Windows.Controls;
using uVK.ViewModel;

namespace uVK.View
{
    /// <summary>
    /// Логика взаимодействия для MessagePage.xaml
    /// </summary>
    public partial class MessagePage : Page
    {
        public MessagePage()
        {
            InitializeComponent();
            DataContext = new MessageViewModel();
        }
    }
}