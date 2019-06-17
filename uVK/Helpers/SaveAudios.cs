using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace uVK.Helpers
{
    public class StructSaveAudios
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

    public static class SaveAudios
    {
        public static List<StructSaveAudios> Audio { get; private set; }
        public static void AddCache()
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
}
