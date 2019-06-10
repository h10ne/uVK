using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using uVK.Model;
using VkNet.Model.RequestParams;

namespace uVK.ViewModel
{
    public class PlayerViewModel:BaseViewModel
    {
        public PlayerViewModel()
        {
            PlayerModel.Audio = ApiDatas.api.Audio.Get(new AudioGetParams { Count = ApiDatas.api.Audio.GetCount(UserDatas.User_id) });
            MusicList = new ListBox();
            PlayerModel.AddAudioToList(PlayerModel.Audio, MusicList);
            PlayerModel.Playlist = new Playlist(new OwnAudios());
            PlayerModel.Playlist.SetAudioInfo(this);
        }

        #region Private members
        private ListBox _musicList;
        private int _volume;
        private bool _isPlay = false;
        private bool _random = false;
        private bool _repeat;
        private string _title;
        private string _artist;
        #endregion

        public  string Title { get { return _title; } set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public  string Artist { get { return _artist; } set { _artist = value; OnPropertyChanged(nameof(Artist)); } }
        public ListBox MusicList { get { return _musicList; } set { _musicList = value; OnPropertyChanged(nameof(MusicList)); } }
        public int Volume { get { return _volume; } set { _volume = value; OnPropertyChanged(nameof(Volume)); } }
        public bool IsPlay { get { return _isPlay; } set { _isPlay = value; OnPropertyChanged(nameof(IsPlay)); } }
        public bool Random { get { return _random; } set { _random = value; OnPropertyChanged(nameof(Random)); } }
        public bool Repeat { get { return _repeat; } set { _repeat = value; OnPropertyChanged(nameof(Repeat)); } }
        #region Commands

        public RelayCommand PlayPauseCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (IsPlay)
                        IsPlay = false;
                    else
                        IsPlay = true;
                });
            }
        }
        public RelayCommand MuteCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    Volume = 0;
                }, (obj)=> Volume>0);
            }
        }
        public RelayCommand FullLoudCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    Volume = 100;
                }, (obj) => Volume < 100);
            }
        }
        #endregion

    }
}
