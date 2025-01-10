using System.Text;

namespace TimeSpace
{
    public class Monster
    {
        public string MapName { get; private set; }
        public int Vnum { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Wave { get; set; }
        public bool AsTarget { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public Monster(string mapName)
        {
            MapName = mapName;
        }

        public static Monster CreateFromGridRow(DataGridViewRow row, string mapName)
        {
            Monster monster = null;
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

                monster = new Monster(mapName)
                {
                    Vnum = int.Parse(vnumStr),
                    X = int.Parse(xStr),
                    Y = int.Parse(yStr),
                    AsTarget = Convert.ToBoolean(row.Cells["AsTarget"]?.Value)
                };

                // Parse attributes
                var attributesCell = row.Cells["Attributes"].Value?.ToString();
                if (!string.IsNullOrEmpty(attributesCell))
                {
                    monster.ParseAttributes(attributesCell);
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
            return monster;
        }

        private void ParseAttributes(string attributesCell)
        {
            var attributes = attributesCell.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var attribute in attributes)
            {
                var parts = attribute.Split('=');
                if (parts.Length == 2)
                {
                    Attributes[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        public string GenerateMonsterScript(DataGridViewRow row)
        {
            int random = new Random().Next(0, 7);
            var script = new StringBuilder();
            script.Append($"Monster.CreateWithVnum({row.Cells["Vnum"].Value}).At({row.Cells["X"].Value}, {row.Cells["Y"].Value}).Facing({random})");
            bool shouldAddAsTarget = Convert.ToBoolean(row.Cells["AsTarget"].Value);
            if (shouldAddAsTarget) script.Append($".AsTarget()");
            foreach (var attr in Attributes)
            {
                AppendAttributeToScript(script, attr, row);
            }
            
            return script.ToString();
        }

        private void AppendAttributeToScript(StringBuilder script, KeyValuePair<string, string> attr, DataGridViewRow row)
        {  
            switch (attr.Key)
            {
                case "SpawnAfterMobsKilled":
                case "WithCustomLevel":
                    script.Append($".{attr.Key}({attr.Value})");
                    break;
                case "SpawnAfterTaskStart":
                case "OnThreeFourthsHP":
                case "OnHalfHp":
                case "OnQuarterHp":
                    script.Append($".{attr.Key}()");
                    break;
            }
        }
    }
}