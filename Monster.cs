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
        public string Vnum { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string AdditionalAttribute { get; set; }
        public string AdditionalValue { get; set; }
        public bool AsTarget { get; set; }

        public Monster(string mapName)
        {
            MapName = mapName;
        }

        public string GenerateMonsterScript(DataGridViewRow row)
        {
            var script = new StringBuilder();
            script.Append($"Monster.CreateWithVnum({row.Cells["Vnum"].Value}).At({row.Cells["X"].Value}, {row.Cells["Y"].Value}).Facing(2)");
            string selectedAttribute = row.Cells["AdditionalAttribute"].Value?.ToString();
            if (!string.IsNullOrEmpty(selectedAttribute) && selectedAttribute != "none")
            {
                switch (selectedAttribute)
                {
                    case "SpawnAfterMobsKilled":
                    case "WithCustomLevel":
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        script.Append($".{selectedAttribute}({row.Cells["AdditionalValue"].Value})");
                        break;
                    case "SpawnAfterTaskStart":
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        script.Append($".{selectedAttribute}()");
                        break;
                    case "OnThreeFourthsHP":
                    case "OnHalfHp":
                    case "OnQuarterHp":
                        if (Convert.ToBoolean(row.Cells["AsTarget"].Value))
                        {
                            script.Append(".AsTarget()");
                        }
                        script.Append($".{selectedAttribute}()");
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