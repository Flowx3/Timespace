using System.Text;

public class Npc
{
    public string MapName { get; set; }
    public int Vnum { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    public Npc(string mapName)
    {
        MapName = mapName;
    }

    public static Npc CreateFromGridRow(DataGridViewRow row, string mapName)
    {
        Npc npc = null;
        try
        {
            string vnumStr = row.Cells["Vnum"].Value?.ToString();
            string xStr = row.Cells["X"].Value?.ToString();
            string yStr = row.Cells["Y"].Value?.ToString();

            if (string.IsNullOrEmpty(vnumStr))
                throw new ArgumentNullException("Vnum");
            if (string.IsNullOrEmpty(xStr))
                throw new ArgumentNullException("X");
            if (string.IsNullOrEmpty(yStr))
                throw new ArgumentNullException("Y");

            npc = new Npc(mapName)
            {
                Vnum = int.Parse(vnumStr),
                X = int.Parse(xStr),
                Y = int.Parse(yStr)
            };

            // Parse attributes
            var attributesCell = row.Cells["Attributes"].Value?.ToString();
            if (!string.IsNullOrEmpty(attributesCell))
            {
                ParseAttributes(npc, attributesCell);
            }
        }
        catch (ArgumentNullException ex)
        {
            MessageBox.Show($"Error: Missing value for '{ex.ParamName}' in map '{mapName}'.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            MessageBox.Show($"Error: Invalid value format in map '{mapName}'. Details: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return npc;
    }

    private static void ParseAttributes(Npc npc, string attributesCell)
    {
        var attributes = attributesCell.Split(',')
            .Select(attr => attr.Trim())
            .Where(attr => !string.IsNullOrEmpty(attr));

        foreach (var attribute in attributes)
        {
            var parts = attribute.Split('=');
            if (parts.Length == 2)
            {
                npc.Attributes[parts[0].Trim()] = parts[1].Trim();
            }
            else if (parts.Length == 1)
            {
                // For attributes without values (flags)
                npc.Attributes[parts[0].Trim()] = "true";
            }
        }
    }

    public string GenerateNpcScript()
    {
        int random = new Random().Next(0, 7);
        var sb = new StringBuilder();
        sb.Append($"MapNpc.CreateNpcWithVnum({Vnum})");
        sb.Append($".At({X}, {Y})");
        sb.Append($".Facing({random})");

        // Handle special NPC attributes
        foreach (var attr in Attributes)
        {
            switch (attr.Key)
            {
                case "WithMustProtectAura":
                case "WithFollowPlayer":
                    sb.Append($".{attr.Key}()");
                    break;
                case "WithCustomLevel":
                case "WithHpMultiplier":
                    sb.Append($".{attr.Key}({attr.Value})");
                    break;
                default:
                    sb.Append($".WithAttribute(\"{attr.Key}\", \"{attr.Value}\")");
                    break;
            }
        }
        return sb.ToString();
    }
}