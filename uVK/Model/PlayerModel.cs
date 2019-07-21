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


        public static void Search(string SearchRequest, ObservableCollection<OneAudioViewModel> MusicList)
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

        async public static void GetPlaylistsAsync(long userId, SourceList<AlbumViewModel> PlaylistSource)
        {
            await Task.Factory.StartNew(() =>
            {
                var playlists = ApiDatas.api.Audio.GetPlaylists(userId).ToList();
                foreach (var pl in playlists)
                {
                    string cover;
                    if (pl.Cover != null)
                        cover = pl.Cover.Photo135;
                    else
                        cover = pl.Covers.ToList()[0].Photo135;
                    string author;

                    try
                    {
                        author = pl.MainArtists.ToList()[0].Name;
                    }
                    catch
                    {
                        author = $"{UserDatas.Name} {UserDatas.Surname}";
                    }

                    AlbumViewModel playList = new AlbumViewModel()
                    {
                        ImageSource = cover,
                        Audios = ApiDatas.api.Audio.Get(new AudioGetParams() { PlaylistId = pl.Id }).ToList(),
                        Author = author,
                        Title = pl.Title
                    };
                    PlaylistSource.Add(playList);
                }
            });
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
                            ImageSourse = friend.Photo200.ToString(),
                            Id = friend.Id
                        }); ;
                }
            });
        }


        async public static void AddAudioToListAsync(List<VkNet.Model.Attachments.Audio> audios, SourceList<OneAudioViewModel> list, double width = 800, long id = -1)
        {
            await Task.Factory.StartNew(() =>
            {
                if(id!=-1)
                {
                    audios = ApiDatas.api.Audio.Get(new AudioGetParams()
                    {
                        OwnerId = id
                    }).ToList();
                }

                foreach (var audio in audios)
                {

                    if (audio.Url == null)
                        continue;

                    string imageSource = "/Images/ImageMusic.png";
                    try
                    {
                        imageSource = audio.Album.Cover.Photo270;
                    }
                    catch { }

                    list.Add(new OneAudioViewModel()
                    {
                        ImageSourseString = imageSource,
                        Artist = audio.Artist,
                        Title = audio.Title,
                        Duration = Helpers.Decoder.ConvertTimeToString(audio.Duration),
                        Width = width,
                        Url = Helpers.Decoder.DecodeAudioUrl(audio.Url).ToString(),
                        Durration = audio.Duration
                    });;
                }
            });
        }

        public static void AddCacheToList(ListBox MusicList)
        {
            MusicList.Items.Clear();
            foreach (var audio in SaveAudios.Audio)
            {
                MusicList.Items.Add(audio.Artist + " - " + audio.Title);
            }
        }

        #region Синхронно

        public static void GetPlaylists(long userId, ObservableCollection<AlbumViewModel> PlaylistSourse)
        {
            var playlists = ApiDatas.api.Audio.GetPlaylists(userId).ToList();
            foreach (var pl in playlists)
            {
                string cover;
                if (pl.Cover != null)
                    cover = pl.Cover.Photo135;
                else
                    cover = pl.Covers.ToList()[0].Photo135;
                string author;

                try
                {
                    author = pl.MainArtists.ToList()[0].Name;
                }
                catch
                {
                    author = $"{UserDatas.Name} {UserDatas.Surname}";
                }

                AlbumViewModel playList = new AlbumViewModel()
                {
                    ImageSource = cover,
                    Audios = ApiDatas.api.Audio.Get(new AudioGetParams() { PlaylistId = pl.Id }).ToList(),
                    Author = author,
                    Title = pl.Title
                };
                PlaylistSourse.Add(playList);
            }
        }

        public static ObservableCollectionExtended<FriendsMusicViewModel> DownloadFriendsWithOpenAudio()
        {
            var friends = GetFriendsWithOpenAudio();
            ObservableCollectionExtended<FriendsMusicViewModel> friendsMusics = new ObservableCollectionExtended<FriendsMusicViewModel>();
            int i = 0;
            foreach (var friend in friends)
            {
                friendsMusics.Add(new FriendsMusicViewModel()
                {
                    UserName = $"{friend.FirstName} {friend.LastName}",
                    CountAudio = $"{ApiDatas.api.Audio.GetCount(friend.Id)} аудиозаписей",
                    ImageSourse = friend.Photo200.ToString()
                });
            }
            return friendsMusics;
        }


        public static void AddAudioToList(List<VkNet.Model.Attachments.Audio> audios, ObservableCollection<OneAudioViewModel> MusicList, bool fromSearch = false)
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
                string imageSource = "/Images/ImageMusic.png";
                try
                {
                    imageSource = audio.Album.Cover.Photo270;
                }
                catch { }

                MusicList.Add(new OneAudioViewModel()
                {
                    ImageSourseString = imageSource,
                    Artist = audio.Artist,
                    Title = audio.Title,
                    Duration = Helpers.Decoder.ConvertTimeToString(audio.Duration),
                    Width = 800,
                    Url = Helpers.Decoder.DecodeAudioUrl(audio.Url).ToString(),
                    Durration = audio.Duration
                });
            }
        }
        #endregion
    }
}

