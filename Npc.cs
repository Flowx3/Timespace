using System.Text;

public class Npc
{
    public string MapName { get; set; }
    public int Vnum { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string EventName { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    public Npc(string mapName)
    {
        MapName = mapName;
    }

    public string GenerateNpcScript()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Npc.Create({Vnum})");
        sb.AppendLine($"    .SetPosition({X}, {Y})");
        if (!string.IsNullOrEmpty(EventName))
        {
            sb.AppendLine($"    .SetEvent(\"{EventName}\")");
        }
        foreach (var attr in Attributes)
        {
            sb.AppendLine($"    .WithAttribute(\"{attr.Key}\", \"{attr.Value}\")");
        }
        sb.AppendLine($"    .Spawn(\"{MapName}\");");
        return sb.ToString();
    }
}
