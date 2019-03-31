using System;
using System.Diagnostics;
using System.Threading;

class Playlist
{
    public IState State { get; set; }
    public Playlist(IState ws)
    {
        State = ws;
    }
    public void NextSong(uVK.MainWindow main)
    {
        State.NextSong(main);
    }

    public void PrevSong(uVK.MainWindow main)
    {
        State.PrevSong(main);
    }

    public void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false)
    {
        State.SetAudioInfo(main, isback, fromClick);
    }

}

interface IState
{
    void NextSong(uVK.MainWindow main);
    void PrevSong(uVK.MainWindow main);
    void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false);
}
/*
class IdAudios : IState
{
    public void AudioMenuClick(uVK.MainWindow main)
    {
        foreach (var audio in main.vkDatas.IdAudios)
            if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
            {
                main.api.Audio.Add(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
            }
    }

    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        if (main.MusicList.SelectedIndex == -1)
            main.MusicList.SelectedItem = 0;
        foreach (var audio in main.vkDatas.IdAudios)
            if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
            {
                if (audio.Url != null)
                {
                    main.player.URL = audio.Url.ToString();
                    main.artist_name.Text = audio.Artist;
                    main.title_name.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
                else if (isback)
                {
                    main.MusicList.SelectedIndex -= 1;
                    SetAudioInfo(main, true);
                }
                else
                {
                    main.MusicList.SelectedIndex += 1;
                    SetAudioInfo(main, false);
                }
            }
        //if (main.VkBools.isBlack)
        //    main.play_pause_btn.Image = Resource1.pause_white;
        //else
        //    main.play_pause_btn.Image = Resource1.pause;
        //main.VkBools.isPlay = true;
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.VkBools.random)
        {
            Random rnds = new Random();
            int rnd_max = int.Parse(main.api.Audio.GetCount(long.Parse(main.IdSongs_box.Text)).ToString());
            if (rnd_max > 1800)
                rnd_max = 1800;
            int value = rnds.Next(0, rnd_max - 1);
            main.MusicList.SelectedIndex = value;
            Thread.Sleep(270);
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.MusicList.SelectedIndex += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.MusicList.SelectedIndex = 0;
                SetAudioInfo(main);
            }
        }
    }

    public void PrevSong(uVK.MainWindow main)
    {
        try
        {


            if (main.MusicList.SelectedIndex <= -1)
            {
                main.MusicList.SelectedIndex = main.MusicList.Items.Count;
            }
            else
                main.MusicList.SelectedIndex -= 1;
            SetAudioInfo(main, true);
        }
        catch (Exception ex)
        {
            main.MusicList.SelectedIndex = 4998;
            SetAudioInfo(main, true);
        }
    }
}
*/

public class SavesAudios:IState
{
    public void NextSong(uVK.MainWindow main)
    {
        if (main.RandomAudioButton.IsChecked.Value)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, main.vkDatas.Cache.Audio.Count);
            Thread.Sleep(270);
            Debug.Print(value.ToString());
            main.vkDatas.OffsetSave = value;
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.vkDatas.OffsetSave += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.vkDatas.OffsetSave = 1;
                SetAudioInfo(main);
            }
        }
    }
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.vkDatas.OffsetSave -= 1; ;
            if (main.vkDatas.OffsetSave == -1)
                main.vkDatas.OffsetSave = main.vkDatas.Cache.Audio.Count - 1;
            SetAudioInfo(main, true);
        }
        catch
        {
            SetAudioInfo(main, true);
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false)
    {
        if (fromClick)
            foreach (var audio in main.vkDatas.Cache.Audio)
            {
                if (audio.Artist + " - " + audio.Title == main.SaveMusic.SelectedItem.ToString())
                {
                    main.vkDatas.OffsetSave = main.SaveMusic.SelectedIndex;
                    main.player.URL = audio.Url.ToString();
                    main.MusicArtist.Text = audio.Artist;
                    main.MusicName.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
            }
        else
        {
            main.player.URL = main.vkDatas.Cache.Audio[main.vkDatas.OffsetSave].Url.ToString();
            main.MusicArtist.Text = main.vkDatas.Cache.Audio[main.vkDatas.OffsetSave].Artist;
            main.MusicName.Text = main.vkDatas.Cache.Audio[main.vkDatas.OffsetSave].Title;
            for (int i = 0; i < main.SaveMusic.Items.Count; i++)
            {
                string saveitem = main.SaveMusic.Items[i].ToString();
                if (main.SaveMusic.Items[i].ToString() == main.vkDatas.Cache.Audio[main.vkDatas.OffsetSave].Artist + " - " + main.vkDatas.Cache.Audio[main.vkDatas.OffsetSave].Title)
                {
                    string str = main.SaveMusic.Items[i].ToString();
                    main.SaveMusic.SelectedIndex = i;
                    break;
                }
            }
            main.player.controls.play();

        }
        var uriImageSource = new Uri("https://psv4.userapi.com/c848020/u279747195/docs/d8/f81b6ea0493b/ImageMusic.png?extra=eAix6htvrxG4hUmiCSjYZFxb05FSZFEuJMjSpZXm5a3QVGsK6OOUkCOSYmjwrnV0VoILNas2Rf3ZN0M3QQNRTCG-39Ff_lkWs28baALlQGZaCkQLoLejYgpYKhoqnmuCYjABlFLmzm-zOWJ4CIPMm-2Q9PU", UriKind.RelativeOrAbsolute);
        main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);

    }
}
class OwnAudios : IState
{

    public void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false)
    {
        if (fromClick)
            foreach (var audio in main.vkDatas.Audio)
            {
                if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
                {
                    main.vkDatas.OffsetOwn = main.MusicList.SelectedIndex;
                    break;
                }
            }
            while (main.vkDatas.Audio[main.vkDatas.OffsetOwn].Url == null)
            {
                if (isback)
                    main.vkDatas.OffsetOwn--;
                else
                    main.vkDatas.OffsetOwn++;
            }
        var url = main.vkDatas.Audio[main.vkDatas.OffsetOwn].Url;
        string path = url.Scheme + "://" + url.Authority + url.AbsolutePath;
        main.player.URL = url.Scheme + "://" + url.Authority + url.AbsolutePath;
        main.MusicArtist.Text = main.vkDatas.Audio[main.vkDatas.OffsetOwn].Artist;
        main.MusicName.Text = main.vkDatas.Audio[main.vkDatas.OffsetOwn].Title;
        for (int i = 0; i < main.MusicList.Items.Count; i++)
            if (main.MusicList.Items[i].ToString() == main.vkDatas.Audio[main.vkDatas.OffsetOwn].Artist + " - " + main.vkDatas.Audio[main.vkDatas.OffsetOwn].Title)
            {
                string str = main.MusicList.Items[i].ToString();
                main.MusicList.SelectedIndex = i;
                break;
            }
        main.player.controls.play();

        try
        {
            var uriImageSource = new Uri(main.vkDatas.Audio[main.vkDatas.OffsetOwn].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
        catch
        {
            var uriImageSource = new Uri("https://psv4.userapi.com/c848020/u279747195/docs/d8/f81b6ea0493b/ImageMusic.png?extra=eAix6htvrxG4hUmiCSjYZFxb05FSZFEuJMjSpZXm5a3QVGsK6OOUkCOSYmjwrnV0VoILNas2Rf3ZN0M3QQNRTCG-39Ff_lkWs28baALlQGZaCkQLoLejYgpYKhoqnmuCYjABlFLmzm-zOWJ4CIPMm-2Q9PU", UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.RandomAudioButton.IsChecked.Value)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, main.vkDatas.Audio.Count - 1);
            Thread.Sleep(270);
            main.vkDatas.OffsetOwn = value;
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.vkDatas.OffsetOwn += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.vkDatas.OffsetOwn = 0;
                SetAudioInfo(main);
            }
        }
    }

    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.vkDatas.OffsetOwn -= 1; ;
            if (main.vkDatas.OffsetOwn == -1)
                main.vkDatas.OffsetOwn = main.vkDatas.Audio.Count-1;
            SetAudioInfo(main, true);
        }
        catch
        {
            SetAudioInfo(main, true);
        }
    }
}

class SearchAudios : IState
{
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.vkDatas.OffsetSearch -= 1;
            SetAudioInfo(main);
        }
        catch
        {
            main.vkDatas.OffsetSearch = main.vkDatas.SearchAudios.Count - 1;
            SetAudioInfo(main, true);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.vkDatas.SearchAudios != null)
        {
            if (main.RandomAudioButton.IsChecked.Value)
            {
                Random rnds = new Random();
                int value;
                value = rnds.Next(0, main.vkDatas.SearchAudios.Count);
                main.vkDatas.OffsetSearch = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    main.vkDatas.OffsetSearch += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    main.vkDatas.OffsetSearch = 0;
                    SetAudioInfo(main);
                }
            }
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false)
    {
        if (fromClick)
            foreach (var audio in main.vkDatas.SearchAudios)
            {
                if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
                {
                    main.vkDatas.OffsetSearch = main.MusicList.SelectedIndex;
                }
            }

        while (main.vkDatas.Audio[main.vkDatas.OffsetSearch].Url == null)
        {
            if (isback)
                main.vkDatas.OffsetSearch--;
            else
                main.vkDatas.OffsetSearch++;
        }
        main.player.URL = main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Url.ToString();
        main.MusicArtist.Text = main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Artist;
        main.MusicName.Text = main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Title;
        for (int i = 0; i < main.MusicList.Items.Count; i++)
        {
            string str = main.MusicList.Items[i].ToString();
            string str2 = main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Artist + " - " +
                main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Title;
            if (main.MusicList.Items[i].ToString() == main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Artist
                + " - " + main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Title)
            {
                main.MusicList.SelectedIndex = i;
                break;
            }
        }

        main.player.controls.play();


        try
        {
            var uriImageSource = new Uri(main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
        catch
        {
            var uriImageSource = new Uri("https://psv4.userapi.com/c848020/u279747195/docs/d8/f81b6ea0493b/ImageMusic.png?extra=eAix6htvrxG4hUmiCSjYZFxb05FSZFEuJMjSpZXm5a3QVGsK6OOUkCOSYmjwrnV0VoILNas2Rf3ZN0M3QQNRTCG-39Ff_lkWs28baALlQGZaCkQLoLejYgpYKhoqnmuCYjABlFLmzm-zOWJ4CIPMm-2Q9PU", UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
    }
}

class RecommendedAudio : IState
{
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.vkDatas.OffsetRecom -= 1; ;
            if (main.vkDatas.OffsetRecom == -1)
                main.vkDatas.OffsetRecom = main.vkDatas.RecommendedAudio.Count-1;
            SetAudioInfo(main, true);
        }
        catch
        {
            SetAudioInfo(main, true);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.RandomAudioButton.IsChecked.Value)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, main.vkDatas.RecommendedAudio.Count - 1);
            Thread.Sleep(270);
            main.vkDatas.OffsetRecom = value;
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.vkDatas.OffsetRecom += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.vkDatas.OffsetRecom = 0;
                SetAudioInfo(main);
            }
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false)
    {
        if (fromClick)
            foreach (var audio in main.vkDatas.RecommendedAudio)
            {
                if (audio.Artist + " - " + audio.Title == main.RecommendationsList.SelectedItem.ToString())
                {
                    main.vkDatas.OffsetRecom = main.RecommendationsList.SelectedIndex;
                    break;
                }
            }
        while (main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Url == null)
        {
            if (isback)
                main.vkDatas.OffsetRecom--;
            else
                main.vkDatas.OffsetRecom++;
        }
        var url = main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Url;
        string path = url.Scheme + "://" + url.Authority + url.AbsolutePath;
        main.player.URL = url.Scheme + "://" + url.Authority + url.AbsolutePath;
        main.MusicArtist.Text = main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Artist;
        main.MusicName.Text = main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Title;
        for (int i = 0; i < main.MusicList.Items.Count; i++)
            if (main.RecommendationsList.Items[i].ToString() == main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Artist + " - " + main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Title)
            {
                string str = main.RecommendationsList.Items[i].ToString();
                main.RecommendationsList.SelectedIndex = i;
                break;
            }
        main.player.controls.play();

        try
        {
            var uriImageSource = new Uri(main.vkDatas.RecommendedAudio[main.vkDatas.OffsetRecom].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
        catch
        {
            var uriImageSource = new Uri("https://psv4.userapi.com/c848020/u279747195/docs/d8/f81b6ea0493b/ImageMusic.png?extra=eAix6htvrxG4hUmiCSjYZFxb05FSZFEuJMjSpZXm5a3QVGsK6OOUkCOSYmjwrnV0VoILNas2Rf3ZN0M3QQNRTCG-39Ff_lkWs28baALlQGZaCkQLoLejYgpYKhoqnmuCYjABlFLmzm-zOWJ4CIPMm-2Q9PU", UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
    }
}
class HotAudio : IState
{
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.MusicList.SelectedIndex -= 1;
            SetAudioInfo(main, true);
        }
        catch
        {
            main.MusicList.SelectedIndex = main.MusicList.Items.Count - 1;
            SetAudioInfo(main, true);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.RandomAudioButton.IsChecked.Value)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, main.MusicList.Items.Count);
            main.MusicList.SelectedIndex = value;
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.MusicList.SelectedIndex += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.MusicList.SelectedIndex = 0;
                SetAudioInfo(main);
            }
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false, bool fromClick = false)
    {
        foreach (var audio in main.vkDatas.HotAudios)
            if (audio.Artist + " - " + audio.Title == main.MusicList.SelectedItem.ToString())
            {
                if (audio.Url != null)
                {
                    main.player.URL = audio.Url.ToString();
                    main.MusicArtist.Text = audio.Artist;
                    main.MusicName.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
                else if (isback)
                {
                    main.MusicList.SelectedIndex -= 1;
                    SetAudioInfo(main, true);
                }
                else
                {
                    main.MusicList.SelectedIndex += 1;
                    SetAudioInfo(main, false);
                }
            }
    }
}


