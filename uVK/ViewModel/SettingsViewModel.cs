using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.Remoting.Lifetime;
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
        private bool _isLeaveGroups = false;
        [Reactive] public string GroupCleanText { get; set; } = "Выполнить очистку";
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
                return new RelayCommand(async (obj) =>
                {
                    _isDownloadiong = true;
                    await Task.Factory.StartNew(() =>
                    {
                        int count = PlayerModel.Audio.Count;
                        int current = 0;
                        WebClient client = new WebClient();
                        foreach (var audio in PlayerModel.Audio)
                        {
                            current++;
                            SaveAudiosText = $"Скачивается {current}/{count}";
                            client.DownloadFile(audio.Url,
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                "\\uVK\\SaveAudios\\" +
                                SettingsModel.GetRightNameAudio(audio));
                        }
                        SaveAudiosText = "Завершено!";
                    });
                    //await SettingsModel.SaveAllAudioAsync(SaveAudiosText);
                }, (obj => !_isDownloadiong ));
            }
        }

        public RelayCommand LeaveGroups
        {
            get
            {
                return  new RelayCommand(async (obj) =>
                {
                    await Task.Factory.StartNew(() =>
                    {
                        _isLeaveGroups = true;
                        List<long> leaveGroups = new List<long>();
                        var groups = ApiDatas.Api.Groups.Get(new VkNet.Model.RequestParams.GroupsGetParams());
                        int count = groups.Count;
                        int current = 0;
                        foreach (var group in groups)
                        {
                            current++;
                            GroupCleanText = $"Проверяем {current}/{count}";
                            SettingsModel.AddOrNotGroup(leaveGroups,group,CheckGroupAdmin,CheckGroupWallClear,int.Parse(GroupAFKDays));
                        }
                        current = 0;
                        count = leaveGroups.Count;
                        foreach (var group in leaveGroups)
                        {
                            GroupCleanText = $"Выходим {current}/{count}";
                            ApiDatas.Api.Groups.Leave(group);
                        }
                        GroupCleanText = "Завершено";
                    });

                    //SettingsModel.GroupCleaner(int.Parse(GroupAFKDays),CheckGroupWallClear, CheckGroupAdmin);
                }, o => !string.IsNullOrEmpty(GroupAFKDays) && _isLeaveGroups);
            }
        }
    }
}
