using Microsoft.Extensions.DependencyInjection;

public class Switches
{
    public bool IsSearch { get; set; } = false;
    public bool IsHot { get; set; } = false;
    public bool IsRecommend { get; set; } = false;
    public bool IsOwn { get; set; } = true;
    public bool IsId { get; set; } = false;
    public Switches()
    {
        IsSearch  = false;
        IsHot = false;
        IsRecommend  = false;
        IsOwn = true;
        IsId  = false;
    }
}

public class VkDatas
{
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> SearchAudios { get; set; }
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> Audio { get; set; }
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> RecommendedAudio { get; set; }
    public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> IdAudios { get; set; }
    public System.Collections.Generic.IEnumerable<VkNet.Model.Attachments.Audio> HotAudios { get; set; }
    public long user_id { get; set; }
    public ServiceCollection service { get; set; }
    public int OffsetOwn = 0;
    public int OffsetSearch = 0;
    public int OffsetHot = -1;
    public int OffsetRecom = -1;
    public VkDatas()
    {

    }
}