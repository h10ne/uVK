using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using uVK.ViewModel;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace uVK.Helpers
{
    public class Playlist
    {
        private IState State { get; }

        public Playlist(IState ws)
        {
            State = ws;
        }

        public void NextSong(PlayerViewModel main)
        {
            State.NextSong(main);
        }

        public void PrevSong(PlayerViewModel main)
        {
            State.PrevSong(main);
        }

        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            State.SetAudioInfo(main, isback, fromClick);
        }
    }

    public interface IState
    {
        void NextSong(PlayerViewModel main);
        void PrevSong(PlayerViewModel main);
        void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false);
    }


    class IdAudios : IState
    {
        int _offset;

        public void NextSong(PlayerViewModel main)
        {
            if (main.Random)
            {
                Random rnds = new Random();
                int value = rnds.Next(0, main.FriendsMusicAudios.Count);
                Thread.Sleep(270);
                Debug.Print(value.ToString());
                _offset = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    _offset += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    _offset = 0;
                    SetAudioInfo(main);
                }
            }

            main.SelectedSaveIndex = _offset;
        }

        public void PrevSong(PlayerViewModel main)
        {
            try
            {
                _offset -= 1;
                if (_offset == -1)
                    _offset = main.FriendsMusicAudios.Count - 1;
                main.FriendsMusicAudiosSelectedIndex = _offset;
                SetAudioInfo(main, true);
            }
            catch
            {
                //SetAudioInfo(main, true);
            }
        }

        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            if (fromClick)
                _offset = main.FriendsMusicAudiosSelectedIndex;
            while (main.FriendsMusicAudios[_offset].Url == null)
            {
                if (isback)
                    _offset--;
                else
                    _offset++;
            }

            main.Player.URL = main.FriendsMusicAudios[_offset].Url;

            main.CurrentTimePositionValue = 0;
            main.MaximumTimePosition = Decoder.ConvertTimeToString(main.FriendsMusicAudios[_offset].Durration);
            main.DurrationMaximum = main.FriendsMusicAudios[_offset].Durration;
            main.Artist = main.FriendsMusicAudios[_offset].Artist;
            main.Title = main.FriendsMusicAudios[_offset].Title;

            /*
             * Делает выделение текущей композиции на моделе, если найдено
             */
            //for (int i = 0; i < main.UserAudios.Count; i++)
            //    if (main.UserAudios[i].ToString() == PlayerModel.Audio[PlayerModel.OffsetOwn].Artist + " - " + PlayerModel.Audio[PlayerModel.OffsetOwn].Title)
            //    {
            //        string str = main.UserAudios[i].ToString();
            //        main.SelectedIndex = i;
            //        break;
            //    }
            main.Player.controls.play();

            try
            {
                main.ImageSource = main.FriendsMusicAudios[_offset].ImageSourseString;
            }
            catch
            {
                main.ImageSource = @"/Images/ImageMusic.png";
            }
        }
    }


    public class AlbumAudios : IState
    {
        private readonly List<Audio> _audios;
        private int _offset;

        public AlbumAudios(List<Audio> audios, PlayerViewModel main)
        {
            _audios = audios;
            main.State = PlayerViewModel.PlaylistState.Album;
            main.Model.AddAudioToList(_audios, main.AlbumAudios);
        }

        public void NextSong(PlayerViewModel main)
        {
            if (main.Random)
            {
                Random rnds = new Random();
                int value = rnds.Next(0, _audios.Count - 1);
                Thread.Sleep(270);
                _offset = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    _offset += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    _offset = 0;
                    SetAudioInfo(main);
                }
            }
        }

        public void PrevSong(PlayerViewModel main)
        {
            try
            {
                _offset -= 1;
                if (_offset == -1)
                    _offset = _audios.Count - 1;
                SetAudioInfo(main, true);
            }
            catch
            {
                SetAudioInfo(main, true);
            }
        }

        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            if (fromClick)
                _offset = main.SelectedAlbumAudiosIndex;
            while (_audios[_offset].Url == null)
            {
                if (isback)
                    _offset--;
                else
                    _offset++;
            }

            main.Player.URL = Decoder.DecodeAudioUrl(_audios[_offset].Url).ToString();
            main.MaximumTimePosition = _audios[_offset].Duration.ToString();
            main.IsPlay = true;
            main.CurrentTimePositionValue = 0;
            main.MaximumTimePosition = Decoder.ConvertTimeToString(_audios[_offset].Duration);
            main.DurrationMaximum = _audios[_offset].Duration;
            main.Artist = _audios[_offset].Artist;
            main.Title = _audios[_offset].Title;

            /*
             * Делает выделение текущей композиции на моделе, если найдено
             */
            for (int i = 0; i < main.AlbumAudios.Count; i++)
                if (main.AlbumAudios[i].ToString() == _audios[_offset].Artist + " - " + _audios[_offset].Title)
                {
                    main.SelectedAlbumAudiosIndex = i;
                    break;
                }

            main.Player.controls.play();

            try
            {
                main.ImageSource = _audios[_offset].Album.Cover.Photo135;
            }
            catch
            {
                main.ImageSource = @"/Images/ImageMusic.png";
            }
        }
    }

    public class SavesAudios : IState
    {
        private int _offset;
        public SavesAudios(PlayerViewModel main)
        {
            main.State = PlayerViewModel.PlaylistState.Save;
        }

        public void NextSong(PlayerViewModel main)
        {
            if (main.Random)
            {
                Random rnds = new Random();
                int value = rnds.Next(0, SaveAudios.Audio.Count);
                Thread.Sleep(270);
                Debug.Print(value.ToString());
                _offset = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    _offset += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    _offset = 0;
                    SetAudioInfo(main);
                }
            }
        }

        public void PrevSong(PlayerViewModel main)
        {
            try
            {
                _offset -= 1;
                if (_offset == -1)
                    _offset = SaveAudios.Audio.Count - 1;
                main.SelectedSaveIndex = _offset;
                SetAudioInfo(main, true);
            }
            catch
            {
                //SetAudioInfo(main, true);
            }
        }

        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            main.ImageSource = @"/Images/ImageMusic.png";
            if (fromClick)
            {
                _offset = main.SelectedSaveIndex;
            }

            main.Player.URL = SaveAudios.Audio[_offset].Url;
            main.Artist = SaveAudios.Audio[_offset].Artist;
            main.Title = SaveAudios.Audio[_offset].Title;
            main.Player.controls.play();
            SetDurration(main);
        }

        private async void SetDurration(PlayerViewModel main)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(100);
                    main.DurrationMaximum = main.Player.currentMedia.duration;
                    main.MaximumTimePosition = main.Player.currentMedia.durationString;
                });
            }
            catch
            {
                // ignored
            }
        }
    }

    public class OwnAudios : IState
    {
        private int _offset;
        public OwnAudios(PlayerViewModel main)
        {
            main.State = PlayerViewModel.PlaylistState.Own;
        }

        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            if (fromClick)
                _offset = main.SelectedIndex;
            while (ApiDatas.Audio[_offset].Url == null)
            {
                if (isback)
                    _offset--;
                else
                    _offset++;
            }

            main.Player.URL = ApiDatas.Audio[_offset].Url.ToString();
            main.MaximumTimePosition = ApiDatas.Audio[_offset].Duration.ToString();

            main.CurrentTimePositionValue = 0;
            main.MaximumTimePosition = Decoder.ConvertTimeToString(ApiDatas.Audio[_offset].Duration);
            main.DurrationMaximum = ApiDatas.Audio[_offset].Duration;
            main.Artist = ApiDatas.Audio[_offset].Artist;
            main.Title = ApiDatas.Audio[_offset].Title;

            /*
             * Делает выделение текущей композиции на моделе, если найдено
             */
            for (int i = 0; i < main.UserAudios.Count; i++)
                if (main.UserAudios[i].ToString() == ApiDatas.Audio[_offset].Artist + " - " +
                    ApiDatas.Audio[_offset].Title)
                {
                    main.SelectedIndex = i;
                    break;
                }

            main.Player.controls.play();

            try
            {
                main.ImageSource = ApiDatas.Audio[_offset].Album.Cover.Photo135;
            }
            catch
            {
                main.ImageSource = @"/Images/ImageMusic.png";
            }
        }

        public void NextSong(PlayerViewModel main)
        {
            if (main.Random)
            {
                Random rnds = new Random();
                int value = rnds.Next(0, ApiDatas.Audio.Count - 1);
                Thread.Sleep(270);
                _offset = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    _offset += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    _offset = 0;
                    SetAudioInfo(main);
                }
            }
        }

        public void PrevSong(PlayerViewModel main)
        {
            try
            {
                _offset -= 1;
                if (_offset == -1)
                    _offset = ApiDatas.Audio.Count - 1;
                SetAudioInfo(main, true);
            }
            catch
            {
                SetAudioInfo(main, true);
            }
        }
    }

    public class SearchAudios : IState
    {
        private readonly List<Audio> _audios;
        private int _offset;
        public SearchAudios(PlayerViewModel main, List<Audio> audios)
        {
            _audios = audios;
            main.State = PlayerViewModel.PlaylistState.Search;
        }

        public void PrevSong(PlayerViewModel main)
        {
            try
            {
                _offset -= 1;
                SetAudioInfo(main);
            }
            catch
            {
                _offset = _audios.Count - 1;
                SetAudioInfo(main, true);
            }
        }

        public void NextSong(PlayerViewModel main)
        {
            if (_audios != null)
            {
                if (main.Random)
                {
                    Random rnds = new Random();
                    var value = rnds.Next(0, _audios.Count);
                    _offset = value;
                    SetAudioInfo(main);
                }
                else
                {
                    try
                    {
                        _offset += 1;
                        SetAudioInfo(main);
                    }
                    catch
                    {
                        _offset = 0;
                        SetAudioInfo(main);
                    }
                }
            }
        }

        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            if (fromClick)
            {
                _offset = main.SelectedIndex;
                if (_offset == -1)
                    return;
            }

            while (ApiDatas.Audio[_offset].Url == null)
            {
                if (isback)
                    _offset--;
                else
                    _offset++;
            }

            main.Player.URL = Decoder.DecodeAudioUrl(_audios[_offset].Url)
                .ToString();
            main.Artist = _audios[_offset].Artist;
            main.Title = _audios[_offset].Title;
            main.MaximumTimePosition =
                Decoder.ConvertTimeToString(_audios[_offset].Duration);
            main.DurrationMaximum = _audios[_offset].Duration;
            for (int i = 0; i < main.UserAudios.Count; i++)
            {
                if (main.UserAudios[i].ToString() == _audios[_offset].Artist
                    + " - " + _audios[_offset].Title)
                {
                    main.SelectedIndex = i;
                    break;
                }
            }

            main.Player.controls.play();


            try
            {
                main.ImageSource = _audios[_offset].Album.Cover.Photo135;
            }
            catch
            {
                main.ImageSource = @"/Images/ImageMusic.png";
            }
        }
    }

    //class RecommendedAudio : IState
    //{
    //    public void PrevSong(PlayerViewModel main)
    //    {
    //        try
    //        {

    //            PlayerModel.OffsetRecom -= 1; ;
    //            if (PlayerModel.OffsetRecom == -1)
    //                PlayerModel.OffsetRecom = PlayerModel.RecommendedAudio.Count - 1;
    //            SetAudioInfo(main, true);
    //        }
    //        catch
    //        {
    //            SetAudioInfo(main, true);
    //        }
    //    }

    //    public void NextSong(PlayerViewModel main)
    //    {
    //        if (main.RandomAudioButton.IsChecked.Value)
    //        {
    //            Random rnds = new Random();
    //            int value = rnds.Next(0, PlayerModel.RecommendedAudio.Count - 1);
    //            Thread.Sleep(270);
    //            PlayerModel.OffsetRecom = value;
    //            SetAudioInfo(main);
    //        }
    //        else
    //        {
    //            try
    //            {
    //                PlayerModel.OffsetRecom += 1;
    //                SetAudioInfo(main);
    //            }
    //            catch
    //            {
    //                PlayerModel.OffsetRecom = 0;
    //                SetAudioInfo(main);
    //            }
    //        }
    //    }
    //    public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
    //    {
    //        if (fromClick)
    //            foreach (var audio in PlayerModel.RecommendedAudio)
    //            {
    //                if (audio.Artist + " - " + audio.Title == main.RecommendationsList.SelectedItem.ToString())
    //                {
    //                    PlayerModel.OffsetRecom = main.RecommendationsList.SelectedIndex;
    //                    break;
    //                }
    //            }
    //        while (PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Url == null)
    //        {
    //            if (isback)
    //                PlayerModel.OffsetRecom--;
    //            else
    //                PlayerModel.OffsetRecom++;
    //        }
    //        var url = PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Url;
    //        string path = url.Scheme + "://" + url.Authority + url.AbsolutePath;
    //        main.Player.URL = url.Scheme + "://" + url.Authority + url.AbsolutePath;
    //        main.Artist = PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Artist;
    //        main.Title = PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Title;
    //        for (int i = 0; i < main.MusicList.Items.Count; i++)
    //            if (main.RecommendationsList.Items[i].ToString() == PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Artist + " - " + PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Title)
    //            {
    //                string str = main.RecommendationsList.Items[i].ToString();
    //                main.RecommendationsList.SelectedIndex = i;
    //                break;
    //            }
    //        main.Player.controls.play();

    //        try
    //        {
    //            var uriImageSource = new Uri(PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
    //            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
    //        }
    //        catch
    //        {
    //            var uriImageSource = new Uri("https://s8.hostingkartinok.com/uploads/images/2019/04/ba91888882438a65608f0f6c2906af44.png", UriKind.RelativeOrAbsolute);
    //            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
    //        }
    //    }
    //}
    //class HotAudio : IState
    //{
    //    public void PrevSong(PlayerViewModel main)
    //    {
    //        try
    //        {

    //            main.SelectedIndex -= 1;
    //            SetAudioInfo(main, true);
    //        }
    //        catch
    //        {
    //            main.SelectedIndex = main.MusicList.Items.Count - 1;
    //            SetAudioInfo(main, true);
    //        }
    //    }

    //    public void NextSong(PlayerViewModel main)
    //    {
    //        if (main.RandomAudioButton.IsChecked.Value)
    //        {
    //            Random rnds = new Random();
    //            int value = rnds.Next(0, main.MusicList.Items.Count);
    //            main.SelectedIndex = value;
    //            SetAudioInfo(main);
    //        }
    //        else
    //        {
    //            try
    //            {
    //                main.SelectedIndex += 1;
    //                SetAudioInfo(main);
    //            }
    //            catch
    //            {
    //                main.SelectedIndex = 0;
    //                SetAudioInfo(main);
    //            }
    //        }
    //    }
    //    public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
    //    {
    //        foreach (var audio in PlayerModel.HotAudios)
    //            if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
    //            {
    //                if (audio.Url != null)
    //                {
    //                    main.Player.URL = audio.Url.ToString();
    //                    main.Artist = audio.Artist;
    //                    main.Title = audio.Title;
    //                    main.Player.controls.play();
    //                    break;
    //                }
    //                else if (isback)
    //                {
    //                    main.SelectedIndex -= 1;
    //                    SetAudioInfo(main, true);
    //                }
    //                else
    //                {
    //                    main.SelectedIndex += 1;
    //                    SetAudioInfo(main, false);
    //                }
    //            }
    //    }
    //}
}