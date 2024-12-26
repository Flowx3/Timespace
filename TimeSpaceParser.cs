using System.Text;
using System.Text.RegularExpressions;
using TimeSpace;

public class TimeSpaceParser
{
    private readonly TimeSpaceTool _mainForm;
    private readonly string _filePath;
    private readonly Dictionary<string, CustomTabPage> _mapPages;
    private readonly Func<List<string>> _getMapNames;
    private List<CustomTabPage> mapTabs = TimeSpaceTool.mapTabs;
    private readonly Dictionary<string, List<string>> _eventScripts;

    public TimeSpaceParser(TimeSpaceTool mainForm, string filePath, Func<List<string>> getMapNames)
    {
        _mainForm = mainForm;
        _filePath = filePath;
        _mapPages = new Dictionary<string, CustomTabPage>();
        _getMapNames = getMapNames;
        _eventScripts = new Dictionary<string, List<string>>();
    }

    public async void PopulateFromFile()
    {
        string[] lines = File.ReadAllLines(_filePath);
        string currentMapName = "";
        CustomTabPage currentPage = null;
        StringBuilder eventBuilder = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // Handle existing parsing logic
            if (trimmedLine.StartsWith("local map_"))
            {
                ParseMapDefinition(trimmedLine);
            }
            else if (trimmedLine.StartsWith("local portal_"))
            {
                ParsePortalDefinition(trimmedLine);
            }
            else if (trimmedLine.StartsWith("--- Map"))
            {
                currentMapName = ParseMapSectionStart(trimmedLine);
                currentPage = _mapPages.GetValueOrDefault(currentMapName);
            }
            else if (currentPage != null && trimmedLine.Contains("Monster.CreateWithVnum"))
            {
                ParseMonsterDefinition(trimmedLine, currentPage);
            }
            else if (currentPage != null && trimmedLine.Contains("MapObject.Create"))
            {
                ParseMapObjectDefinition(trimmedLine, currentPage);
            }
            // New event parsing logic
            else if (currentMapName != "" && (trimmedLine.Contains(".OnTaskFinish({") || trimmedLine.Contains(".OnTaskFail({")))
            {
                eventBuilder = new StringBuilder();
                eventBuilder.AppendLine(trimmedLine);
            }
            else if (eventBuilder != null)
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
                        ParseTaskEvent(completeEventScript, currentPage);
                    }

                    eventBuilder = null;
                }
            }
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
    }

    private void ParseMapDefinition(string line)
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
            // Parse task type if present
            if (line.Contains("TimeSpaceTaskType."))
            {
                var taskMatch = Regex.Match(line, @"TimeSpaceTaskType\.(\w+)");
                if (taskMatch.Success)
                {
                    tabPage.SetTaskType(taskMatch.Groups[1].Value);
                }
            }

            _mapPages[mapName] = tabPage;
        }
    }
    private void ParseTaskEvent(string eventScript, CustomTabPage page)
    {
        // Extract event type
        bool isTaskFinish = eventScript.Contains(".OnTaskFinish");

        // Extract portal references
        var portalMatches = Regex.Matches(eventScript, @"Event\.OpenPortal\((portal_[^)]+)\)");
        List<string> portals = new List<string>();
        foreach (Match match in portalMatches)
        {
            portals.Add(match.Groups[1].Value);
        }

        // Extract time modifications
        var addTimeMatch = Regex.Match(eventScript, @"Event\.AddTime\((\d+)\)");
        var removeTimeMatch = Regex.Match(eventScript, @"Event\.RemoveTime\((\d+)\)");

        int addTime = addTimeMatch.Success ? int.Parse(addTimeMatch.Groups[1].Value) : 0;
        int removeTime = removeTimeMatch.Success ? int.Parse(removeTimeMatch.Groups[1].Value) : 0;

        // Check for despawn all mobs
        bool despawnMobs = eventScript.Contains("Event.DespawnAllMobsInRoom");

        // Generate and store the event script
        StringBuilder newScript = new StringBuilder();
        newScript.AppendLine($"{page.MapName}.On{(isTaskFinish ? "TaskFinish" : "TaskFail")}({{");

        foreach (var portal in portals)
        {
            newScript.AppendLine($"    Event.OpenPortal({portal}),");
        }

        if (addTime > 0)
        {
            newScript.AppendLine($"    Event.AddTime({addTime}),");
        }
        if (removeTime > 0)
        {
            newScript.AppendLine($"    Event.RemoveTime({removeTime}),");
        }
        if (despawnMobs)
        {
            newScript.AppendLine($"    Event.DespawnAllMobsInRoom({page.MapName}),");
        }

        newScript.AppendLine("})");

        if (!page.EventManagerScripts.ContainsKey(page.MapName))
        {
            page.EventManagerScripts[page.MapName] = new List<string>();
        }
        page.EventManagerScripts[page.MapName].Add(newScript.ToString());
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

    private string ParseMapSectionStart(string line)
    {
        // Example: --- Map 3_11
        var match = Regex.Match(line, @"--- Map (\d+_\d+)");
        return match.Success ? $"map_{match.Groups[1].Value}" : "";
    }

    private void ParseMonsterDefinition(string line, CustomTabPage page)
    {
        // Example: Monster.CreateWithVnum(339).At(11, 12).AsTarget().SpawnAfterMobsKilled(3).WithCustomLevel(50)  
        var match = Regex.Match(line, @"Monster\.CreateWithVnum\((\d+)\)\.At\((\d+),\s*(\d+)\)(.*)");
        if (match.Success)
        {
            int vnum = int.Parse(match.Groups[1].Value);
            int x = int.Parse(match.Groups[2].Value);
            int y = int.Parse(match.Groups[3].Value);

            bool isTarget = line.Contains(".AsTarget()");

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            var attributesPart = match.Groups[4].Value;

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

            page.AddMonster(vnum, x, y, isTarget, attributes);
        }
    }

    private void ParseMapObjectDefinition(string line, CustomTabPage page)
    {
        // Example: MapObject.CreateLever().At(14, 18)  
        if (line.Contains("CreateLever"))
        {
            var match = Regex.Match(line, @"\.At\((\d+),\s*(\d+)\)");
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);

                // Parse linked portals from the OnTrigger event  
                var linkedPortals = new List<string>();
                var portalLines = GetPortalTriggerLines(line);
                foreach (var portalLine in portalLines)
                {
                    var portalMatch = Regex.Match(portalLine, @"Event\.(?:Open|Toggle)Portal\((portal_\w+)\)");
                    if (portalMatch.Success)
                    {
                        linkedPortals.Add(portalMatch.Groups[1].Value);
                    }
                }

                page.AddMapObject("Lever", x, y, linkedPortals);
            }
        }
        else if (line.Contains("CreateRegularBox"))
        {
            var match = Regex.Match(line, @"\.At\((\d+),\s*(\d+)\)");
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                page.AddMapObject("RegularBox", x, y);
            }
        }
        else if (line.Contains("CreateOldBox"))
        {
            var match = Regex.Match(line, @"\.At\((\d+),\s*(\d+)\)");
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                page.AddMapObject("OldBox", x, y);
            }
        }
        else if (line.Contains("CreateLuxuriousBox"))
        {
            var match = Regex.Match(line, @"\.At\((\d+),\s*(\d+)\)");
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                page.AddMapObject("LuxuriousBox", x, y);
            }
        }
    }
    private List<string> GetPortalTriggerLines(string objectLine)
    {
        var portalLines = new List<string>();
        var triggerStart = objectLine.IndexOf("OnTrigger({");
        if (triggerStart != -1)
        {
            var triggerEnd = objectLine.IndexOf("})", triggerStart);
            if (triggerEnd != -1)
            {
                var triggerContent = objectLine.Substring(triggerStart, triggerEnd - triggerStart);
                portalLines.AddRange(triggerContent.Split(',').Where(l => l.Contains("Portal")));
            }
        }
        return portalLines;
    }
}