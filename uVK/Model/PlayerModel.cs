using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VkNet.Model.RequestParams;
using uVK.Helpers;
using System.Windows;
using uVK.States;
using System.Collections.ObjectModel;
using uVK.Styles.AudioStyles;
using VkNet.Enums.Filters;
using DynamicData.Binding;
using DynamicData;
using uVK.ViewModel;

namespace uVK.Model
{
    public static class PlayerModel
    {

        public enum PlaylistState
        {
            own,
            save,
            recommend,
            search,
            album
        }

        #region Variables
        public static List<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
        public static List<VkNet.Model.Attachments.Audio> Audio { get; set; }
        public static List<VkNet.Model.Attachments.Audio> RecommendedAudio { get; set; }
        public static int OffsetOwn = 0;
        public static int OffsetSearch = 0;
        public static int OffsetRecom = 0;
        public static int OffsetSave = 0;
        public static WMPLib.WindowsMediaPlayer Player = new WMPLib.WindowsMediaPlayer();
        public static Playlist Playlist;
        #endregion


        public static void Search(string SearchRequest, ObservableCollection<AudioList> MusicList)
        {
            //Task.Run(() =>
            //{
            PlayerModel.SearchAudios = PlayerModel.SearchAudios = ApiDatas.api.Audio.Search(new AudioSearchParams
            {
                Query = SearchRequest,
                Autocomplete = true,
                SearchOwn = true,
                Count = 50,
                PerformerOnly = false
            }).ToList();
            PlayerModel.AddAudioToList(PlayerModel.SearchAudios, MusicList, true);

            //});
        }
        public static void Getplaylists(long userId, ObservableCollection<PlayList> PlaylistSourse)
        {
            var playlists = ApiDatas.api.Audio.GetPlaylists(userId).ToList();
            foreach (var pl in playlists)
            {
                PlayList playList = new PlayList(pl);
                PlaylistSourse.Add(playList);
            }
        }

        private static ObservableCollectionExtended<VkNet.Model.User> GetFriendsWithOpenAudio()
        {
            ObservableCollectionExtended<VkNet.Model.User> FriendsWithOpenAudio = new ObservableCollectionExtended<VkNet.Model.User>();
            var friends = ApiDatas.api.Friends.Get(new FriendsGetParams
            {
                Fields = ProfileFields.All,
                Order = VkNet.Enums.SafetyEnums.FriendsOrder.Hints
            });
            foreach (var friend in friends)
            {
                if (friend.CanSeeAudio)
                {
                    FriendsWithOpenAudio.Add(friend);
                }
            }
            return FriendsWithOpenAudio;
        }

        public static ObservableCollectionExtended<FriendsMusicViewModel> DownloadFriendsWithOpenAudio()
        {
            var friends = GetFriendsWithOpenAudio();
            ObservableCollectionExtended<FriendsMusicViewModel> friendsMusics = new ObservableCollectionExtended<FriendsMusicViewModel>();
            int i = 0;
            foreach (var friend in friends)
            {
                //    i++;
                friendsMusics.Add(new FriendsMusicViewModel()
                {
                    UserName = $"{friend.FirstName} {friend.LastName}",
                    CountAudio = $"{ApiDatas.api.Audio.GetCount(friend.Id)} аудиозаписей",
                    ImageSourse = friend.Photo200.ToString()
                });
                //if (i == 2)
                //    break;
            }
            return friendsMusics;
        }

        async public static void DownloadFriendsWithOpenAudioAsync(SourceList<FriendsMusicViewModel> friendsMusics)
        {
            await Task.Factory.StartNew(() =>
            {
                var friends = GetFriendsWithOpenAudio();
                int i = 0;
                foreach (var friend in friends)
                {
                    long count = ApiDatas.api.Audio.GetCount(friend.Id);
                    if (count != 0)
                        friendsMusics.Add(new FriendsMusicViewModel()
                        {
                            UserName = $"{friend.FirstName} {friend.LastName}",
                            CountAudio = $"{count} аудиозаписей",
                            ImageSourse = friend.Photo200.ToString()
                        });
                }
            });
        }


        public static void AddAudioToList(List<VkNet.Model.Attachments.Audio> audios, ObservableCollection<AudioList> MusicList, bool fromSearch = false)
        {
            //MusicList = new ObservableCollection<AudioList>();
            MusicList.Clear();
            //if (fromSearch)
            //{
            //    bool IncludeOwn = false;
            //    int startValue = 0;
            //    for (int i = 0; i < SearchAudios.Count; i++)
            //    {
            //        foreach (var ownaudio in Audio)
            //        {
            //            if (ownaudio.AccessKey == SearchAudios[i].AccessKey)
            //            {
            //                if (!IncludeOwn)
            //                {
            //                    IncludeOwn = true;
            //                    //MusicList.Add("               Ваши аудиозаписи:");
            //                }
            //                MusicList.Add(new AudioList(SearchAudios[i]));
            //                startValue++;
            //            }
            //        }
            //    }
            //    //if (startValue != 0)
            //    //    MusicList.Add("               Все аудиозаписи:");
            //    for (int j = startValue; j < SearchAudios.Count; j++)
            //    {
            //        MusicList.Add(new AudioList(SearchAudios[j]));
            //    }
            //    return;
            //}
            foreach (var audio in audios)
            {
                MusicList.Add(new AudioList(audio));
            }
        }
        public static void AddCacheToList(ListBox MusicList)
        {
            MusicList.Items.Clear();
            foreach (var audio in SaveAudios.Audio)
            {
                MusicList.Items.Add(audio.Artist + " - " + audio.Title);
            }
        }
    }
}

