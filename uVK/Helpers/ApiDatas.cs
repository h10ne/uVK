using System.Collections.Generic;
using VkNet;

namespace uVK.Helpers
{
    public static class ApiDatas
    {
        public static VkApi Api;
        public static bool IsAuth = false;
        public static List<VkNet.Model.Attachments.Audio> Audio { get; set; }
    }
}