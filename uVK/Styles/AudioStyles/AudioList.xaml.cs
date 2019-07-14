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

namespace uVK.Styles.AudioStyles
{
    /// <summary>
    /// Логика взаимодействия для AudioList.xaml
    /// </summary>
    public partial class AudioList : UserControl
    {
        public AudioList(VkNet.Model.Attachments.Audio audio)
        {
            InitializeComponent();
            Title.Text = audio.Title;
            Artist.Text = audio.Artist;


            try
            {
                Uri uriImageSource = new Uri(audio.Album.Cover.Photo135);
                MusicImage.ImageSource = new BitmapImage(uriImageSource);
            }
            catch
            {
                //MusicImage.ImageSource = new BitmapImage(new Uri(@"/Images/ImageMusic.png"));
            }


        }
    }
}
