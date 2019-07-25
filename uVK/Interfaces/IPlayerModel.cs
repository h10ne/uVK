using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using DynamicData;
using DynamicData.Binding;
using uVK.ViewModel; 

namespace uVK.Interfaces
{
    public interface IPlayerModel
    {
        #region Async
        void GetPlaylistsAsync(long userId, SourceList<AlbumViewModel> playlistSource);
        void DownloadFriendsWithOpenAudioAsync(SourceList<FriendsMusicViewModel> friendsMusics);
        void AddAudioToListAsync(List<VkNet.Model.Attachments.Audio> audios,
            SourceList<OneAudioViewModel> list, double width = 800, long id = -1);
        void GetUserAudio();
        #endregion

        #region NonAsync

        void AddCacheToList(ListBox musicList);

        List<VkNet.Model.Attachments.Audio> Search(string searchRequest, ObservableCollection<OneAudioViewModel> musicList, PlayerViewModel main);

        void GetPlaylists(long userId, ObservableCollection<AlbumViewModel> playlistSourse);

        ObservableCollectionExtended<FriendsMusicViewModel> DownloadFriendsWithOpenAudio();

        void AddAudioToList(List<VkNet.Model.Attachments.Audio> audios,
            ObservableCollection<OneAudioViewModel> musicList, bool fromSearch = false);

        #endregion

    }
}