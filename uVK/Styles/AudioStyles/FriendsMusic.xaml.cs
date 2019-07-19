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
using uVK.Model;

namespace uVK.Styles.AudioStyles
{
    /// <summary>
    /// Логика взаимодействия для FriendsMusic.xaml
    /// </summary>
    public partial class FriendsMusic : UserControl
    {
        public long Id;
        public FriendsMusic(VkNet.Model.User user)
        {
            InitializeComponent();
            Id = user.Id;
            Username.Text = $"{user.FirstName} {user.LastName}";
            CountAudio.Text = $"{ApiDatas.api.Audio.GetCount(Id)} аудиозаписей";
            UserPhoto.ImageSource = new BitmapImage(user.Photo100);
        }
    }
}
