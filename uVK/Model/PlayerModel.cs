using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace uVK.Model
{
    public static class PlayerModel
    {
        

        //        #region Variables
        public static  VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
        public static VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> Audio { get; set; }
        public static VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> RecommendedAudio { get; set; }
        //        public SaveAudios Cache;
        //        public ServiceCollection service { get; set; }
        //        public int OffsetOwn = 0;
        //        public int OffsetSearch = 0;
        //        public int OffsetHot = -1;
        //        public int OffsetRecom = 0;
        //        public int OffsetSave = 0;
        //        #endregion
    }
}

