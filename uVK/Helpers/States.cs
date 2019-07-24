using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using uVK.Model;
using uVK.ViewModel;

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
            main.SelectedSaveIndex = PlayerModel.OffsetSave;
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

            PlayerModel.Player.URL = main.FriendsMusicAudios[_offset].Url;

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
            PlayerModel.Player.controls.play();

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
        private readonly List<VkNet.Model.Attachments.Audio> _audios;
        private int _offset;
        public AlbumAudios(List<VkNet.Model.Attachments.Audio> audios, PlayerViewModel main)
        {
            _audios = audios;
            main.State = PlayerModel.PlaylistState.Album;
            PlayerModel.AddAudioToList(_audios, main.AlbumAudios);
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
            PlayerModel.Player.URL = Decoder.DecodeAudioUrl(_audios[_offset].Url).ToString();
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
            PlayerModel.Player.controls.play();

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
        public SavesAudios(PlayerViewModel main)
        {
            main.State = PlayerModel.PlaylistState.Save;
        }
        public void NextSong(PlayerViewModel main)
        {
            if (main.Random)
            {
                Random rnds = new Random();
                int value = rnds.Next(0, SaveAudios.Audio.Count);
                Thread.Sleep(270);
                Debug.Print(value.ToString());
                PlayerModel.OffsetSave = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    PlayerModel.OffsetSave += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    PlayerModel.OffsetSave = 0;
                    SetAudioInfo(main);
                }
            }
            main.SelectedSaveIndex = PlayerModel.OffsetSave;
        }
        public void PrevSong(PlayerViewModel main)
        {
            try
            {

                PlayerModel.OffsetSave -= 1;
                if (PlayerModel.OffsetSave == -1)
                    PlayerModel.OffsetSave = SaveAudios.Audio.Count - 1;
                main.SelectedSaveIndex = PlayerModel.OffsetSave;
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
                PlayerModel.OffsetSave = main.SelectedSaveIndex;
            }
            PlayerModel.Player.URL = SaveAudios.Audio[PlayerModel.OffsetSave].Url;
            main.Artist = SaveAudios.Audio[PlayerModel.OffsetSave].Artist;
            main.Title = SaveAudios.Audio[PlayerModel.OffsetSave].Title;
            PlayerModel.Player.controls.play();
            SetDurration(main);
        }

        private async void SetDurration(PlayerViewModel main)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(100);
                    main.DurrationMaximum = PlayerModel.Player.currentMedia.duration;
                    main.MaximumTimePosition = PlayerModel.Player.currentMedia.durationString;
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
        public OwnAudios(PlayerViewModel main)
        {
            main.State = PlayerModel.PlaylistState.Own;
        }
        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            if (fromClick)
                PlayerModel.OffsetOwn = main.SelectedIndex;
            while (PlayerModel.Audio[PlayerModel.OffsetOwn].Url == null)
            {
                if (isback)
                    PlayerModel.OffsetOwn--;
                else
                    PlayerModel.OffsetOwn++;
            }
            PlayerModel.Player.URL = PlayerModel.Audio[PlayerModel.OffsetOwn].Url.ToString();
            main.MaximumTimePosition = PlayerModel.Audio[PlayerModel.OffsetOwn].Duration.ToString();            

            main.CurrentTimePositionValue = 0;
            main.MaximumTimePosition = Decoder.ConvertTimeToString(PlayerModel.Audio[PlayerModel.OffsetOwn].Duration);
            main.DurrationMaximum = PlayerModel.Audio[PlayerModel.OffsetOwn].Duration;
            main.Artist = PlayerModel.Audio[PlayerModel.OffsetOwn].Artist;
            main.Title = PlayerModel.Audio[PlayerModel.OffsetOwn].Title;

            /*
             * Делает выделение текущей композиции на моделе, если найдено
             */             
            for (int i = 0; i < main.UserAudios.Count; i++)
                if (main.UserAudios[i].ToString() == PlayerModel.Audio[PlayerModel.OffsetOwn].Artist + " - " + PlayerModel.Audio[PlayerModel.OffsetOwn].Title)
                {
                    main.SelectedIndex = i;
                    break;
                }
            PlayerModel.Player.controls.play();

            try
            {
                main.ImageSource = PlayerModel.Audio[PlayerModel.OffsetOwn].Album.Cover.Photo135;
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
                int value = rnds.Next(0, PlayerModel.Audio.Count - 1);
                Thread.Sleep(270);
                PlayerModel.OffsetOwn = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    PlayerModel.OffsetOwn += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    PlayerModel.OffsetOwn = 0;
                    SetAudioInfo(main);
                }
            }
        }

        public void PrevSong(PlayerViewModel main)
        {
            try
            {

                PlayerModel.OffsetOwn -= 1;
                if (PlayerModel.OffsetOwn == -1)
                    PlayerModel.OffsetOwn = PlayerModel.Audio.Count - 1;
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
        public SearchAudios(PlayerViewModel main)
        {
            main.State = PlayerModel.PlaylistState.Search;
        }
        public void PrevSong(PlayerViewModel main)
        {
            try
            {

                PlayerModel.OffsetSearch -= 1;
                SetAudioInfo(main);
            }
            catch
            {
                PlayerModel.OffsetSearch = PlayerModel.SearchAudios.Count - 1;
                SetAudioInfo(main, true);
            }
        }

        public void NextSong(PlayerViewModel main)
        {
            if (PlayerModel.SearchAudios != null)
            {
                if (main.Random)
                {
                    Random rnds = new Random();
                    var value = rnds.Next(0, PlayerModel.SearchAudios.Count);
                    PlayerModel.OffsetSearch = value;
                    SetAudioInfo(main);
                }
                else
                {
                    try
                    {
                        PlayerModel.OffsetSearch += 1;
                        SetAudioInfo(main);
                    }
                    catch
                    {
                        PlayerModel.OffsetSearch = 0;
                        SetAudioInfo(main);
                    }
                }
            }
        }
        public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
        {
            if (fromClick)
            {
                PlayerModel.OffsetSearch = main.SelectedIndex;
                if (PlayerModel.OffsetSearch == -1)
                    return;
            }

            while (PlayerModel.Audio[PlayerModel.OffsetSearch].Url == null)
            {
                if (isback)
                    PlayerModel.OffsetSearch--;
                else
                    PlayerModel.OffsetSearch++;
            }
            PlayerModel.Player.URL = Decoder.DecodeAudioUrl(PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Url).ToString();
            main.Artist = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Artist;
            main.Title = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Title;
            main.MaximumTimePosition = Decoder.ConvertTimeToString(PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Duration);
            main.DurrationMaximum = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Duration;
            for (int i = 0; i < main.UserAudios.Count; i++)
            {
                if (main.UserAudios[i].ToString() == PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Artist
                    + " - " + PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Title)
                {
                    main.SelectedIndex = i;
                    break;
                }
            }

            PlayerModel.Player.controls.play();


            try
            {
                main.ImageSource = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Album.Cover.Photo135;
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
    //        PlayerModel.Player.URL = url.Scheme + "://" + url.Authority + url.AbsolutePath;
    //        main.Artist = PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Artist;
    //        main.Title = PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Title;
    //        for (int i = 0; i < main.MusicList.Items.Count; i++)
    //            if (main.RecommendationsList.Items[i].ToString() == PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Artist + " - " + PlayerModel.RecommendedAudio[PlayerModel.OffsetRecom].Title)
    //            {
    //                string str = main.RecommendationsList.Items[i].ToString();
    //                main.RecommendationsList.SelectedIndex = i;
    //                break;
    //            }
    //        PlayerModel.Player.controls.play();

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
    //                    PlayerModel.Player.URL = audio.Url.ToString();
    //                    main.Artist = audio.Artist;
    //                    main.Title = audio.Title;
    //                    PlayerModel.Player.controls.play();
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


