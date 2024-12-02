using System.Text.RegularExpressions;
using TimeSpace;

public class TimeSpaceParser
{
    private readonly Form1 _mainForm;
    private readonly string _filePath;
    private readonly Dictionary<string, CustomTabPage> _mapPages;
    private readonly Func<List<string>> _getMapNames;

    public TimeSpaceParser(Form1 mainForm, string filePath, Func<List<string>> getMapNames)
    {
        _mainForm = mainForm;
        _filePath = filePath;
        _mapPages = new Dictionary<string, CustomTabPage>();
        _getMapNames = getMapNames;
    }

    public void PopulateFromFile()
    {
        string[] lines = File.ReadAllLines(_filePath);
        string currentMapName = "";
        CustomTabPage currentPage = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // Parse Map definitions
            if (trimmedLine.StartsWith("local map_"))
            {
                ParseMapDefinition(trimmedLine);
            }
            // Parse Portals
            else if (trimmedLine.StartsWith("local portal_"))
            {
                ParsePortalDefinition(trimmedLine);
            }
            // Parse Map section starts
            else if (trimmedLine.StartsWith("--- Map"))
            {
                currentMapName = ParseMapSectionStart(trimmedLine);
                currentPage = _mapPages.GetValueOrDefault(currentMapName);
            }
            // Parse Monsters
            else if (currentPage != null && trimmedLine.Contains("Monster.CreateWithVnum"))
            {
                ParseMonsterDefinition(trimmedLine, currentPage);
            }
            // Parse MapObjects
            else if (currentPage != null && trimmedLine.Contains("MapObject.Create"))
            {
                ParseMapObjectDefinition(trimmedLine, currentPage);
            }
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
            tabPage.SetMapVnum(mapVnum);
            tabPage.SetMapCoordinates($"{coordX}, {coordY}");

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

    private void ParsePortalDefinition(string line)
    {
        // Example: local portal_3_11_to_3_10 = Portal.Create(PortalType.TsNormal).From(map_3_11, 14, 1).To(map_3_10, 14, 28)
        var match = Regex.Match(line, @"portal_(\d+_\d+)_to_(\d+_\d+).*From\(map_(\d+_\d+),\s*(\d+),\s*(\d+)\)\.To\(map_(\d+_\d+),\s*(\d+),\s*(\d+)\)");
        if (match.Success)
        {
            string sourceMapName = $"map_{match.Groups[3].Value}";
            string targetMapName = $"map_{match.Groups[6].Value}";
            int fromX = int.Parse(match.Groups[4].Value);
            int fromY = int.Parse(match.Groups[5].Value);
            int toX = int.Parse(match.Groups[7].Value);
            int toY = int.Parse(match.Groups[8].Value);

            if (_mapPages.TryGetValue(sourceMapName, out var sourcePage))
            {
                var portalType = line.Contains("PortalType.TsNormal") ? "Type1" : "Type2";
                var orientation = line.Contains("North") ? "North" : "South";
                Portal portal = new Portal(sourceMapName, targetMapName, portalType, orientation, fromX, fromY, toX, toY, _getMapNames, sourcePage);
                sourcePage.AddPortalToMap(sourcePage.MapName, portal);
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
        // This is a simplified version - you might need to enhance this based on your actual file structure
        var portalLines = new List<string>();
        var triggerStart = objectLine.IndexOf("OnTrigger({");
        if (triggerStart != -1)
        {
            var triggerEnd = objectLine.IndexOf("})", triggerStart);
            var triggerContent = objectLine.Substring(triggerStart, triggerEnd - triggerStart);
            portalLines.AddRange(triggerContent.Split(',').Where(l => l.Contains("Portal")));
        }
        return portalLines;
    }
}