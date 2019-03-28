using System;
using System.Drawing;
using System.Threading;
using System.Windows.Media;

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
                    bool th = false;
                    while (main.vkDatas.Audio[main.vkDatas.OffsetOwn].Url == null)
                    {
                        if (isback)
                            main.vkDatas.OffsetOwn--;
                        else
                            main.vkDatas.OffsetOwn++;
                        th = true;
                    }
                    if (th) throw new Exception("1");
                    main.player.URL = audio.Url.ToString();
                    main.MusicArtist.Text = audio.Artist;
                    main.MusicName.Text = audio.Title;
                    main.player.controls.play();                    
                    break;
                }
            }
        else
        {
            main.player.URL = main.vkDatas.Audio[main.vkDatas.OffsetOwn].Url.ToString();
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
        }

        try
        {
            var uriImageSource = new Uri(main.vkDatas.Audio[main.vkDatas.OffsetOwn].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
        catch
        {
            var uriImageSource = new Uri("https://raw.githubusercontent.com/dr0b99/uVK/master/uVK/Images/ImageMusic.png", UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.RandomAudioButton.IsChecked.Value)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, int.Parse(main.api.Audio.GetCount(main.vkDatas.user_id).ToString()) - 1);
            main.MusicList.SelectedIndex = value;
            Thread.Sleep(270);
            main.vkDatas.OffsetOwn += 1;
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
                main.vkDatas.OffsetOwn += 1;
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
                main.vkDatas.OffsetOwn = int.Parse(main.api.Audio.GetCount(main.vkDatas.user_id).ToString()) - 1;
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
                    bool th = false;
                    while (main.vkDatas.Audio[main.vkDatas.OffsetSearch].Url == null)
                    {
                        if (isback)
                            main.vkDatas.OffsetSearch--;
                        else
                            main.vkDatas.OffsetSearch++;
                        th = true;
                    }
                    if (th) throw new Exception("1");
                    main.player.URL = audio.Url.ToString();
                    main.MusicArtist.Text = audio.Artist;
                    main.MusicName.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
            }
        else
        {
            main.player.URL = main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Url.ToString();
            main.MusicArtist.Text = main.vkDatas.Audio[main.vkDatas.OffsetSearch].Artist;
            main.MusicName.Text = main.vkDatas.Audio[main.vkDatas.OffsetSearch].Title;
            for (int i = 0; i < main.MusicList.Items.Count; i++)
                if (main.MusicList.Items[i].ToString() == main.vkDatas.Audio[main.vkDatas.OffsetSearch].Artist + " - " + main.vkDatas.Audio[main.vkDatas.OffsetOwn].Title)
                {
                    string str = main.MusicList.Items[i].ToString();
                    main.MusicList.SelectedIndex = i;
                    break;
                }
            main.player.controls.play();
        }

        try
        {
            var uriImageSource = new Uri(main.vkDatas.SearchAudios[main.vkDatas.OffsetSearch].Album.Cover.Photo135, UriKind.RelativeOrAbsolute);
            main.MusicImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(uriImageSource);
        }
        catch
        {
            var uriImageSource = new Uri("https://raw.githubusercontent.com/dr0b99/uVK/master/uVK/Images/ImageMusic.png", UriKind.RelativeOrAbsolute);
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
        if (main.MusicList.SelectedIndex == -1)
            main.MusicList.SelectedIndex = 0;
        foreach (var audio in main.vkDatas.RecommendedAudio)
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


