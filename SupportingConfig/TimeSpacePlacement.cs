using YamlDotNet.Serialization;

public class TimeSpacePlacement
{
    [YamlMember(Alias = "map_id")]
    public int MapId { get; set; }

    [YamlMember(Alias = "map_x")]
    public int MapX { get; set; }

    [YamlMember(Alias = "map_y")]
    public int MapY { get; set; }
}