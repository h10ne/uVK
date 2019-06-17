using System;
using System.IO;
using System.Windows;

namespace uVK.Model
{
    public static class SettingsModel
    {
        public static void Logout()
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin");
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }    
    }
}
