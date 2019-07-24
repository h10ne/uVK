using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using uVK.Helpers;
using uVK.Model;

namespace uVK.ViewModel
{
    class SettingsViewModel:ReactiveObject
    {
        #region variables

        private bool _isDownloadiong;

        [Reactive] public int DownloadValue { get; set; } = 0;
        [Reactive] public int MaxDownloadValue { get; set; } = 1;
        [Reactive] public string SaveAudiosText { get; set; } = "Сохранить все аудиозаписи";
        [Reactive] public bool CheckGroupAdmin { get; set; }
        [Reactive] public bool CheckGroupWallClear { get; set; }
        private string _groupAFKDays;
        public string GroupAFKDays
        {
            get => _groupAFKDays;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.RaiseAndSetIfChanged(ref _groupAFKDays, value);
                    return;
                }
                if (char.IsDigit(value[value.Length-1]))
                {
                    this.RaiseAndSetIfChanged(ref _groupAFKDays, value);
                    return;
                }
                this.RaiseAndSetIfChanged(ref _groupAFKDays, _groupAFKDays);
            }
        }

        #endregion
        public SettingsViewModel()
        {       
        }
        public RelayCommand Logout
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    SettingsModel.Logout();
                });
            }
        }

        public RelayCommand SaveAllAudioAsyncCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    //_isDownloadiong = true;

                    SettingsModel.SaveAllAudioAsync(SaveAudiosText);
                    

                }, (obj => !_isDownloadiong ));
            }
        }

        public RelayCommand LeaveGroups
        {
            get
            {
                return  new RelayCommand((obj) =>
                {
                    SettingsModel.GroupCleaner(int.Parse(GroupAFKDays),CheckGroupWallClear, CheckGroupAdmin);
                }, o => !string.IsNullOrEmpty(GroupAFKDays));
            }
        }
    }
}
