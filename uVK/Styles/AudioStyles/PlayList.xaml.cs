using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using uVK;
using uVK.Helpers;
using uVK.Model;

namespace uVK.Styles.AudioStyles
{
    /// <summary>
    /// Логика взаимодействия для PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        public List<VkNet.Model.Attachments.Audio> Audios = new List<VkNet.Model.Attachments.Audio>();
        public PlayList(VkNet.Model.Attachments.AudioPlaylist playlist)
        {
            InitializeComponent();
            //Инициализация обложки
            Uri uriImageSource = new Uri(@"https://pp.userapi.com/c848524/v848524102/1d1b12/a2hsTiaW8RU.jpg");
            if (playlist.Cover!=null)
                uriImageSource = new Uri(playlist.Cover.Photo135, UriKind.RelativeOrAbsolute);
            else
            {
                uriImageSource = new Uri(playlist.Covers.ToList()[0].Photo135, UriKind.RelativeOrAbsolute);
            }
            PlayListImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
            //Инициализация песен
            Audios = ApiDatas.api.Audio.Get(new VkNet.Model.RequestParams.AudioGetParams
            {
                PlaylistId = playlist.Id
            }).ToList();
            Title.Text = playlist.Title;
            try
            {
                Author.Text = playlist.MainArtists.ToList()[0].Name;
            }
            catch
            {
                Author.Text = $"{UserDatas.Name} {UserDatas.Surname}";
            }
        }
    }
}
