using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

class StructSaveAudios
{
    public string Url;
    public string Artist;
    public string Title;
    public StructSaveAudios(string artist, string title, string url)
    {
        Url = url;
        Artist = artist;
        Title = title;
    }
}

public class SaveAudios
{
    internal List<StructSaveAudios> Audio { get; private set; }
    public SaveAudios()
    {
        Audio = new List<StructSaveAudios>();
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\SaveAudios\\");
        string pathToSave = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\SaveAudios\\";
        string[] files2 = Directory.GetFiles(pathToSave, "*.*");
        List<string> filesname = Directory.GetFiles(pathToSave, "*.*").ToList();
        foreach (var name in filesname)
        {
            string[] fullPath = name.Split('\\');
            string[] AudioMix = fullPath[fullPath.Length - 1].Split('↨');
            Audio.Add(new StructSaveAudios(AudioMix[0], AudioMix[1], name));
        }
    }

}
public class Switches
{
    public bool IsSearch { get; set; } = false;
    public bool IsHot { get; set; } = false;
    public bool IsRecommend { get; set; } = false;
    public bool IsOwn { get; set; } = true;
    public bool IsId { get; set; } = false;
    public Switches()
    {
        IsSearch = false;
        IsHot = false;
        IsRecommend = false;
        IsOwn = true;
        IsId = false;
    }
}

public class VkDatas
{
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> Audio { get; set; }
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> RecommendedAudio { get; set; }
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> IdAudios { get; set; }
    public IEnumerable<VkNet.Model.Attachments.Audio> HotAudios { get; set; }
    public SaveAudios Cache;
    public ServiceCollection service { get; set; }
    public int OffsetOwn = 0;
    public int OffsetSearch = 0;
    public int OffsetHot = -1;
    public int OffsetRecom = 0;
    public int OffsetSave = 0;
    public uVK.UserDatas datas;
    public VkDatas()
    {
        datas = new uVK.UserDatas();
        Cache = new SaveAudios();
    }
}
public static class Des_Ser
{
    public async static void Serialize(uVK.UserDatas datas)
    {
        await Task.Run(() =>
        {
            IFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\");
            using (Stream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin", FileMode.Create))
            {
                formatter.Serialize(stream, datas);
            }
        });
    }
    static public void Deserialize(ref uVK.UserDatas datas)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin", FileMode.Open))
        {
            datas = (uVK.UserDatas)formatter.Deserialize(stream);
        }
    }
}