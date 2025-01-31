﻿using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TimeSpace;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

public class TimeSpaceParser
{
    private readonly TimeSpaceTool _mainForm;
    private readonly string _filePath;
    private readonly Dictionary<string, CustomTabPage> _mapPages;
    private readonly Func<List<string>> _getMapNames;
    private List<CustomTabPage> mapTabs = TimeSpaceTool.mapTabs;
    private readonly Dictionary<string, List<string>> _eventScripts;
    private int currentLineIndex = 0;
    private Config _config;
    private ItemSearchManager _searchManager;
    private int _tsNumber;
    public TimeSpaceParser(TimeSpaceTool mainForm, string filePath, Func<List<string>> getMapNames, Config config)
    {
        _mainForm = mainForm;
        _filePath = filePath;
        _mapPages = new Dictionary<string, CustomTabPage>();
        _getMapNames = getMapNames;
        _eventScripts = new Dictionary<string, List<string>>();
        _config = config;
        _searchManager = new ItemSearchManager(config);
    }

    public async void PopulateFromFile()
    {
        string[] lines = File.ReadAllLines(_filePath);
        string currentMapName = "";
        CustomTabPage currentPage = null;
        bool isProcessingWaves = false;
        StringBuilder eventBuilder = null;
        List<string> allLines = new List<string>(lines);


        while (currentLineIndex < lines.Length)
        {
            string trimmedLine = lines[currentLineIndex].Trim();

            if (trimmedLine.StartsWith("local map_"))
            {
                ParseMapDefinition(trimmedLine, allLines);
            }
            if (trimmedLine.StartsWith("local portal_"))
            {
                ParsePortalDefinition(trimmedLine);
            }
            if (trimmedLine.StartsWith("--- Map") || trimmedLine.StartsWith("map_"))
            {
                string newMapName = "";

                if (trimmedLine.StartsWith("--- Map"))
                {
                    var match = Regex.Match(trimmedLine, @"--- Map (\d+_\d+)");
                    newMapName = match.Success ? $"map_{match.Groups[1].Value}" : "";
                }
                else if (trimmedLine.StartsWith("map_"))
                {
                    var match = Regex.Match(trimmedLine, @"map_\d+_\d+");
                    newMapName = match.Success ? match.Value : "";
                }

                if (!string.IsNullOrEmpty(newMapName) && newMapName != currentMapName)
                {
                    currentMapName = newMapName;
                    currentPage = _mapPages.GetValueOrDefault(currentMapName);
                }
            }
            if (trimmedLine.Contains(".AddNpcs({"))
            {
                // Parse the NPCs block
                int bracketCount = 1; // Start with 1 for the opening bracket
                currentLineIndex++; // Move past the .AddNpcs({ line

                while (currentLineIndex < lines.Length && bracketCount > 0)
                {
                    string npcLine = lines[currentLineIndex].Trim();

                    if (npcLine.Contains("{")) bracketCount++;
                    if (npcLine.Contains("}")) bracketCount--;

                    if (npcLine.Contains("MapNpc.CreateNpcWithVnum"))
                    {
                        ParseNpcDefinition(npcLine, currentPage);
                    }

                    currentLineIndex++;
                }
                currentLineIndex--; // Adjust for the extra increment in the main loop
            }
            if (trimmedLine.Contains("AddMonsterWaves({"))
            {
                if (currentPage != null)
                {
                    isProcessingWaves = true;
                    ParseMonsterWaves(lines, ref currentLineIndex, currentPage);
                }
            }          

            if (currentPage != null && trimmedLine.Contains("Monster.CreateWithVnum"))
            {
                ParseMonsterDefinition(trimmedLine, currentPage);
            }

            if (currentPage != null && trimmedLine.Contains("MapObject.Create"))
            {
                ParseMapObjectDefinition(trimmedLine, currentPage, allLines);
            }
            if (currentPage != null && trimmedLine.Contains("TimeSpace.Create"))
            {
                var tsMatch = Regex.Match(trimmedLine, @"TimeSpace\.Create\((\d+)\)");
                if (tsMatch.Success)
                {
                    _tsNumber = int.Parse(tsMatch.Groups[1].Value);
                    _mainForm.textBox2.Text = _tsNumber.ToString();
                }
            }
            if (currentPage != null && trimmedLine.Contains("SetLives("))
            {
                var livesMatch = Regex.Match(trimmedLine, @"SetLives\((\d+)\)");
                if (livesMatch.Success)
                {
                    int lives = int.Parse(livesMatch.Groups[1].Value);
                    _mainForm.textBox4.Text = lives.ToString();
                }
            }
            if (currentPage != null && trimmedLine.Contains("SetDurationInSeconds"))
            {
                var durationMatch = Regex.Match(trimmedLine, @"SetDurationInSeconds\((\d+)\)");
                if (durationMatch.Success)
                {
                    int duration = int.Parse(durationMatch.Groups[1].Value);
                    _mainForm.numericUpDown2.Value = duration;
                }
            }
            if (currentPage != null && trimmedLine.Contains("SetBonusPointItemDropChance"))
            {
                var bonusPointMatch = Regex.Match(trimmedLine, @"SetBonusPointItemDropChance\((\d+)\)");
                if (bonusPointMatch.Success)
                {
                    int bonusPoint = int.Parse(bonusPointMatch.Groups[1].Value);
                    _mainForm.textBox8.Text = bonusPoint.ToString();
                }
            }
            if (currentPage != null && trimmedLine.Contains("SetSpawn"))
            {
                var spawnMatch = Regex.Match(trimmedLine, @"SetSpawn\(Location\.InMap\(map_\d+_\d+\)\.At\((\d+),\s*(\d+)\)\)");
                if (spawnMatch.Success)
                {
                    int x = int.Parse(spawnMatch.Groups[1].Value);
                    int y = int.Parse(spawnMatch.Groups[2].Value);
                    _mainForm.textBox3.Text = x.ToString();
                    _mainForm.textBox5.Text = y.ToString();
                }
            }
            if (currentPage != null && (
                trimmedLine.Contains(".OnTaskFinish({") ||
                trimmedLine.Contains(".OnTaskFail({") ||
                trimmedLine.Contains(".OnAllTargetMobsDead({")
            ))
            {
                eventBuilder = new StringBuilder();
                eventBuilder.AppendLine(trimmedLine);
            }

            if (eventBuilder != null)
            {
                eventBuilder.AppendLine(trimmedLine);
                if (trimmedLine.Contains("})"))
                {
                    // Store the complete event script
                    if (!_eventScripts.ContainsKey(currentMapName))
                    {
                        _eventScripts[currentMapName] = new List<string>();
                    }
                    string completeEventScript = eventBuilder.ToString();
                    _eventScripts[currentMapName].Add(completeEventScript);

                    // Parse the event and add it to the current page
                    if (currentPage != null)
                    {
                        if (completeEventScript.Contains(".OnAllTargetMobsDead"))
                        {
                            ParseOnAllTargetMobsDeadEvent(completeEventScript, currentPage);
                        }
                        else
                        {
                            ParseTaskEvent(completeEventScript, currentPage);
                        }
                    }
                    eventBuilder = null;
                }
            }
            currentLineIndex++;
            }
        foreach (var mapPage in _mapPages.Values)
        {
            if (_eventScripts.TryGetValue(mapPage.MapName, out var scripts))
            {
                foreach (var script in scripts)
                {
                    mapPage.EventManagerScripts[mapPage.MapName] = scripts;
                }
            }
        }
        foreach(var tab in mapTabs)
        {
            tab.SaveAllValues(null, EventArgs.Empty);
        }
        ParseTimeSpaceConfig(_tsNumber);
    }

    private void ParseMapDefinition(string line, List<string> allLines)
    {
        // Example: local map_3_11 = Map.Create().WithMapId(4100).SetMapCoordinates(3, 11)
        var match = Regex.Match(line, @"local (map_\d+_\d+)\s*=\s*Map\.Create\(\)\.WithMapId\((\d+)\)\.SetMapCoordinates\((\d+),\s*(\d+)\)");
        if (match.Success)
        {
            string mapName = match.Groups[1].Value;
            int mapVnum = int.Parse(match.Groups[2].Value);
            int coordX = int.Parse(match.Groups[3].Value);
            int coordY = int.Parse(match.Groups[4].Value);
            var tabPage = new CustomTabPage(mapName, _mainForm, _getMapNames);
            _mainForm.tabControl2.TabPages.Add(tabPage);
            mapTabs.Add(tabPage);
            tabPage.SetMapVnum(mapVnum);
            tabPage.LoadMap(_mainForm);
            tabPage.SetMapCoordinates($"{coordX}, {coordY}");
            var Coords = new Point(coordX, coordY);
            tabPage.SetCoordinates(Coords);

            // Check the next line for TimeSpaceTaskType if we're not at the end of the file
            if (currentLineIndex + 1 < allLines.Count)
            {
                string nextLine = allLines[currentLineIndex + 1];
                if (nextLine.Contains("TimeSpaceTaskType."))
                {
                    var taskMatch = Regex.Match(nextLine, @"TimeSpaceTaskType\.(\w+)\)?");
                    if (taskMatch.Success)
                    {
                        string taskType = taskMatch.Groups[1].Value.TrimEnd(')');
                        tabPage.SetTaskType(taskType);
                        currentLineIndex++;
                    }
                }
                if (nextLine.Contains("WithTaskText"))
                {
                    var taskTextMatch = Regex.Match(nextLine, @"WithTaskText\(""(.*?)""\)");
                    if (taskTextMatch.Success)
                    {
                        string taskText = taskTextMatch.Groups[1].Value;
                        tabPage.SetTaskText(taskText);
                    }
                }
            }

            _mapPages[mapName] = tabPage;
        }
    }

    private void ParseOnAllTargetMobsDeadEvent(string eventScript, CustomTabPage page)
    {
        // Extract the events between the curly braces
        var match = Regex.Match(eventScript, @"\{([\s\S]*?)\}");
        if (!match.Success) return;

        // Split the content by commas and lines, then clean up each event
        var events = match.Groups[1].Value
            .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim())
            .Where(e => !string.IsNullOrWhiteSpace(e) && !e.Contains(".OnAllTargetMobsDead"))
            .ToList();

        // Set the events in the page
        page.SetOnAllTargetMobsDead(events);
    }
    private void ParseTaskEvent(string eventScript, CustomTabPage page)
    {
        // First, determine if this is a TaskFinish or TaskFail event by checking the eventScript
        bool isTaskFinish = eventScript.Contains(".OnTaskFinish");

        // Extract the events between the curly braces
        var match = Regex.Match(eventScript, @"\{([\s\S]*?)\}");
        if (!match.Success) return;

        // Split the content by commas and lines, then clean up each event
        var events = match.Groups[1].Value
            .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim())
            .Where(e => !string.IsNullOrWhiteSpace(e) && !e.Contains(".OnTaskFinish") && !e.Contains(".OnTaskFail"))
            .ToList();

        // Set the appropriate event list based on type
        if (isTaskFinish)
        {
            page.SetTaskFinishEvents(events);
        }
        else
        {
            page.SetTaskFailEvents(events);
        }
    }
    private void ParsePortalDefinition(string line)
    {
        // Example 1: local portal_3_11_to_3_10 = Portal.Create(PortalType.TsNormal).From(map_3_11, 14, 1).To(map_3_10, 14, 28).MinimapOrientation(PortalMinimapOrientation.South)
        // Example 2: local portal_3_5_to_UNKNOWN = Portal.Create(PortalType.TSEndClosed).From(map_3_5, 14, 1).To(map_3_5, 14, 1).MinimapOrientation(PortalMinimapOrientation.North)

        var match = Regex.Match(line, @"portal_(\d+_\d+)_to_(\w+).*From\(map_(\d+_\d+),\s*(\d+),\s*(\d+)\)\.To\(map_([^,]+),\s*(\d+),\s*(\d+)\).*MinimapOrientation\(PortalMinimapOrientation\.(\w+)\)");

        if (match.Success)
        {
            string sourceMapName = $"map_{match.Groups[3].Value}";
            string targetMapName = match.Groups[2].Value.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase)
                ? "UNKNOWN"  // Set target map to "UNKNOWN" directly
                : $"map_{match.Groups[6].Value}";
            int fromX = int.Parse(match.Groups[4].Value);
            int fromY = int.Parse(match.Groups[5].Value);
            int toX = int.Parse(match.Groups[7].Value);
            int toY = int.Parse(match.Groups[8].Value);
            string orientation = match.Groups[9].Value;

            var portalTypeMatch = Regex.Match(line, @"PortalType\.(\w+)");
            string portalType = portalTypeMatch.Success ? portalTypeMatch.Groups[1].Value : "TsNormal"; // Default to TsNormal if not found

            if (_mapPages.TryGetValue(sourceMapName, out var sourcePage))
            {
                Portal portal = new Portal(sourceMapName, targetMapName, portalType, orientation, fromX, fromY, toX, toY, _getMapNames, sourcePage);
                sourcePage.AddPortalToMap(sourcePage.MapName, portal);
                sourcePage.Portals.Add(portal);
            }
        }
    }

    private void ParseMonsterWaves(string[] lines, ref int currentIndex, CustomTabPage page)
    {
        int currentWave = 0;
        decimal waveDelay = 0;
        bool isInWaveBlock = false;

        while (currentIndex < lines.Length)
        {
            string line = lines[currentIndex].Trim();

            // Check for the end of the entire waves section
            if (line == "})")
            {
                break;
            }

            // Parse wave delay and set wave number
            var delayMatch = Regex.Match(line, @"MonsterWave\.CreateWithDelay\((\d+)\s*\*\s*(\d+)\)");
            if (delayMatch.Success)
            {
                int waveNumber = int.Parse(delayMatch.Groups[1].Value);
                waveDelay = decimal.Parse(delayMatch.Groups[2].Value);
                currentWave = waveNumber + 1;  // Convert 0-based to 1-based wave number

                // Set the wave delay input for the page (only needs to be done once)
                if (page._waveDelayInput.Value != waveDelay)
                {
                    page._waveDelayInput.Value = waveDelay;
                    page._useWavesCheckbox.Checked = true;
                }

                isInWaveBlock = true;
            }

            // Parse monsters in the current wave
            if (isInWaveBlock && line.Contains("Monster.CreateWithVnum"))
            {
                ParseWaveMonster(line, page, currentWave);
            }

            // Check for end of current wave block
            if (line.StartsWith("}),") || line == "})")
            {
                isInWaveBlock = false;
            }

            currentIndex++;
        }
    }

    private void ParseWaveMonster(string line, CustomTabPage page, int wave)
    {
        var match = Regex.Match(line, @"Monster\.CreateWithVnum\((\d+)\)\.At\((\d+),\s*(\d+)\)(.*)");
        if (match.Success)
        {
            int vnum = int.Parse(match.Groups[1].Value);
            int x = int.Parse(match.Groups[2].Value);
            int y = int.Parse(match.Groups[3].Value);

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            var attributesPart = match.Groups[4].Value;

            //// Parse facing direction optional as of right now would rather randomize for output
            //var facingMatch = Regex.Match(attributesPart, @"Facing\((\d+)\)");
            //if (facingMatch.Success)
            //{
            //    attributes["Facing"] = facingMatch.Groups[1].Value;
            //}

            // Parse other attributes
            var spawnAfterMobsKilledMatch = Regex.Match(attributesPart, @"SpawnAfterMobsKilled\((\d+)\)");
            if (spawnAfterMobsKilledMatch.Success)
            {
                attributes["SpawnAfterMobsKilled"] = spawnAfterMobsKilledMatch.Groups[1].Value;
            }

            var spawnAfterTaskStartMatch = Regex.Match(attributesPart, @"SpawnAfterTaskStart\(\)");
            if (spawnAfterTaskStartMatch.Success)
            {
                attributes["SpawnAfterTaskStart"] = "true";
            }

            var withCustomLevelMatch = Regex.Match(attributesPart, @"WithCustomLevel\((\d+)\)");
            if (withCustomLevelMatch.Success)
            {
                attributes["WithCustomLevel"] = withCustomLevelMatch.Groups[1].Value;
            }

            // Add the monster with its wave number
            page.AddMonster(vnum, x, y, false, attributes, wave);
        }
    }
    private void ParseMonsterDefinition(string line, CustomTabPage page)
    {
        int currentWave = -1;

        // Parse individual monster definition
        var match = Regex.Match(line, @"Monster\.CreateWithVnum\((\d+)\)\.At\((\d+),\s*(\d+)\)(.*)");
        if (match.Success)
        {
            int vnum = int.Parse(match.Groups[1].Value);
            int x = int.Parse(match.Groups[2].Value);
            int y = int.Parse(match.Groups[3].Value);
            bool isTarget = line.Contains(".AsTarget()");

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            var attributesPart = match.Groups[4].Value;

            // Parse facing direction Optional would rather keep it at a fixed or random value
            //var facingMatch = Regex.Match(attributesPart, @"Facing\((\d+)\)");
            //if (facingMatch.Success)
            //{
            //    attributes["Facing"] = facingMatch.Groups[1].Value;
            //}

            // Parse existing attributes
            var spawnAfterMobsKilledMatch = Regex.Match(attributesPart, @"SpawnAfterMobsKilled\((\d+)\)");
            if (spawnAfterMobsKilledMatch.Success)
            {
                attributes["SpawnAfterMobsKilled"] = spawnAfterMobsKilledMatch.Groups[1].Value;
            }

            var spawnAfterTaskStartMatch = Regex.Match(attributesPart, @"SpawnAfterTaskStart\(\)");
            if (spawnAfterTaskStartMatch.Success)
            {
                attributes["SpawnAfterTaskStart"] = "true";
            }

            var withCustomLevelMatch = Regex.Match(attributesPart, @"WithCustomLevel\((\d+)\)");
            if (withCustomLevelMatch.Success)
            {
                attributes["WithCustomLevel"] = withCustomLevelMatch.Groups[1].Value;
            }

            // Add the monster with wave information if applicable
            page.AddMonster(vnum, x, y, isTarget, attributes, currentWave);
        }
    }
    private void ParseNpcDefinition(string line, CustomTabPage page)
    {
        // Base NPC pattern with coordinates
        var npcMatch = Regex.Match(line, @"MapNpc\.CreateNpcWithVnum\((\d+)\)\.At\((\d+),\s*(\d+)\)");

        if (npcMatch.Success)
        {
            int vnum = int.Parse(npcMatch.Groups[1].Value);
            int x = int.Parse(npcMatch.Groups[2].Value);
            int y = int.Parse(npcMatch.Groups[3].Value);

            // Create new NPC instance
            var npc = new Npc(page.MapName)
            {
                Vnum = vnum,
                X = x,
                Y = y
            };

            // Parse each possible attribute
            if (line.Contains(".WithMustProtectAura()"))
            {
                npc.Attributes["WithMustProtectAura"] = "";
            }

            if (line.Contains(".WithFollowPlayer()"))
            {
                npc.Attributes["WithFollowPlayer"] = "";
            }

            var levelMatch = Regex.Match(line, @"\.WithCustomLevel\((\d+)\)");
            if (levelMatch.Success)
            {
                npc.Attributes["WithCustomLevel"] = levelMatch.Groups[1].Value;
            }

            var hpMatch = Regex.Match(line, @"\.WithHpMultiplier\((\d+(?:\.\d+)?)\)");
            if (hpMatch.Success)
            {
                npc.Attributes["WithHpMultiplier"] = hpMatch.Groups[1].Value;
            }

            page.AddNpc(npc);
        }
    }
    private void ParseMapObjectDefinition(string line, CustomTabPage page, List<string> allLines)
    {
        var objectTypes = new[] { "Lever", "RegularBox", "OldBox", "LuxuriousBox" };

        foreach (var objectType in objectTypes)
        {
            if (line.Contains($"Create{objectType}"))
            {
                var coordMatch = Regex.Match(line, @"\.At\((\d+),\s*(\d+)\)");
                if (coordMatch.Success)
                {
                    int x = int.Parse(coordMatch.Groups[1].Value);
                    int y = int.Parse(coordMatch.Groups[2].Value);

                    List<string> events = null;
                    if (objectType == "Lever")
                    {
                        events = ParseLeverEvents(allLines, ref currentLineIndex);
                    }

                    page.AddMapObject(objectType, x, y, events);
                }
                break;
            }
        }
    }

    private List<string> ParseLeverEvents(List<string> lines, ref int currentLineIndex)
    {
        var events = new List<string>();
        var fullDefinition = new StringBuilder();
        var bracketCount = 0;
        var startLine = currentLineIndex;

        // Collect all lines until we have a complete definition
        do
        {
            string currentLine = lines[currentLineIndex];
            fullDefinition.AppendLine(currentLine);

            bracketCount += currentLine.Count(c => c == '{');
            bracketCount -= currentLine.Count(c => c == '}');

            currentLineIndex++;
        } while (bracketCount > 0 && currentLineIndex < lines.Count);

        // Now parse the complete definition
        string completeDefinition = fullDefinition.ToString();
        var onSwitchMatch = Regex.Match(completeDefinition, @"OnSwitch\({([^}]+)}|OnTrigger\({([^}]+)}", RegexOptions.Singleline);

        if (onSwitchMatch.Success)
        {
            var eventSection = onSwitchMatch.Groups[1].Success ? onSwitchMatch.Groups[1].Value : onSwitchMatch.Groups[2].Value;
            var eventMatches = Regex.Matches(eventSection, @"Event\.(\w+)\(([\w_\d]+)\)");

            foreach (Match match in eventMatches)
            {
                var eventType = match.Groups[1].Value;
                var eventParams = match.Groups[2].Value;
                events.Add($"Event.{eventType}({eventParams})");
            }
        }

        return events;
    }
    private void ParseTimeSpaceConfig(int tsId)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();

            var yaml = File.ReadAllText(_config.TimeSpaceConfigPath);
            var timeSpaces = deserializer.Deserialize<List<TimeSpaceConfig>>(yaml);

            var timeSpace = timeSpaces.FirstOrDefault(ts => ts.TsId == tsId);
            if (timeSpace == null) return;

            // Set basic information
            _mainForm.textBox10.Text = timeSpace.SeedsOfPowerRequired.ToString();
            _mainForm.textBox14.Text = timeSpace.MinLevel.ToString();
            _mainForm.textBox9.Text = timeSpace.MaxLevel.ToString();

            // Set special properties
            _mainForm.isHeroCheckBox.Checked = timeSpace.IsHero;
            _mainForm.isSpecialCheckBox.Checked = timeSpace.IsSpecial;
            _mainForm.isHiddenCheckBox.Checked = timeSpace.IsHidden;

            // Load and set translations
            var translations = LoadTimeSpaceTranslations();

            // Construct the exact keys we're looking for
            string nameKey = $"TS_{tsId}_NAME";
            string descKey = $"TS_{tsId}_DESCRIPTION";

            // Set name if found
            if (translations.TryGetValue(nameKey, out string name))
            {
                _mainForm.modernTextBox3.Text = name;
            }
            else
            {
                _mainForm.modernTextBox3.Text = $"TS {tsId}"; // Fallback name
                Debug.WriteLine($"Translation not found for key: {nameKey}");
            }

            // Set description if found
            if (translations.TryGetValue(descKey, out string description))
            {
                _mainForm.textBox15.Text = description;
            }
            else
            {
                _mainForm.textBox15.Text = "No description available"; // Fallback description
                Debug.WriteLine($"Translation not found for key: {descKey}");
            }

            // Set placement information
            if (timeSpace.Placement?.Any() == true)
            {
                var placement = timeSpace.Placement[0];
                _mainForm.modernTextBox6.Text = placement.MapId.ToString();
                _mainForm.modernTextBox5.Text = placement.MapX.ToString();
                _mainForm.modernTextBox4.Text = placement.MapY.ToString();
            }

            // Set rewards
            SetRewards(timeSpace.Rewards);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error parsing TimeSpace config: {ex}");
            MessageBox.Show("Error parsing TimeSpace configuration.", "Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void SetRewards(TimeSpaceRewards rewards)
    {
        // Set Draw rewards
        if (rewards.Draw?.Count > 0)
        {
            SetItemSlot(_mainForm.DrawItemSlot1, rewards.Draw.ElementAtOrDefault(0));
            SetItemSlot(_mainForm.DrawItemSlot2, rewards.Draw.ElementAtOrDefault(1));
            SetItemSlot(_mainForm.DrawItemSlot3, rewards.Draw.ElementAtOrDefault(2));
            SetItemSlot(_mainForm.DrawItemSlot4, rewards.Draw.ElementAtOrDefault(3));
            SetItemSlot(_mainForm.DrawItemSlot5, rewards.Draw.ElementAtOrDefault(4));
        }

        // Set Special rewards
        if (rewards.Special?.Count > 0)
        {
            SetItemSlot(_mainForm.SpecialItemSlot1, rewards.Special.ElementAtOrDefault(0));
            SetItemSlot(_mainForm.SpecialItemSlot2, rewards.Special.ElementAtOrDefault(1));
        }

        // Set Bonus rewards
        if (rewards.Bonus?.Count > 0)
        {
            SetItemSlot(_mainForm.BonusItemSlot1, rewards.Bonus.ElementAtOrDefault(0));
            SetItemSlot(_mainForm.BonusItemSlot2, rewards.Bonus.ElementAtOrDefault(1));
            SetItemSlot(_mainForm.BonusItemSlot3, rewards.Bonus.ElementAtOrDefault(2));
        }
    }
    private void SetItemSlot(ItemSlot slot, TimeSpaceReward? reward)
    {
        if (reward != null)
        {
            var itemData = _searchManager.GetAllItems()
                .FirstOrDefault(item => item.Vnum == reward.ItemVnum);
            if (itemData != null)
            {
                slot.SetItem(itemData, reward.Amount);
            }
        }
        else
        {
            slot.SetItem(null);
        }
    }
    private Dictionary<string, string> LoadTimeSpaceTranslations()
    {
        var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Make key comparison case-insensitive
        var translationPath = _config.TimespacesFilePath;

        if (File.Exists(translationPath))
        {
            var lines = File.ReadAllLines(translationPath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains(':')) continue;

                // Split on first occurrence of ':' to handle potential colons in the translation text
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    translations[key] = value.Replace("[n]", Environment.NewLine);
                }
            }
        }

        return translations;
    }
    // Optional: Helper method to get portal trigger lines (was used now it reads the Events instead)
    private List<string> GetPortalTriggerLines(string line)
    {
        var events = new List<string>();
        var onSwitchMatch = Regex.Match(line, @"OnSwitch\(\s*{\s*(.*?)\s*}\s*\)", RegexOptions.Singleline);

        if (onSwitchMatch.Success)
        {
            var eventSection = onSwitchMatch.Groups[1].Value;
            events.AddRange(eventSection.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l)));
        }

        return events;
    }
}