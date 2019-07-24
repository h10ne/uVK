using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using uVK.Helpers;
using VkNet.Enums.Filters;

namespace uVK.Model
{
    public static class SettingsModel
    {
        public static void Logout()
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                        "\\uVK\\UserDatas\\data.bin");
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public static async Task SaveAllAudioAsync(string complete)
        {
            await Task.Factory.StartNew(() =>
            {
                WebClient webClient = new WebClient();
                foreach (var audio in PlayerModel.Audio)
                {
                    string Name = audio.Artist + "↨" + audio.Title;
                    var per = Name.LastIndexOfAny(new[] {'\\', '/', '?', ':', '*', '/', '>', '<', '|', '\"'});
                    while (per != -1)
                    {
                        Name = Name.Remove(per, 1);
                        per = Name.LastIndexOfAny(new[] {'\\', '/', '?', ':', '*', '/', '>', '<', '|', '\"'});
                    }

                    webClient.DownloadFile(audio.Url,
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\SaveAudios\\" +
                        Name);
                }
            });
        }

        public static string GetRightNameAudio(VkNet.Model.Attachments.Audio audio)
        {
            string Name = audio.Artist + "↨" + audio.Title;
            var per = Name.LastIndexOfAny(new[] {'\\', '/', '?', ':', '*', '/', '>', '<', '|', '\"'});
            while (per != -1)
            {
                Name = Name.Remove(per, 1);
                per = Name.LastIndexOfAny(new[] {'\\', '/', '?', ':', '*', '/', '>', '<', '|', '\"'});
            }

            return Name;
        }

        public static void AddOrNotGroup(List<long> leaveGroups, VkNet.Model.Group group, bool isadmin, bool wallclear,
            int days)
        {
            if (isadmin)
                if (group.IsAdmin == true)
                {
                    Debug.WriteLine("Вы админ");
                    return;
                }

            if (ApiDatas.Api.Groups.GetById(null, group.Id.ToString(), GroupsFields.All)[0].Deactivated != null)
            {
                leaveGroups.Add(group.Id);
                Debug.WriteLine("Заблокирована!");
                return;
            }

            //next step if not banned
            var lastposts = ApiDatas.Api.Wall.Get(new VkNet.Model.RequestParams.WallGetParams
            {
                OwnerId = -group.Id,
                Count = 2
            }).WallPosts;
            if (wallclear)
                if (lastposts.Count == 0)
                {
                    leaveGroups.Add(group.Id);
                    Debug.WriteLine("Стена пустая!");
                    return;
                }

            try
            {
                if (lastposts[0].IsPinned.GetValueOrDefault() != true)
                {
                    var qwe = lastposts[0].Date.GetValueOrDefault().Date;
                    if (DateTime.Now - lastposts[0].Date.GetValueOrDefault().Date > new TimeSpan(days, 0, 0, 0, 0))
                    {
                        leaveGroups.Add(group.Id);
                        Debug.WriteLine("Мертва!");
                        return;
                    }
                }
                else if (DateTime.Now - lastposts[1].Date.GetValueOrDefault().Date > new TimeSpan(days, 0, 0, 0, 0))
                {
                    leaveGroups.Add(group.Id);
                    Debug.WriteLine("Мертва!");
                    return;
                }

                Debug.WriteLine("Все в порядке!");
            }
            catch
            {
                // ignored
            }
        }

        public static async void GroupCleaner(int days, bool wallclear, bool isadmin)
        {
            await Task.Factory.StartNew(() =>
            {
                List<long> leaveGroups = new List<long>();
                var groups = ApiDatas.Api.Groups.Get(new VkNet.Model.RequestParams.GroupsGetParams());
                Debug.WriteLine($"Вы состоите в {groups.Count} группах");
                foreach (var group in groups)
                {
                    Debug.Write($"Проверка {groups.IndexOf(group) + 1} группы. ");
                    if (isadmin)
                        if (group.IsAdmin == true)
                        {
                            Debug.WriteLine("Вы админ");
                            continue;
                        }

                    if (ApiDatas.Api.Groups.GetById(null, group.Id.ToString(), GroupsFields.All)[0].Deactivated != null)
                    {
                        leaveGroups.Add(group.Id);
                        Debug.WriteLine("Заблокирована!");
                        continue;
                    }

                    //next step if not banned
                    var lastposts = ApiDatas.Api.Wall.Get(new VkNet.Model.RequestParams.WallGetParams
                    {
                        OwnerId = -group.Id,
                        Count = 2
                    }).WallPosts;
                    if (wallclear)
                        if (lastposts.Count == 0)
                        {
                            leaveGroups.Add(group.Id);
                            Debug.WriteLine("Стена пустая!");
                            continue;
                        }

                    try
                    {
                        if (lastposts[0].IsPinned.GetValueOrDefault() != true)
                        {
                            var qwe = lastposts[0].Date.GetValueOrDefault().Date;
                            if (DateTime.Now - lastposts[0].Date.GetValueOrDefault().Date >
                                new TimeSpan(days, 0, 0, 0, 0))
                            {
                                leaveGroups.Add(group.Id);
                                Debug.WriteLine("Мертва!");
                                continue;
                            }
                        }
                        else if (DateTime.Now - lastposts[1].Date.GetValueOrDefault().Date >
                                 new TimeSpan(days, 0, 0, 0, 0))
                        {
                            leaveGroups.Add(group.Id);
                            Debug.WriteLine("Мертва!");
                            continue;
                        }

                        Debug.WriteLine("Все в порядке!");
                    }
                    catch
                    {
                        // ignored
                    }
                }

                Debug.WriteLine($"Вы покинете {leaveGroups.Count} групп. Нажмите любую кнопку, что бы подтвердить");
                foreach (var groupId in leaveGroups)
                {
                    Debug.Write($"Покидаем {leaveGroups.IndexOf(groupId)} группу.");
                    ApiDatas.Api.Groups.Leave(groupId);
                    Debug.WriteLine($"Успешно!");
                }

                Debug.Write($"Нажмите любую кнопку, что бы закрыть программу");
            });
        }
    }
}