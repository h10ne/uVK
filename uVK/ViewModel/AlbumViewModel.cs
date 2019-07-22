using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

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
