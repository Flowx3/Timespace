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
        private readonly Dictionary<string, List<string>> eventManagerScripts;

        public MapEventGenerator(Dictionary<string, List<string>> eventManagerScripts)
        {
            this.eventManagerScripts = eventManagerScripts;
        }

        public void GenerateEvents(List<CustomTabPage> mapTabs)
        {
            mapScripts.Clear();
            monsterEvents.Clear();

            // Generate individual event files for each tab
            foreach (var tab in mapTabs)
            {
                var mapScript = new StringBuilder();
                ProcessMapTab(tab, mapScript);
                File.WriteAllText($"Events_{tab.MapName}.lua", mapScript.ToString());
                mapScripts[tab.MapName] = mapScript;
            }

            // Generate combined output
            var finalScript = new StringBuilder();
            foreach (var mapScript in mapScripts.Values)
            {
                finalScript.Append(mapScript.ToString());
            }
            File.WriteAllText("CombinedEvents.lua", finalScript.ToString());
        }

        private void ProcessMapTab(CustomTabPage tab, StringBuilder mapScript)
        {
            string mapName = tab.MapName;

            // Process Monsters using the tab's specific wave delay
            ProcessMonsters(tab.MonsterDataGridView, mapName, tab._waveDelayInput.Value, mapScript);

            // Process Npcs
            ProcessNpcs(tab.NpcDataGridview, mapName, mapScript);

            // Process Objects
            ProcessObjects(tab.Objects, mapName, mapScript);

            // Process Map Join Events
            ProcessMapJoinEvents(mapName, mapScript);

            // Process Task Finish/Fail Events
            ProcessTaskEvents(tab, mapName, mapScript);

            // Process OnAllTargetMobsDead Events
            ProcessOnAllTargetMobsDeadEvents(tab, mapName, mapScript);
        }

        private void ProcessMonsters(DataGridView monsterDataGridView, string mapName, decimal waveDelay, StringBuilder mapScript)
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
                mapScript.AppendLine($"{mapName}.AddMonsters({{");
                mapScript.AppendLine(string.Join(", \n", nonWaveMonsters.Select(m => $"    {m}")));
                mapScript.AppendLine("})");
            }

            if (waves.Any())
            {
                mapScript.AppendLine($"{mapName}.AddMonsterWaves({{");
                foreach (var wave in waves.OrderBy(w => w.Key))
                {
                    var delay = wave.Key - 1 + " * " + waveDelay;
                    mapScript.AppendLine($"    -- wave {wave.Key}");
                    mapScript.AppendLine($"    MonsterWave.CreateWithDelay({delay}).WithMonsters({{");
                    mapScript.AppendLine(string.Join(", \n", wave.Value.Select(m => $"        {m}")));
                    mapScript.AppendLine("    }),");
                }
                mapScript.AppendLine("})");
            }
        }
        private void ProcessNpcs(DataGridView npcDataGridView, string mapName, StringBuilder mapScript)
        {
            var npcs = new List<string>();

            foreach (DataGridViewRow row in npcDataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                var npc = Npc.CreateFromGridRow(row, mapName);
                if (npc == null) continue;

                var npcScript = npc.GenerateNpcScript();
                npcs.Add(npcScript);
            }

            if (npcs.Any())
            {
                mapScript.AppendLine($"{mapName}.AddNpcs({{");
                mapScript.AppendLine(string.Join(", \n", npcs.Select(n => $"    {n}")));
                mapScript.AppendLine("})");
            }
        }
        private void ProcessObjects(List<MapObject> objects, string mapName, StringBuilder mapScript)
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
                mapScript.AppendLine($"{mapName}.AddObjects({{");
                mapScript.AppendLine(string.Join(", \n", validObjects));
                mapScript.AppendLine("})");
            }
        }

        private void ProcessMapJoinEvents(string mapName, StringBuilder mapScript)
        {
            mapScript.AppendLine($"{mapName}.OnMapJoin({{");
            mapScript.AppendLine($"    Event.TryStartTaskForMap({mapName}),");

            if (eventManagerScripts.ContainsKey(mapName))
            {
                foreach (var script in eventManagerScripts[mapName])
                {
                    mapScript.AppendLine(script);
                }
            }

            mapScript.AppendLine("})");
        }
        private void ProcessOnAllTargetMobsDeadEvents(CustomTabPage tab, string mapName, StringBuilder mapScript)
        {
            var onAllTargetMobsDeadEvents = tab.GetOnAllTargetMobsDead();

            if (onAllTargetMobsDeadEvents.Any())
            {
                mapScript.AppendLine($"{mapName}.OnAllTargetMobsDead({{");
                foreach (var evt in onAllTargetMobsDeadEvents)
                {
                    mapScript.AppendLine($"    {evt},");
                }
                mapScript.AppendLine("})");
            }
        }
        private void ProcessTaskEvents(CustomTabPage tab, string mapName, StringBuilder mapScript)
        {
            var taskFinishEvents = tab.GetTaskFinishEvents();
            var taskFailEvents = tab.GetTaskFailEvents();

            if (taskFinishEvents.Any())
            {
                mapScript.AppendLine($"{mapName}.OnTaskFinish({{");
                foreach (var evt in taskFinishEvents)
                {
                    mapScript.AppendLine($"    {evt},");
                }
                mapScript.AppendLine("})");
            }

            if (taskFailEvents.Any())
            {
                mapScript.AppendLine($"{mapName}.OnTaskFail({{");
                foreach (var evt in taskFailEvents)
                {
                    mapScript.AppendLine($"    {evt},");
                }
                mapScript.AppendLine("})");
            }
        }
    }
}