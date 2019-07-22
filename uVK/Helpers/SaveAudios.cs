using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace uVK.Helpers
{
    public class StructSaveAudios
    {
        public readonly string Url;
        public readonly string Artist;
        public readonly string Title;
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
            Directory.GetFiles(pathToSave, "*.*");
            List<string> filesname = Directory.GetFiles(pathToSave, "*.*").ToList();
            foreach (var name in filesname)
            {
                string[] fullPath = name.Split('\\');
                string[] audioMix = fullPath[fullPath.Length - 1].Split('↨');
                Audio.Add(new StructSaveAudios(audioMix[0], audioMix[1], name));
            }
        }
    }
}
