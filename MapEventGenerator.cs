using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace TimeSpace
{
    public class MapEventGenerator
    {
        private readonly Dictionary<string, StringBuilder> mapScripts = new Dictionary<string, StringBuilder>();
        private readonly List<Monster> monsterEvents = new List<Monster>();
        private readonly NumericUpDown waveDelayInput;
        private readonly Dictionary<string, List<string>> eventManagerScripts;

        public MapEventGenerator(NumericUpDown waveDelayInput, Dictionary<string, List<string>> eventManagerScripts)
        {
            this.waveDelayInput = waveDelayInput;
            this.eventManagerScripts = eventManagerScripts;
        }

        public void GenerateEvents(List<CustomTabPage> mapTabs)
        {
            mapScripts.Clear();
            monsterEvents.Clear();

            foreach (var tab in mapTabs)
            {
                ProcessMapTab(tab);
            }

            // Generate final output
            var finalScript = new StringBuilder();
            foreach (var mapScript in mapScripts.Values)
            {
                finalScript.Append(mapScript.ToString());
            }
            File.WriteAllText("CombinedEvents.lua", finalScript.ToString());
        }

        private void ProcessMapTab(CustomTabPage tab)
        {
            string mapName = tab.MapName;
            EnsureMapScriptExists(mapName);

            // Process Monsters
            ProcessMonsters(tab.MonsterDataGridView, mapName);

            // Process Objects
            ProcessObjects(tab.Objects, mapName);

            // Process Map Join Events
            ProcessMapJoinEvents(mapName);
        }

        private void ProcessMonsters(DataGridView monsterDataGridView, string mapName)
        {
            var waves = new Dictionary<int, List<string>>();
            var nonWaveMonsters = new List<string>();

            foreach (DataGridViewRow row in monsterDataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                int wave = 0;
                var monster = Monster.CreateFromGridRow(row, mapName);
                if (monster == null) continue;

                var monsterScript = monster.GenerateMonsterScript(row);
                if (monsterDataGridView.Columns.Contains("Wave"))
                {
                    int.TryParse(row?.Cells?["Wave"]?.Value?.ToString() ?? "0", out wave);
                }
                if (wave > 0)
                {
                    if (!waves.ContainsKey(wave))
                        waves[wave] = new List<string>();
                    waves[wave].Add(monsterScript);
                }
                else
                {
                    nonWaveMonsters.Add(monsterScript);
                }

                monsterEvents.Add(monster);
            }

            // Generate monster scripts
            if (nonWaveMonsters.Any())
            {
                mapScripts[mapName].AppendLine($"{mapName}.AddMonsters({{");
                mapScripts[mapName].AppendLine(string.Join(", \n", nonWaveMonsters.Select(m => $"    {m}")));
                mapScripts[mapName].AppendLine("})");
            }

            if (waves.Any())
            {
                mapScripts[mapName].AppendLine($"{mapName}.AddMonsterWaves({{");
                foreach (var wave in waves.OrderBy(w => w.Key))
                {
                    var delay = wave.Key * (int)waveDelayInput.Value;
                    mapScripts[mapName].AppendLine($"    -- wave {wave.Key}");
                    mapScripts[mapName].AppendLine($"    MonsterWave.CreateWithDelay({delay}).WithMonsters({{");
                    mapScripts[mapName].AppendLine(string.Join(", \n", wave.Value.Select(m => $"        {m}")));
                    mapScripts[mapName].AppendLine("    }),");
                }
                mapScripts[mapName].AppendLine("})");
            }
        }

        private void ProcessObjects(List<MapObject> objects, string mapName)
        {
            var validObjects = objects
                .Where(obj => !string.IsNullOrEmpty(obj.ObjectType) &&
                             obj.GetX() != 1500 &&
                             obj.GetY() != 1500)
                .Select(obj => obj.GenerateObjectiveScript())
                .Where(script => !string.IsNullOrEmpty(script))
                .ToList();

            if (validObjects.Any())
            {
                mapScripts[mapName].AppendLine($"{mapName}.AddObjects({{");
                mapScripts[mapName].AppendLine(string.Join(", \n", validObjects));
                mapScripts[mapName].AppendLine("})");
            }
        }

        private void ProcessMapJoinEvents(string mapName)
        {
            mapScripts[mapName].AppendLine($"{mapName}.OnMapJoin({{");
            mapScripts[mapName].AppendLine($"    Event.TryStartTaskForMap({mapName}),");

            if (eventManagerScripts.ContainsKey(mapName))
            {
                foreach (var script in eventManagerScripts[mapName])
                {
                    mapScripts[mapName].AppendLine(script);
                }
            }

            mapScripts[mapName].AppendLine("})");
        }

        private void EnsureMapScriptExists(string mapName)
        {
            if (!mapScripts.ContainsKey(mapName))
            {
                mapScripts[mapName] = new StringBuilder();
            }
        }
    }
}