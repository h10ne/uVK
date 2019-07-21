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
        public PlayList()
        {
            InitializeComponent();
        }
    }
}
