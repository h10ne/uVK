using System;
using System.Threading;
using uVK.Styles.Controls;

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

    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        State.SetAudioInfo(main);
    }

    public void AudioMenuClick(uVK.MainWindow main)
    {
        State.AudioMenuClick(main);
    }

}

interface IState
{
    void NextSong(uVK.MainWindow main);
    void PrevSong(uVK.MainWindow main);
    void SetAudioInfo(uVK.MainWindow main, bool isback = false);
    void AudioMenuClick(uVK.MainWindow main);
}
/*
class IdAudios : IState
{
    public void AudioMenuClick(uVK.MainWindow main)
    {
        foreach (var audio in main.vkDatas.IdAudios)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                main.api.Audio.Add(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
            }
    }

    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        if (main.LbxMenu.SelectedIndex == -1)
            main.LbxMenu.SelectedItem = 0;
        foreach (var audio in main.vkDatas.IdAudios)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
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
                    main.LbxMenu.SelectedIndex -= 1;
                    SetAudioInfo(main, true);
                }
                else
                {
                    main.LbxMenu.SelectedIndex += 1;
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
            main.LbxMenu.SelectedIndex = value;
            Thread.Sleep(270);
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.LbxMenu.SelectedIndex += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.LbxMenu.SelectedIndex = 0;
                SetAudioInfo(main);
            }
        }
    }

    public void PrevSong(uVK.MainWindow main)
    {
        try
        {


            if (main.LbxMenu.SelectedIndex <= -1)
            {
                main.LbxMenu.SelectedIndex = main.LbxMenu.Items.Count;
            }
            else
                main.LbxMenu.SelectedIndex -= 1;
            SetAudioInfo(main, true);
        }
        catch (Exception ex)
        {
            main.LbxMenu.SelectedIndex = 4998;
            SetAudioInfo(main, true);
        }
    }
}
*/
class OwnAudios : IState
{
    public void AudioMenuClick(uVK.MainWindow main)
    {
        foreach (var audio in main.vkDatas.Audio)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                main.api.Audio.Delete(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
            }
    }

    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        try
        {
            if (main.vkDatas._offset == -1)
                throw new Exception();
            foreach (var audio in main.vkDatas.Audio)
                if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
                {
                    main.vkDatas._offset = main.LbxMenu.SelectedIndex;
                    bool th = false;
                    while (main.vkDatas.Audio[main.vkDatas._offset].Url == null)
                    {
                        if (isback)
                            main.vkDatas._offset--;
                        else
                            main.vkDatas._offset++;
                        th = true;
                    }
                    if (th) throw new Exception("1");
                    //main.music_picture.Source = audio.
                    main.player.URL = audio.Url.ToString();
                    main.PlayerControl.artist_name.Text = audio.Artist;
                    main.PlayerControl.title_name.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }

        }
        catch
        {
            Thread.Sleep(270);
            if (main.vkDatas._offset == -1)
                main.vkDatas._offset++;
            main.player.settings.volume = (int) main.PlayerControl.volume_bar.Volume;
            main.player.URL = main.vkDatas.Audio[main.vkDatas._offset].Url.ToString();
            main.PlayerControl.artist_name.Text = main.vkDatas.Audio[main.vkDatas._offset].Artist;
            main.PlayerControl.title_name.Text = main.vkDatas.Audio[main.vkDatas._offset].Title;
            main.duration_timer.Start();
            main.PlayerControl.duration_bar.Value = 0;
            main.AddAudioToList(main.vkDatas.Audio);
            main.LbxMenu.SelectedIndex = main.vkDatas._offset;
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
            int value = rnds.Next(0, int.Parse(main.api.Audio.GetCount(main.vkDatas.user_id).ToString()) - 1);
            main.LbxMenu.SelectedIndex = value;
            Thread.Sleep(270);
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.LbxMenu.SelectedIndex += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.LbxMenu.SelectedIndex = 0;
                SetAudioInfo(main);
            }
        }
    }

    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.LbxMenu.SelectedIndex -= 1;
            if (main.LbxMenu.SelectedIndex == -1)
                main.LbxMenu.SelectedIndex = int.Parse(main.api.Audio.GetCount(main.vkDatas.user_id).ToString()) - 1;
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
    public void AudioMenuClick(uVK.MainWindow main)
    {
        foreach (var audio in main.vkDatas.SearchAudios)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                main.api.Audio.Add(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
            }
    }
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.LbxMenu.SelectedIndex -= 1;
            SetAudioInfo(main);
        }
        catch
        {
            main.LbxMenu.SelectedIndex = main.LbxMenu.Items.Count - 1;
            SetAudioInfo(main, true);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.vkDatas.SearchAudios != null)
        {
            if (main.VkBools.random)
            {
                Random rnds = new Random();
                int value;
                value = rnds.Next(0, main.LbxMenu.Items.Count);
                main.LbxMenu.SelectedIndex = value;
                SetAudioInfo(main);
            }
            else
            {
                try
                {
                    main.LbxMenu.SelectedIndex += 1;
                    SetAudioInfo(main);
                }
                catch
                {
                    main.LbxMenu.SelectedIndex = 0;
                    SetAudioInfo(main);
                }
            }
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        foreach (var audio in main.vkDatas.SearchAudios)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                if (audio.Url != null)
                {
                    main.player.URL = audio.Url.ToString();
                    main.PlayerControl.artist_name.Text = audio.Artist;
                    main.PlayerControl.title_name.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
                else if (isback)
                {
                    main.LbxMenu.SelectedIndex -= 1;
                    SetAudioInfo(main, true);
                }
                else
                {
                    main.LbxMenu.SelectedIndex += 1;
                    SetAudioInfo(main, false);
                }
            }
        //if (main.VkBools.isBlack)
        //    main.play_pause_btn.Image = Resource1.pause_white;
        //else
        //    main.play_pause_btn.Image = Resource1.pause;
        //main.VkBools.isPlay = true;
    }
}

class RecommendedAudio : IState
{
    public void AudioMenuClick(uVK.MainWindow main)
    {
        foreach (var audio in main.vkDatas.RecommendedAudio)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                main.api.Audio.Add(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
            }
    }
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.LbxMenu.SelectedIndex -= 1;
            SetAudioInfo(main, true);
        }
        catch
        {
            main.LbxMenu.SelectedIndex = main.LbxMenu.Items.Count - 1;
            SetAudioInfo(main, true);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.VkBools.random)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, main.LbxMenu.Items.Count);
            main.LbxMenu.SelectedIndex = value;
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.LbxMenu.SelectedIndex += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.LbxMenu.SelectedIndex = 0;
                SetAudioInfo(main);
            }
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        if (main.LbxMenu.SelectedIndex == -1)
            main.LbxMenu.SelectedIndex = 0;
        foreach (var audio in main.vkDatas.RecommendedAudio)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                if (audio.Url != null)
                {
                    main.player.URL = audio.Url.ToString();
                    main.PlayerControl.artist_name.Text = audio.Artist;
                    main.PlayerControl.title_name.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
                else if (isback)
                {
                    main.LbxMenu.SelectedIndex -= 1;
                    SetAudioInfo(main, true);
                }
                else
                {
                    main.LbxMenu.SelectedIndex += 1;
                    SetAudioInfo(main, false);
                }
            }
        //if (main.VkBools.isBlack)
        //    main.play_pause_btn.Image = Resource1.pause_white;
        //else
        //    main.play_pause_btn.Image = Resource1.pause;
        //main.VkBools.isPlay = true;
    }
}


class HotAudio : IState
{
    public void AudioMenuClick(uVK.MainWindow main)
    {
        foreach (var audio in main.vkDatas.HotAudios)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                main.api.Audio.Add(audio.Id.GetValueOrDefault(), audio.OwnerId.GetValueOrDefault());
            }
    }
    public void PrevSong(uVK.MainWindow main)
    {
        try
        {

            main.LbxMenu.SelectedIndex -= 1;
            SetAudioInfo(main, true);
        }
        catch
        {
            main.LbxMenu.SelectedIndex = main.LbxMenu.Items.Count - 1;
            SetAudioInfo(main, true);
        }
    }

    public void NextSong(uVK.MainWindow main)
    {
        if (main.VkBools.random)
        {
            Random rnds = new Random();
            int value = rnds.Next(0, main.LbxMenu.Items.Count);
            main.LbxMenu.SelectedIndex = value;
            SetAudioInfo(main);
        }
        else
        {
            try
            {
                main.LbxMenu.SelectedIndex += 1;
                SetAudioInfo(main);
            }
            catch
            {
                main.LbxMenu.SelectedIndex = 0;
                SetAudioInfo(main);
            }
        }
    }
    public void SetAudioInfo(uVK.MainWindow main, bool isback = false)
    {
        foreach (var audio in main.vkDatas.HotAudios)
            if (audio.Artist + " - " + audio.Title == main.LbxMenu.SelectedItem.ToString())
            {
                if (audio.Url != null)
                {
                    main.player.URL = audio.Url.ToString();
                    main.PlayerControl.artist_name.Text = audio.Artist;
                    main.PlayerControl.title_name.Text = audio.Title;
                    main.player.controls.play();
                    break;
                }
                else if (isback)
                {
                    main.LbxMenu.SelectedIndex -= 1;
                    SetAudioInfo(main, true);
                }
                else
                {
                    main.LbxMenu.SelectedIndex += 1;
                    SetAudioInfo(main, false);
                }
            }
        //if (main.VkBools.isBlack)
        //    main.play_pause_btn.Image = Resource1.pause_white;
        //else
        //    main.play_pause_btn.Image = Resource1.pause;
        //main.VkBools.isPlay = true;
    }
}