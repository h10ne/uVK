using System.Collections.Generic;
using System.Windows.Controls;

namespace uVK.Styles.AudioStyles
{
    /// <summary>
    /// Логика взаимодействия для PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        public List<VkNet.Model.Attachments.Audio> Audios = new List<VkNet.Model.Attachments.Audio>();

        public PlayList()
        {
            InitializeComponent();
        }
    }
}