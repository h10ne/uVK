using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace uVK.ViewModel
{
    public class OneAudioViewModel: ReactiveObject
    {
        [Reactive] public string Artist { get; set; }
        [Reactive] public string Title { get; set; }
        [Reactive] public string ImageSourseString { get; set; }
        [Reactive] public string Duration { get; set; }
        [Reactive] public double Width { get; set; }
        public string Url;
        public int Durration;
        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }
    }
}
