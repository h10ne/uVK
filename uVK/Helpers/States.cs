using System;
using System.Diagnostics;
using System.Threading;
using uVK.Model;
using uVK.ViewModel;
using uVK.Helpers;
public class Playlist
{
    public IState State { get; set; }
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

/*
//class IdAudios : IState
//{
//    public void AudioMenuClick(PlayerViewModel main)
//    {
//        foreach (var audio in PlayerModel.IdAudios)
//            if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
//            {
//                main.api.Audio.Add(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
//            }
//    }

//    public void SetAudioInfo(PlayerViewModel main, bool isback = false)
//    {
//        if (main.SelectedIndex == -1)
//            main.MusicList.SelectedItem = 0;
//        foreach (var audio in PlayerModel.IdAudios)
//            if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
//            {
//                if (audio.Url != null)
//                {
//                    PlayerModel.Player.URL = audio.Url.ToString();
//                    main.artist_name.Text = audio.Artist;
//                    main.title_name.Text = audio.Title;
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
//        //if (main.VkBools.isBlack)
//        //    main.play_pause_btn.Image = Resource1.pause_white;
//        //else
//        //    main.play_pause_btn.Image = Resource1.pause;
//        //main.VkBools.isPlay = true;
//    }

//    public void NextSong(PlayerViewModel main)
//    {
//        if (main.VkBools.random)
//        {
//            Random rnds = new Random();
//            int rnd_max = int.Parse(main.api.Audio.GetCount(long.Parse(main.IdSongs_box.Text)).ToString());
//            if (rnd_max > 1800)
//                rnd_max = 1800;
//            int value = rnds.Next(0, rnd_max - 1);
//            main.SelectedIndex = value;
//            Thread.Sleep(270);
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

//    public void PrevSong(PlayerViewModel main)
//    {
//        try
//        {


//            if (main.SelectedIndex <= -1)
//            {
//                main.SelectedIndex = main.MusicList.Items.Count;
//            }
//            else
//                main.SelectedIndex -= 1;
//            SetAudioInfo(main, true);
//        }
//        catch (Exception ex)
//        {
//            main.SelectedIndex = 4998;
//            SetAudioInfo(main, true);
//        }
//    }
//}
//*/

public class SavesAudios : IState
{
    public void NextSong(PlayerViewModel main)
    {
        if (main.Random)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, uVK.Helpers.SaveAudios.Audio.Count);
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

            PlayerModel.OffsetSave -= 1; ;
            if (PlayerModel.OffsetSave == -1)
                PlayerModel.OffsetSave =  uVK.Helpers.SaveAudios.Audio.Count - 1;
            main.SelectedSaveIndex = PlayerModel.OffsetSave;
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
            foreach (var audio in uVK.Helpers.SaveAudios.Audio)
            {
                if (audio.Artist + " - " + audio.Title == main.SelectedSaveItem)
                {
                    PlayerModel.OffsetSave = main.SelectedSaveIndex;
                    PlayerModel.Player.URL = audio.Url.ToString();
                    main.Artist = audio.Artist;
                    main.Title = audio.Title;
                    PlayerModel.Player.controls.play();
                    break;
                }
            }
        else
        {
            PlayerModel.Player.URL = uVK.Helpers.SaveAudios.Audio[PlayerModel.OffsetSave].Url.ToString();
            main.Artist = uVK.Helpers.SaveAudios.Audio[PlayerModel.OffsetSave].Artist;
            main.Title =  uVK.Helpers.SaveAudios.Audio[PlayerModel.OffsetSave].Title;
            for (int i = 0; i < main.SaveAudiosList.Items.Count; i++)
            {
                string saveitem = main.SaveAudiosList.Items[i].ToString();
                if (main.SaveAudiosList.Items[i].ToString() ==  uVK.Helpers.SaveAudios.Audio[PlayerModel.OffsetSave].Artist + " - " +  uVK.Helpers.SaveAudios.Audio[PlayerModel.OffsetSave].Title)
                {
                    string str = main.SaveAudiosList.Items[i].ToString();
                    main.SaveAudiosList.SelectedIndex = i;
                    break;
                }
            }
            PlayerModel.Player.controls.play();

            main.ImageSource = @"/Images/ImageMusic.png";
        }

    }
}
public class OwnAudios : IState
{

    public void SetAudioInfo(PlayerViewModel main, bool isback = false, bool fromClick = false)
    {
        if (fromClick)
            foreach (var audio in PlayerModel.Audio)
            {
                if (audio.Artist + " - " + audio.Title == main.SelectedItem)
                {
                    PlayerModel.OffsetOwn = main.SelectedIndex;
                    break;
                }
            }
        while (PlayerModel.Audio[PlayerModel.OffsetOwn].Url == null)
        {
            if (isback)
                PlayerModel.OffsetOwn--;
            else
                PlayerModel.OffsetOwn++;
        }
        var url = PlayerModel.Audio[PlayerModel.OffsetOwn].Url;
        string path = url.Scheme + "://" + url.Authority + url.AbsolutePath;
        PlayerModel.Player.URL = url.Scheme + "://" + url.Authority + url.AbsolutePath;
        main.MaximumTimePosition = PlayerModel.Audio[PlayerModel.OffsetOwn].Duration.ToString();

        int minutes = PlayerModel.Audio[PlayerModel.OffsetOwn].Duration;
        int seconds = minutes % 60;
        minutes /= 60;
        string minutesStr = minutes.ToString(); ;
        if (minutes < 10)
            minutesStr = "0" + minutes.ToString();
        string secondsStr = seconds.ToString(); ;
        if (seconds < 10)
            secondsStr = "0" + seconds.ToString();

        main.CurrentTimePositionValue = 0;
        main.MaximumTimePosition = minutesStr + ":" + secondsStr;
        main.DurrationMaximum = PlayerModel.Audio[PlayerModel.OffsetOwn].Duration;
        main.Artist = PlayerModel.Audio[PlayerModel.OffsetOwn].Artist;
        main.Title = PlayerModel.Audio[PlayerModel.OffsetOwn].Title;
        for (int i = 0; i < main.MusicList.Items.Count; i++)
            if (main.MusicList.Items[i].ToString() == PlayerModel.Audio[PlayerModel.OffsetOwn].Artist + " - " + PlayerModel.Audio[PlayerModel.OffsetOwn].Title)
            {
                string str = main.MusicList.Items[i].ToString();
                main.SelectedIndex = i;
                break;
            }
        PlayerModel.Player.controls.play();

        //try
        //{
        //    var uriImageSource = new Uri(PlayerModel.Audio[PlayerModel.OffsetOwn].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
        //    main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        //}
        //catch
        //{
        //    var uriImageSource = new Uri("https://s8.hostingkartinok.com/uploads/images/2019/04/ba91888882438a65608f0f6c2906af44.png", UriKind.RelativeOrAbsolute);
        //    main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        //}
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

            PlayerModel.OffsetOwn -= 1; ;
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
                int value;
                value = rnds.Next(0, PlayerModel.SearchAudios.Count);
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
            foreach (var audio in PlayerModel.SearchAudios)
            {
                if (audio.Artist + " - " + audio.Title == main.SelectedItem)
                {
                    PlayerModel.OffsetSearch = main.SelectedIndex;
                }
            }

        while (PlayerModel.Audio[PlayerModel.OffsetSearch].Url == null)
        {
            if (isback)
                PlayerModel.OffsetSearch--;
            else
                PlayerModel.OffsetSearch++;
        }
        PlayerModel.Player.URL = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Url.ToString();
        main.Artist = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Artist;
        main.Title = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Title;
        for (int i = 0; i < main.MusicList.Items.Count; i++)
        {
            string str = main.MusicList.Items[i].ToString();
            string str2 = PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Artist + " - " +
                PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Title;
            if (main.MusicList.Items[i].ToString() == PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Artist
                + " - " + PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Title)
            {
                main.SelectedIndex = i;
                break;
            }
        }

        PlayerModel.Player.controls.play();


        //try
        //{
        //    var uriImageSource = new Uri(PlayerModel.SearchAudios[PlayerModel.OffsetSearch].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
        //    main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        //}
        //catch
        //{
        //    var uriImageSource = new Uri("https://s8.hostingkartinok.com/uploads/images/2019/04/ba91888882438a65608f0f6c2906af44.png", UriKind.RelativeOrAbsolute);
        //    main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        //}
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


