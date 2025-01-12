using System.Diagnostics;
using TimeSpace;
using TimeSpace.ItemSearchSystem;

public class ItemSearchManager
{
    private static readonly Dictionary<int, Image> _globalIconCache = new Dictionary<int, Image>();
    private static readonly object _cacheLock = new object();
    private readonly Dictionary<int, ItemData> _items = new Dictionary<int, ItemData>();
    private readonly Config _config;
    private readonly string _iconBasePath;
    private readonly Dictionary<string, string> _translations;
    private static List<ItemData>? _cachedItemList = null;
    private bool _isInitialized = false;
    private const int ICON_SIZE = 32;
    public ItemSearchManager(Config config)
    {
        _config = config;
        _iconBasePath = FindIconsDirectory();
        _translations = LoadTranslations();
        Initialize();
    }

    private void Initialize()
    {
        if (!_isInitialized)
        {
            LoadItems();
            _isInitialized = true;
        }
    }

    public List<ItemData> GetAllItems()
    {
        Debug.WriteLine("GetAllItems called");
        try
        {
            if (_cachedItemList == null)
            {
                Debug.WriteLine("Cache is null, creating new item list");
                Debug.WriteLine($"Items dictionary contains {_items.Count} items");
                _cachedItemList = _items.Values.OrderBy(i => i.Vnum).ToList();
                Debug.WriteLine($"Created cached list with {_cachedItemList.Count} items");
            }
            else
            {
                Debug.WriteLine($"Returning cached list with {_cachedItemList.Count} items");
            }
            return _cachedItemList;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in GetAllItems: {ex}");
            throw;
        }
    }
    private void LoadItems()
    {
        try
        {
            string datPath = Path.Combine(_config.GameDataPath, "item.dat");
            if (!File.Exists(datPath))
            {
                Debug.WriteLine($"item.dat not found at: {datPath}");
                return;
            }

            string[] lines = File.ReadAllLines(datPath);
            ItemData? currentItem = null;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("\tVNUM"))
                {
                    if (currentItem != null)
                    {
                        _items[currentItem.Vnum] = currentItem;
                    }

                    var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int vnum))
                    {
                        currentItem = new ItemData { Vnum = vnum };
                    }
                }
                else if (line.StartsWith("\tNAME") && currentItem != null)
                {
                    var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        currentItem.Name = parts[1].Trim();
                        if (_translations.TryGetValue(currentItem.Name, out string? translated))
                        {
                            currentItem.TranslatedName = translated;
                        }
                    }
                }
                else if (line.StartsWith("\tINDEX") && currentItem != null)
                {
                    var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 6 && int.TryParse(parts[5], out int iconIndex))
                    {
                        currentItem.IconIndex = iconIndex;
                    }
                }
            }

            // Don't forget to add the last item
            if (currentItem != null)
            {
                _items[currentItem.Vnum] = currentItem;
            }

            Debug.WriteLine($"Loaded {_items.Count} items");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading items: {ex.Message}");
            throw;
        }
    }
    public Image GetCachedIcon(int iconIndex)
    {
        lock (_cacheLock)
        {
            if (_globalIconCache.TryGetValue(iconIndex, out Image? icon))
            {
                return icon;
            }

            icon = LoadItemIcon(iconIndex);
            _globalIconCache[iconIndex] = icon;
            return icon;
        }
    }
    private Image LoadItemIcon(int iconIndex)
    {
        try
        {
            string iconPath = Path.Combine(_iconBasePath, $"{iconIndex}.png");
            if (!File.Exists(iconPath))
            {
                Debug.WriteLine($"Icon file not found: {iconPath}");
                iconPath = Path.Combine(_iconBasePath, "0.png");
                if (!File.Exists(iconPath))
                {
                    Debug.WriteLine("Default icon not found either");
                    return new Bitmap(ICON_SIZE, ICON_SIZE);
                }
            }

            using (var stream = new FileStream(iconPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Debug.WriteLine($"Loading icon from: {iconPath}");
                return new Bitmap(Image.FromStream(stream));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading icon {iconIndex}: {ex.Message}");
            return new Bitmap(ICON_SIZE, ICON_SIZE);
        }
    }

    private Dictionary<string, string> LoadTranslations()
    {
        var translations = new Dictionary<string, string>();
        var files = Directory.GetFiles(_config.GameTranslationPath, "*_code_uk_item.txt", SearchOption.AllDirectories);

        if (files.Length > 0)
        {
            foreach (string line in File.ReadAllLines(files[0]))
            {
                var parts = line.Split('\t');
                if (parts.Length >= 2)
                {
                    translations[parts[0]] = parts[1].Replace("[n]", "");
                }
            }
        }

        return translations;
    }

    private string FindIconsDirectory()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var possiblePaths = new[]
        {
            Path.Combine(baseDirectory, "Resources", "Icons"),
            Path.Combine(Directory.GetParent(baseDirectory)?.FullName ?? baseDirectory, "Resources", "Icons"),
            Path.Combine(Directory.GetParent(baseDirectory)?.Parent?.FullName ?? baseDirectory, "Resources", "Icons")
        };

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        var defaultPath = Path.Combine(baseDirectory, "Resources", "Icons");
        Directory.CreateDirectory(defaultPath);
        return defaultPath;
    }
}