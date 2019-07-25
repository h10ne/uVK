using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using uVK.Helpers;
using uVK.Model;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

namespace uVK.ViewModel
{
    public class SettingsViewModel : ReactiveObject
    {
        public SettingsViewModel()
        {
            if (ApiDatas.Api.Groups.IsMember("180253523",UserDatas.UserId,null,null).Select(x=>x.Member).FirstOrDefault())
            {
                IsInGroup = true;
                JoinEnable = false;
            }
        }
        #region variables

        private bool _isDownloadiong;
        private bool _isLeaveGroups;
        private bool _isCleanFriends;
        [Reactive] public bool IsInGroup { get; set; } = false;

        public RelayCommand JoinToGroup
        {
            get
            {
                return  new RelayCommand((obj) =>
                {
                    ApiDatas.Api.Groups.Join(180253523);
                    IsInGroup = true;
                    JoinEnable = false;
                }, o => JoinEnable);
            }
        }

        [Reactive] public bool JoinEnable { get; set; } = true;
        [Reactive] public string GroupCleanText { get; set; } = "Выполнить очистку";
        [Reactive] public string FriendCleanText { get; set; } = "Выполнить очистку";
        [Reactive] public string SaveAudiosText { get; set; } = "Сохранить все аудиозаписи";
        [Reactive] public bool CheckGroupAdmin { get; set; }
        [Reactive] public bool CheckFriendSub { get; set; }
        [Reactive] public bool CheckGroupWallClear { get; set; }
        private string _groupAfkDays;
        private string _friendAfkDays;
        public string FriendAFKDays
        {
            get => _friendAfkDays;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.RaiseAndSetIfChanged(ref _friendAfkDays, value);
                    return;
                }

                if (char.IsDigit(value[value.Length - 1]))
                {
                    this.RaiseAndSetIfChanged(ref _friendAfkDays, value);
                    return;
                }

                this.RaiseAndSetIfChanged(ref _friendAfkDays, _friendAfkDays);
            }
        }

        public string GroupAFKDays
        {
            get => _groupAfkDays;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.RaiseAndSetIfChanged(ref _groupAfkDays, value);
                    return;
                }

                if (char.IsDigit(value[value.Length - 1]))
                {
                    this.RaiseAndSetIfChanged(ref _groupAfkDays, value);
                    return;
                }

                this.RaiseAndSetIfChanged(ref _groupAfkDays, _groupAfkDays);
            }
        }

        #endregion

        public RelayCommand Logout
        {
            get { return new RelayCommand((obj) => { SettingsModel.Logout(); }); }
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
                        int count = ApiDatas.Audio.Count;
                        int current = 0;
                        WebClient client = new WebClient();
                        foreach (var audio in ApiDatas.Audio)
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
                }, (obj => !_isDownloadiong));
            }
        }

        public RelayCommand FriendsClean
        {
            get
            {
                return new RelayCommand(async (obj) =>
                {
                    await Task.Factory.StartNew(() =>
                    {
                        _isCleanFriends = true;
                        List<long> friendsToDelete = new List<long>();
                        var friends = ApiDatas.Api.Friends.Get(new FriendsGetParams(){Fields = ProfileFields.All});
                        int current = 0;
                        int count = friends.Count;
                        foreach (var friend in friends)
                        {
                            current++;
                            FriendCleanText = $"Проверяем {current}/{count}";
                            SettingsModel.GetFriendCleanResult(friendsToDelete,friend, int.Parse(FriendAFKDays));
                        }
                        if (CheckFriendSub)
                        {
                            friendsToDelete.AddRange(SettingsModel.GetSubs());
                        }

                        current = 0;
                        count = friendsToDelete.Count;
                        foreach (var friend in friendsToDelete)
                        {
                            current++;
                            FriendCleanText = $"Удаляем {current}/{count}";
                            ApiDatas.Api.Friends.Delete(friend);
                        }

                        FriendCleanText = "Завершено";
                    });
                }, o => !string.IsNullOrEmpty(FriendAFKDays) && !_isCleanFriends);
            }
        }

        public RelayCommand LeaveGroups
        {
            get
            {
                return new RelayCommand(async (obj) =>
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
                            SettingsModel.AddOrNotGroup(leaveGroups, group, CheckGroupAdmin, CheckGroupWallClear,
                                int.Parse(GroupAFKDays));
                        }

                        current = 0;
                        count = leaveGroups.Count;
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\uVK\\Logs\\";
                        Directory.CreateDirectory(path);
                        StreamWriter writer = new StreamWriter(path + "LeaveGroupsLog.txt", true);
                        foreach (var group in leaveGroups)
                        {
                            writer.WriteLine($@"https://vk.com/public{group}");
                            GroupCleanText = $"Выходим {current}/{count}";
                            ApiDatas.Api.Groups.Leave(group);
                        }
                        writer.Close();
                        GroupCleanText = "Завершено";
                    });

                    //SettingsModel.GroupCleaner(int.Parse(GroupAFKDays),CheckGroupWallClear, CheckGroupAdmin);
                }, o => !string.IsNullOrEmpty(GroupAFKDays) && !_isLeaveGroups);
            }
        }
    }
}