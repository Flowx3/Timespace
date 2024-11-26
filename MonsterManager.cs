using System.Text;
using TimeSpace;

namespace TimeSpace;
public class MonsterManager
{
    private List<Monster> MonsterEvents { get; set; } = new List<Monster>();
    private Dictionary<string, StringBuilder> mapScripts;
    private NumericUpDown waveDelayInput;

    public MonsterManager(NumericUpDown waveDelayInput)
    {
        this.waveDelayInput = waveDelayInput;
        this.mapScripts = new Dictionary<string, StringBuilder>();
    }

    public void AddMonster(string mapName, DataGridView monsterDataGridView)
    {
        var monster = new Monster(mapName);
        MonsterEvents.Add(monster);
        monsterDataGridView.Rows.Add();
    }

    public void RemoveMonster(DataGridView monsterDataGridView)
    {
        if (monsterDataGridView.Rows.Count > 0)
        {
            int lastRowIndex = monsterDataGridView.Rows.Count - 1;
            monsterDataGridView.Rows.RemoveAt(lastRowIndex);

            int lastMonsterIndex = MonsterEvents.Count - 1;
            if (lastMonsterIndex >= 0)
            {
                MonsterEvents.RemoveAt(lastMonsterIndex);
            }
        }
    }

    public Dictionary<string, StringBuilder> ProcessMonsterData(List<TabPage> mapTabs)
    {
        mapScripts.Clear();
        MonsterEvents.Clear();

        foreach (var tab in mapTabs)
        {
            ProcessTabMonsters(tab);
        }

        return mapScripts;
    }

    private void ProcessTabMonsters(TabPage tab)
    {
        var monsterDataGridView = tab.Controls.Find("monsterDataGridView", true).FirstOrDefault() as DataGridView;
        if (monsterDataGridView == null) return;

        string mapName = tab.Text; // Assuming tab.Text contains MapName

        if (!mapScripts.ContainsKey(mapName))
        {
            mapScripts[mapName] = new StringBuilder();
        }

        var waves = new Dictionary<int, List<string>>();

        foreach (DataGridViewRow row in monsterDataGridView.Rows)
        {
            if (!row.IsNewRow)
            {
                ProcessMonsterRow(row, mapName, waves);
            }
        }

        GenerateWaveScripts(waves, mapName);
    }

    private void ProcessMonsterRow(DataGridViewRow row, string mapName, Dictionary<int, List<string>> waves)
    {
        int.TryParse(row.Cells["Wave"].Value?.ToString(), out int wave);

        var monster = Monster.CreateFromGridRow(row, mapName);
        if (monster != null)
        {
            var monsterScript = monster.GenerateMonsterScript(row);
            if (!waves.ContainsKey(wave))
            {
                waves[wave] = new List<string>();
            }
            waves[wave].Add(monsterScript);
            MonsterEvents.Add(monster);
        }
    }

    private void GenerateWaveScripts(Dictionary<int, List<string>> waves, string mapName)
    {
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
}