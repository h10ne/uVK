using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uVK.ViewModel;

namespace uVK.View
{
    /// <summary>
    /// Логика взаимодействия для PlayerView.xaml
    /// </summary>
    public partial class PlayerView : Page
    {
        public PlayerView()
        {
            InitializeComponent();
            DataContext = new PlayerViewModel();
        }

        private void MusicSearch_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
