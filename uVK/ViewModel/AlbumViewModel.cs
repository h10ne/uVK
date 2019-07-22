using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uVK.ViewModel
{
    public class AlbumViewModel: ReactiveObject
    {
        [Reactive] public string Author { get; set; }
        [Reactive] public string Title { get; set; }
        [Reactive] public string ImageSource { get; set; }
        public List<VkNet.Model.Attachments.Audio> Audios = new List<VkNet.Model.Attachments.Audio>();
    }
}
