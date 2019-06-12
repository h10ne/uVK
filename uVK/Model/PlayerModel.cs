using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VkNet.Model.RequestParams;

namespace uVK.Model
{
    public static class PlayerModel
    {        
        #region Variables
        public static  VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
        public static VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> Audio { get; set; }
        public static VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> RecommendedAudio { get; set; }
        //        public SaveAudios Cache;
        public static int OffsetOwn = 0;
        public static int OffsetSearch = 0;
        //        public static int OffsetHot = -1;
        public static int OffsetRecom = 0;
        public static int OffsetSave = 0;
        public static WMPLib.WindowsMediaPlayer Player = new WMPLib.WindowsMediaPlayer();
        public static Playlist Playlist;
        public static State state { get; set; }
        #endregion
        public enum State
        {
            own,
            save,
            recommend,
            search
        }
        public static void Search(string SearchRequest, ListBox MusicList)
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
                });
                PlayerModel.state = PlayerModel.State.search;
                PlayerModel.AddAudioToList(PlayerModel.SearchAudios, MusicList);
                PlayerModel.Playlist = new Playlist(new SearchAudios());

            //});
        }
        public static void AddAudioToList(VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> audios, ListBox MusicList)
        {
            MusicList.Items.Clear();
            if (state == State.search)
            {
                bool IncludeOwn = false;
                int startValue = 0;
                for (int i = 0; i < SearchAudios.Count; i++)
                {
                    foreach (var ownaudio in Audio)
                    {
                        if (ownaudio.AccessKey == SearchAudios[i].AccessKey)
                        {
                            if (!IncludeOwn)
                            {
                                IncludeOwn = true;
                                MusicList.Items.Add("               Ваши аудиозаписи:");
                            }
                            MusicList.Items.Add(SearchAudios[i].Artist + " - " + SearchAudios[i].Title);
                            startValue++;
                        }
                    }
                }
                if (startValue != 0)
                    MusicList.Items.Add("               Все аудиозаписи:");
                for (int j = startValue; j < SearchAudios.Count; j++)
                {
                    MusicList.Items.Add(SearchAudios[j].Artist + " - " + SearchAudios[j].Title);
                }
                return;
            }
            foreach (var audio in audios)
            {
                string title = $"{audio.Artist} - {audio.Title}";
                MusicList.Items.Add(title);
            }
        }
    }
}

