using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class TimeSpaceConfig
{
    [YamlMember(Alias = "ts_id")]
    public int TsId { get; set; }

    [YamlMember(Alias = "min_level")]
    public int MinLevel { get; set; }

    [YamlMember(Alias = "max_level")]
    public int MaxLevel { get; set; }

    [YamlMember(Alias = "seeds_of_power_required")]
    public int SeedsOfPowerRequired { get; set; }

    [YamlMember(Alias = "is_hero")]
    public bool IsHero { get; set; }

    [YamlMember(Alias = "is_special")]
    public bool IsSpecial { get; set; }

    [YamlMember(Alias = "is_hidden")]
    public bool IsHidden { get; set; }

    [YamlMember(Alias = "min_players")]
    public int MinPlayers { get; set; }

    [YamlMember(Alias = "max_players")]
    public int MaxPlayers { get; set; }

    public TimeSpaceRewards Rewards { get; set; }
    public List<TimeSpacePlacement> Placement { get; set; }
}
