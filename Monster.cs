using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TimeSpace
{
    public class Monster
    {
        public string MapName { get; private set; }
        public int Vnum { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Wave { get; set; } = 1;
        public bool AsTarget { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public Monster(string mapName)
        {
            MapName = mapName;
        }

        public string GenerateMonsterScript(DataGridViewRow row)
        {
            var script = new StringBuilder();
            script.Append($"Monster.CreateWithVnum({row.Cells["Vnum"].Value}).At({row.Cells["X"].Value}, {row.Cells["Y"].Value}).Facing(2)");

            foreach (var attr in Attributes)
            {
                switch (attr.Key)
                {
                    case "SpawnAfterMobsKilled":
                    case "WithCustomLevel":
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        script.Append($".{attr.Key}({attr.Value})");
                        break;
                    case "SpawnAfterTaskStart":
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        script.Append($".{attr.Key}()");
                        break;
                    case "OnThreeFourthsHP":
                    case "OnHalfHp":
                    case "OnQuarterHp":
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        script.Append($".{attr.Key}()");
                        break;
                    default:
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        break;
                }
            }

            return script.ToString();
        }
    }
}