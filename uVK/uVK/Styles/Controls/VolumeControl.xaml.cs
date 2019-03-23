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

namespace uVK.Styles.Controls
{
    /// <summary>
    /// Логика взаимодействия для VolumeControl.xaml
    /// </summary>
    public partial class VolumeControl : UserControl
    {
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(VolumeControl), new PropertyMetadata(default(double)));

        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public static readonly DependencyProperty IsMuteProperty =
            DependencyProperty.Register("IsMute", typeof(bool), typeof(VolumeControl), new PropertyMetadata(default(bool)));

        public bool IsMute
        {
            get { return (bool)GetValue(IsMuteProperty); }
            set { SetValue(IsMuteProperty, value); }
        }


        public VolumeControl()
        {
            InitializeComponent();
        }

        private void VolumeButton_OnClick(object sender, RoutedEventArgs e)
        {
            Volume = 100;
        }

        private void VolumeControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //if (!VolumePopup.IsOpen)
            //    VolumePopup.IsOpen = true;

            if (e.Delta < 0)
            {
                Volume -= 5;
                if (Volume < 0)
                    Volume = 0;
            }
            else
            {
                Volume += 5;
                if (Volume > 100)
                    Volume = 100;
            }
        }

        private void MuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            Volume = 0;
        }
    }
}
