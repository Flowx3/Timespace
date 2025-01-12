using YamlDotNet.Serialization;

public class TimeSpaceReward
{
    [YamlMember(Alias = "item_vnum")]
    public int ItemVnum { get; set; }
    public int Amount { get; set; }
}