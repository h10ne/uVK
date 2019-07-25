using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using VkNet.Model.RequestParams;
using uVK.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VkNet.Enums.Filters;
using DynamicData.Binding;
using DynamicData;
using uVK.Interfaces;
using uVK.ViewModel;

namespace uVK.Model
{
    public class PlayerModel : IPlayerModel
    {

        public async void GetPlaylistsAsync(long userId, SourceList<AlbumViewModel> playlistSource)
        {
            await Task.Factory.StartNew(() =>
            {
                var playlists = ApiDatas.Api.Audio.GetPlaylists(userId).ToList();
                foreach (var pl in playlists)
                {
                    var cover = pl.Cover != null ? pl.Cover.Photo135 : pl.Covers.ToList()[0].Photo135;
                    string author;

                    try
                    {
                        author = pl.MainArtists.ToList()[0].Name;
                    }
                    catch
                    {
                        Debug.Assert(pl.OwnerId != null, "pl.OwnerId != null");
                        var user =
                            ApiDatas.Api.Users.Get(userIds: new[] { pl.OwnerId.Value }, ProfileFields.FirstName)[0];
                        author = $"{user.FirstName} {user.LastName}";
                    }

                    AlbumViewModel playList = new AlbumViewModel()
                    {
                        ImageSource = cover,
                        Audios = ApiDatas.Api.Audio.Get(new AudioGetParams() { PlaylistId = pl.Id, OwnerId = pl.OwnerId })
                            .ToList(),
                        Author = author,
                        Title = pl.Title
                    };
                    playlistSource.Add(playList);
                }
            });
        }

        private ObservableCollectionExtended<VkNet.Model.User> GetFriendsWithOpenAudio()
        {
            ObservableCollectionExtended<VkNet.Model.User> friendsWithOpenAudio =
                new ObservableCollectionExtended<VkNet.Model.User>();
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

        public async void DownloadFriendsWithOpenAudioAsync(SourceList<FriendsMusicViewModel> friendsMusics)
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

        public async void AddAudioToListAsync(List<VkNet.Model.Attachments.Audio> audios,
            SourceList<OneAudioViewModel> list, double width = 800, long id = -1)
        {
            await Task.Factory.StartNew(() =>
            {
                if (id != -1)
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

        public void GetUserAudio()
        {
            ApiDatas.Audio = ApiDatas.Api.Audio.Get(new AudioGetParams { Count = ApiDatas.Api.Audio.GetCount(UserDatas.UserId) })
                .ToList();
            var audios = new List<VkNet.Model.Attachments.Audio>();
            foreach (var audio in ApiDatas.Audio)
            {
                if (audio.Url != null)
                {
                    audio.Url = Decoder.DecodeAudioUrl(audio.Url);
                    audios.Add(audio);
                }
            }

            ApiDatas.Audio = audios;
        }

        #region Синхронно

        public void AddCacheToList(ListBox musicList)
        {
            musicList.Items.Clear();
            foreach (var audio in SaveAudios.Audio)
            {
                musicList.Items.Add(audio.Artist + " - " + audio.Title);
            }
        }

        public List<VkNet.Model.Attachments.Audio> Search(string searchRequest, ObservableCollection<OneAudioViewModel> musicList, PlayerViewModel main)
        {
            //Task.Run(() =>
            //{
            if (searchRequest == String.Empty)
            {
                return null;
            }

            var searchAudios = ApiDatas.Api.Audio.Search(new AudioSearchParams
            {
                Query = searchRequest,
                Autocomplete = true,
                SearchOwn = true,
                Count = 50,
                PerformerOnly = false
            }).ToList();
            AddAudioToList(searchAudios, musicList, true);
            main.SearchAudios = searchAudios;
            return null;
            //});
        }

        public void GetPlaylists(long userId, ObservableCollection<AlbumViewModel> playlistSourse)
        {
            playlistSourse.Clear();
            //var albums = ApiDatas.Api.Audio
            var playlists = ApiDatas.Api.Audio.GetPlaylists(userId).ToList();
            foreach (var pl in playlists)
            {
                var cover = pl.Cover != null ? pl.Cover.Photo135 : pl.Covers.ToList()[0].Photo135;
                string author;
                try
                {
                    author = pl.MainArtists.ToList()[0].Name;
                }
                catch
                {
                    Debug.Assert(pl.OwnerId != null, "pl.OwnerId != null");
                    var user =
                        ApiDatas.Api.Users.Get(userIds: new[] { pl.OwnerId.Value }, ProfileFields.FirstName)[0];
                    author = $"{user.FirstName} {user.LastName}";
                }

                var audios = ApiDatas.Api.Audio.Get(new AudioGetParams() { PlaylistId = pl.Id, OwnerId = pl.OwnerId })
                    .ToList();
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

        public ObservableCollectionExtended<FriendsMusicViewModel> DownloadFriendsWithOpenAudio()
        {
            var friends = GetFriendsWithOpenAudio();
            ObservableCollectionExtended<FriendsMusicViewModel> friendsMusics =
                new ObservableCollectionExtended<FriendsMusicViewModel>();
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


        public void AddAudioToList(List<VkNet.Model.Attachments.Audio> audios,
            ObservableCollection<OneAudioViewModel> musicList, bool fromSearch = false)
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