using System.Windows.Controls;

namespace uVK.Styles.AudioStyles
{
    /// <summary>
    /// Логика взаимодействия для FriendsMusic.xaml
    /// </summary>
    public partial class FriendsMusic : UserControl
    {
        public long Id;

        public FriendsMusic()
        {
            InitializeComponent();
            //Id = user.Id;
            //Username.Text = $"{user.FirstName} {user.LastName}";
            //CountAudio.Text = $"{ApiDatas.api.Audio.GetCount(Id)} аудиозаписей";
            //UserPhoto.ImageSource = new BitmapImage(user.Photo100);
        }
    }
}