using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using VkNet.Model.RequestParams;
using uVK.Helpers;
using uVK.Helpers.States;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            Own,
            Save,
            Search,
            Album,
            IdAudios,
            Null
        }

        #region Variables
        public static List<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
        public static List<VkNet.Model.Attachments.Audio> Audio { get; set; }
        public static int OffsetOwn = 0;
        public static int OffsetSearch = 0;
        public static int OffsetSave = 0;
        public static WMPLib.WindowsMediaPlayer Player = new WMPLib.WindowsMediaPlayer();
        public static Playlist Playlist;
        #endregion


        public static void Search(string searchRequest, ObservableCollection<OneAudioViewModel> musicList)
        {
            //Task.Run(() =>
            //{
            if (searchRequest == String.Empty)
            {
                return;
            }
            SearchAudios = SearchAudios = ApiDatas.Api.Audio.Search(new AudioSearchParams
            {
                Query = searchRequest,
                Autocomplete = true,
                SearchOwn = true,
                Count = 50,
                PerformerOnly = false
            }).ToList();
            AddAudioToList(SearchAudios, musicList, true);

            //});
        }

        public static async void GetPlaylistsAsync(long userId, SourceList<AlbumViewModel> playlistSource)
        {
            await Task.Factory.StartNew(() =>
            {
                var playlists = ApiDatas.Api.Audio.GetPlaylists(userId).ToList();
                foreach (var pl in playlists)
                {
                    string cover;
                    cover = pl.Cover != null ? pl.Cover.Photo135 : pl.Covers.ToList()[0].Photo135;
                    string author;

                    try
                    {
                        author = pl.MainArtists.ToList()[0].Name;
                    }
                    catch
                    {
                        Debug.Assert(pl.OwnerId != null, "pl.OwnerId != null");
                        var user =
                            ApiDatas.Api.Users.Get(userIds: new long[] { pl.OwnerId.Value }, ProfileFields.FirstName)[0];
                        author = $"{user.FirstName} {user.LastName}";
                    }

                    AlbumViewModel playList = new AlbumViewModel()
                    {
                        ImageSource = cover,
                        Audios = ApiDatas.Api.Audio.Get(new AudioGetParams() { PlaylistId = pl.Id, OwnerId = pl.OwnerId}).ToList(),
                        Author = author,
                        Title = pl.Title
                    };
                    playlistSource.Add(playList);
                }
            });
        }

        private static ObservableCollectionExtended<VkNet.Model.User> GetFriendsWithOpenAudio()
        {
            ObservableCollectionExtended<VkNet.Model.User> friendsWithOpenAudio = new ObservableCollectionExtended<VkNet.Model.User>();
            var friends = ApiDatas.Api.Friends.Get(new FriendsGetParams
            {
                Fields = ProfileFields.All,
                Order = VkNet.Enums.SafetyEnums.FriendsOrder.Hints
            });
            foreach (var friend in friends)
            {
                if (friend.CanSeeAudio)
                {
                    friendsWithOpenAudio.Add(friend);
                }
            }
            return friendsWithOpenAudio;
        }



        public static async void DownloadFriendsWithOpenAudioAsync(SourceList<FriendsMusicViewModel> friendsMusics)
        {
            await Task.Factory.StartNew(() =>
            {
                var friends = GetFriendsWithOpenAudio();
                foreach (var friend in friends)
                {
                    long count = ApiDatas.Api.Audio.GetCount(friend.Id);
                    if (count != 0)
                        friendsMusics.Add(new FriendsMusicViewModel()
                        {
                            UserName = $"{friend.FirstName} {friend.LastName}",
                            CountAudio = $"{count} аудиозаписей",
                            ImageSourse = friend.Photo200.ToString(),
                            Id = friend.Id
                        });
                }
            });
        }


        public static async void AddAudioToListAsync(List<VkNet.Model.Attachments.Audio> audios, SourceList<OneAudioViewModel> list, double width = 800, long id = -1)
        {
            await Task.Factory.StartNew(() =>
            {
                if(id!=-1)
                {
                    audios = ApiDatas.Api.Audio.Get(new AudioGetParams()
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
                    catch
                    {
                        // ignored
                    }

                    list.Add(new OneAudioViewModel()
                    {
                        ImageSourseString = imageSource,
                        Artist = audio.Artist,
                        Title = audio.Title,
                        Duration = Decoder.ConvertTimeToString(audio.Duration),
                        Width = width,
                        Url = Decoder.DecodeAudioUrl(audio.Url).ToString(),
                        Durration = audio.Duration
                    });
                }
            });
        }

        public static void AddCacheToList(ListBox musicList)
        {
            musicList.Items.Clear();
            foreach (var audio in SaveAudios.Audio)
            {
                musicList.Items.Add(audio.Artist + " - " + audio.Title);
            }
        }

        #region Синхронно

        public static void GetPlaylists(long userId, ObservableCollection<AlbumViewModel> playlistSourse)
        {
            playlistSourse.Clear();
                //var albums = ApiDatas.Api.Audio
            var playlists = ApiDatas.Api.Audio.GetPlaylists(userId).ToList();
            foreach (var pl in playlists)
            {
                string cover;
                cover = pl.Cover != null ? pl.Cover.Photo135 : pl.Covers.ToList()[0].Photo135;
                string author;
                try
                {
                    author = pl.MainArtists.ToList()[0].Name;
                }
                catch
                {
                    Debug.Assert(pl.OwnerId != null, "pl.OwnerId != null");
                    var user =
                        ApiDatas.Api.Users.Get(userIds: new long[] {pl.OwnerId.Value}, ProfileFields.FirstName)[0];
                    author = $"{user.FirstName} {user.LastName}";
                }

                var audios = ApiDatas.Api.Audio.Get(new AudioGetParams() {PlaylistId = pl.Id, OwnerId = pl.OwnerId}).ToList();
                AlbumViewModel playList = new AlbumViewModel()
                {
                    ImageSource = cover,
                    Audios = audios,
                    Author = author,
                    Title = pl.Title
                };
                playlistSourse.Add(playList);
            }
        }

        public static ObservableCollectionExtended<FriendsMusicViewModel> DownloadFriendsWithOpenAudio()
        {
            var friends = GetFriendsWithOpenAudio();
            ObservableCollectionExtended<FriendsMusicViewModel> friendsMusics = new ObservableCollectionExtended<FriendsMusicViewModel>();
            foreach (var friend in friends)
            {
                friendsMusics.Add(new FriendsMusicViewModel()
                {
                    UserName = $"{friend.FirstName} {friend.LastName}",
                    CountAudio = $"{ApiDatas.Api.Audio.GetCount(friend.Id)} аудиозаписей",
                    ImageSourse = friend.Photo200.ToString()
                });
            }
            return friendsMusics;
        }


        public static void AddAudioToList(List<VkNet.Model.Attachments.Audio> audios, ObservableCollection<OneAudioViewModel> musicList, bool fromSearch = false)
        {
            //MusicList = new ObservableCollection<AudioList>();
            musicList.Clear();
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
                if (audio.Url == null)
                {
                    continue;
                }
                string imageSource = "/Images/ImageMusic.png";
                try
                {
                    imageSource = audio.Album.Cover.Photo270;
                }
                catch
                {
                    // ignored
                }

                musicList.Add(new OneAudioViewModel()
                {
                    ImageSourseString = imageSource,
                    Artist = audio.Artist,
                    Title = audio.Title,
                    Duration = Decoder.ConvertTimeToString(audio.Duration),
                    Width = 800,
                    Url = Decoder.DecodeAudioUrl(audio.Url).ToString(),
                    Durration = audio.Duration
                });
            }
        }
        #endregion
    }
}

