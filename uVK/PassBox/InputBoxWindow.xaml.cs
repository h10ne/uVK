using System.IO;
using System.Windows;
using System.Windows.Input;

namespace uVK.PassBox
{
    /// <summary>
    /// Логика взаимодействия для InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow
    {
        public InputBoxWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("someFile.tempdat", TbCode.Text);
            Close();
        }
    }
}