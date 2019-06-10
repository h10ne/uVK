using System;
using System.IO;

namespace uVK.Model
{
    public static class SettingsModel
    {
        public static void Logout()
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin");
            System.Windows.Forms.Application.Restart();
            System.Environment.Exit(1);
        }    
    }
}
