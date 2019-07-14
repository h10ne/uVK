using System.Windows.Controls;
using uVK.ViewModel;

namespace uVK.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = new MessageViewModel();
        }
    }
}
