using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
public static class Switches
{
    public static bool IsSearch { get; set; } = false;
    public static bool IsHot { get; set; } = false;
    public static bool IsRecommend { get; set; } = false;
    public static bool IsOwn { get; set; } = true;
    public static bool IsId { get; set; } = false;
}

public static class Des_Ser
{
    public static void Serialize(uVK.UserDatasToSerialize datas)
    {
        IFormatter formatter = new BinaryFormatter();
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\");
        using (Stream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin", FileMode.Create))
        {
            formatter.Serialize(stream, datas);
        }
    }
    static public void Deserialize(ref uVK.UserDatasToSerialize datas)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin", FileMode.Open))
        {
            datas = (uVK.UserDatasToSerialize)formatter.Deserialize(stream);
        }
    }
}