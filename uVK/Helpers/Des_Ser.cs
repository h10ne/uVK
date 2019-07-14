using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace uVK.Helpers
{
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
}
