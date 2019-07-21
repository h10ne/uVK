﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace uVK.ViewModel
{
    public class FriendsMusicViewModel : ReactiveObject
    {
        [Reactive] public string UserName { get; set; }
        [Reactive] public string ImageSourse { get; set; }
        [Reactive] public string CountAudio { get; set; }
        public long Id;
    }
}
