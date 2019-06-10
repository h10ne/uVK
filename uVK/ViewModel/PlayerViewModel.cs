using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using uVK.Model;

namespace uVK.ViewModel
{
    class PlayerViewModel:BaseViewModel
    {
        public PlayerViewModel()
        {
           // PlayerModel.Audio = ApiDatas.api.Audio.Get(new VkNet.Model.RequestParams.AudioGetParams { });
        }

        #region Private members
        private List<string> _musicList;
        private int _volume;
        private bool _isPlay = false;
        #endregion

        public List<string> MusicList { get { return _musicList; } set { _musicList = value; OnPropertyChanged(nameof(MusicList)); } }
        public int Volume { get { return _volume; } set { _volume = value; OnPropertyChanged(nameof(Volume)); } }
        public bool IsPlay { get { return _isPlay; } set { _isPlay = value; OnPropertyChanged(nameof(IsPlay)); } }

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
