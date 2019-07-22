using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace uVK.Helpers
{
    public static class DesSer
    {
        public static void Serialize(UserDatasToSerialize datas)
        {
            IFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\");
            using (Stream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin", FileMode.Create))
            {
                formatter.Serialize(stream, datas);
            }
        }
        public static void Deserialize(ref UserDatasToSerialize datas)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin", FileMode.Open))
            {
                datas = (UserDatasToSerialize)formatter.Deserialize(stream);
            }
        }
    }
}
