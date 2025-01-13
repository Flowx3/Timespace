using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using TimeSpace;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TimeSpace
{

    public struct Config {
        public string GameDataPath { get; set; }
        public string GameMapsPath { get; set; }
        public string TimespacesFilePath { get; set; }
        public string TimeSpaceConfigPath { get; set; }
        public string GameTranslationPath { get; set; }
    }
    public partial class TimeSpaceTool : Form
    {
        public static List<CustomTabPage> mapTabs = new List<CustomTabPage>();
        public Func<List<string>> getMapNames;
        private int mapCount = 0;
        public List<MapDataDTO> loadedMaps = [];
        public Config config;
        private Dictionary<string, string> timespacesData;
        private MapResourceFileLoader mapResourceFileLoader;
        public TimeSpaceTool()
        {
            //.WithOnStartDialog(6194) TO GET PARTNER PUT INTO TIMESPACE CONFIG TAB
            Name = "TimeSpace Tool";
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey900, Primary.Grey900, Accent.Amber700, TextShade.WHITE);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            mapCount++;
            if (!File.Exists("./config.json"))
            {
                string DatPath = "";
                using (var openFileDialog = new FolderBrowserDialog())
                {
                    openFileDialog.Description = "Select Dat Path";

                    var result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        DatPath = openFileDialog.SelectedPath;
                    }
                    else
                    {
                        throw new InvalidOperationException("A required path was not provided.");
                    }
                }
                string mapsPath = "";
                using (var openFileDialog = new FolderBrowserDialog())
                {
                    openFileDialog.Description = "Select Maps Path";

                    var result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        mapsPath = openFileDialog.SelectedPath;
                    }
                    else
                    {
                        throw new InvalidOperationException("A required path was not provided.");
                    }
                }
                string translationPath = "";
                using (var openFileDialog = new FolderBrowserDialog())
                {
                    openFileDialog.Description = "Select Translation Path";
                    var result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        translationPath = openFileDialog.SelectedPath;
                    }
                    else
                    {
                        throw new InvalidOperationException("A required path was not provided.");
                    }
                }
                MessageBox.Show("Please select the Timespaces Translations.yaml file.", "select .YAML file");
                string timespaceTranslation = "";
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        timespaceTranslation = openFileDialog.FileName;
                    }
                }
                string timespaceConfig = "";
                MessageBox.Show("Please select the Timespaces Config.yaml file.", "select .YAML file");
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        timespaceConfig = openFileDialog.FileName;
                    }
                }
                config = new()
                {
                    GameDataPath = DatPath,
                    GameMapsPath = mapsPath,
                    TimespacesFilePath = timespaceTranslation,
                    TimeSpaceConfigPath = timespaceConfig,
                    GameTranslationPath = translationPath
                };
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            else
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("./config.json"));
            }
            InitializeComponent();
            textBox1.Text = config.TimespacesFilePath;
            textBox11.Text = config.GameMapsPath;
            textBox12.Text = config.GameDataPath;
            textBox13.Text = config.TimeSpaceConfigPath;
            modernTextBox2.Text = config.GameTranslationPath;
            var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
            timespacesData = deserializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(config.TimespacesFilePath));
            mapResourceFileLoader = new(new ResourceLoadingConfiguration() { GameDataPath = config.GameDataPath, GameMapsPath = config.GameMapsPath });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string timespaceTranslation = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    timespaceTranslation = openFileDialog.FileName;
                }
                else
                {
                    if (config.TimespacesFilePath != null)
                        timespaceTranslation = config.TimespacesFilePath;
                    else
                        throw new InvalidOperationException("A required path was not provided.");
                }
            }
            config.TimespacesFilePath = timespaceTranslation;
            textBox1.Text = config.TimespacesFilePath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 1)
            {
                var tsNameKey = $"{textBox2.Text}_NAME";
                var tsDescriptionKey = $"{textBox2.Text}_DESCRIPTION";
                timespacesData[tsNameKey] = modernTextBox3.Text;
                timespacesData[tsDescriptionKey] = textBox15.Text;

                var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
                File.WriteAllText(config.TimespacesFilePath, serializer.Serialize(timespacesData));
                MessageBox.Show("TS data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            var newTab = new CustomTabPage($"map_3_{mapCount}", this, GetMapNames);
            newTab.AutoScroll = true;

            tabControl2.TabPages.Add(newTab);
            mapTabs.Add(newTab);
            mapCount++;

            // Refresh all tabs
            foreach (var tab in mapTabs)
            {
                tab.SaveAndRefreshPortals(sender, e, false);
            }
        }
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is TabControl tabControl && tabControl.SelectedTab is CustomTabPage currentMapTab)
            {
                currentMapTab.RefreshGridMarkings();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabPages.Count < 1)
                return;

            var selectedTab = tabControl2.SelectedTab as CustomTabPage;
            if (selectedTab != null)
            {
                // Clean up the coordinates
                selectedTab.CleanupCoordinates();

                // Remove from tab control and list
                tabControl2.TabPages.Remove(selectedTab);
                if (mapTabs.Contains(selectedTab))
                {
                    mapTabs.Remove(selectedTab);
                }

                // Dispose the tab
                selectedTab.Dispose();
            }
        }
        private void MaterialButton1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Lua files (*.lua)|*.lua|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadTimeSpaceScript(openFileDialog.FileName);
                }
            }
        }
        private void materialButton2_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.Description = "Select lang folder";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    modernTextBox2.Text = openFileDialog.SelectedPath;
                }
            }
            config.GameTranslationPath = modernTextBox2.Text;
        }
        private void LoadTimeSpaceScript(string filePath)
        {
            foreach (var customTabPage in mapTabs)
            {
                customTabPage.CleanupCoordinates();
                customTabPage.Dispose();
            }
            tabControl2.TabPages.Clear();
            mapTabs.Clear();
            var parser = new TimeSpaceParser(this, filePath, GetMapNames, config);
            parser.PopulateFromFile();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            loadedMaps = mapResourceFileLoader.LoadAsync().GetAwaiter().GetResult().ToList();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (mapTabs.Count <= 0)
            {
                MessageBox.Show("You need at least 1 Map to create a TimeSpace.", "Can't generate Script.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var tabpage in mapTabs)
            {
                tabpage.SaveAllValues(sender, e);
            }
            var luaScript = new StringBuilder();

            // Add the initial required imports  
            luaScript.AppendLine("-- TimeSpace Script generated by Time Space Tool. Check for errors, and modify things if needed.");
            luaScript.AppendLine("local Map = require('Map')");
            luaScript.AppendLine("local Monster = require('Monster')");
            luaScript.AppendLine("local Event = require('Event')");
            luaScript.AppendLine("local MapObject = require('MapObject')");
            luaScript.AppendLine("local MapNpc = require('MapNpc')");
            luaScript.AppendLine("local Portal = require('Portal')");
            luaScript.AppendLine("local Location = require('Location')");
            luaScript.AppendLine("local TimeSpace = require('TimeSpace')");
            luaScript.AppendLine("local PortalType = require('PortalType')");
            luaScript.AppendLine("local PortalMinimapOrientation = require('PortalMinimapOrientation')");
            luaScript.AppendLine("local TimeSpaceObjective = require('TimeSpaceObjective')");
            luaScript.AppendLine("local TimeSpaceTaskType = require('TimeSpaceTaskType')");
            luaScript.AppendLine("local TimeSpaceTask = require('TimeSpaceTask')");
            luaScript.AppendLine("local MonsterWave = require('MonsterWave')");
            luaScript.AppendLine();

            // Generate objective script  
            luaScript.AppendLine(objectivesPanel.GetObjectivesScript());
            luaScript.AppendLine();

            // Generate map scripts  
            var mapScripts = new StringBuilder();
            foreach (var mapTab in mapTabs)
            {
                mapScripts.AppendLine(mapTab.GenerateMapScript());
            }

            luaScript.AppendLine();

            // Load saved portal scripts  
            var localPortalScript = File.ReadAllText("localPortals.lua");
            var addPortalScript = File.ReadAllText("addPortals.lua");

            // Append the map scripts to the main script  
            luaScript.AppendLine(mapScripts.ToString());

            // Append the portal scripts to the main script  
            luaScript.AppendLine(localPortalScript);
            luaScript.AppendLine(addPortalScript);
            luaScript.AppendLine();

            // Load and combine individual event files
            var eventHandlingScripts = new StringBuilder();
            foreach (var tab in mapTabs)
            {
                var eventFile = $"Events_{tab.MapName}.lua";
                if (File.Exists(eventFile))
                {
                    eventHandlingScripts.AppendLine(File.ReadAllText(eventFile));
                }
            }

            // Append the event handling scripts to the main script  
            luaScript.AppendLine(eventHandlingScripts.ToString());

            // Final TimeSpace creation script  
            var mapNames = string.Join(", ", mapTabs.Select(m => m.MapName));
            luaScript.AppendLine($"local ts = TimeSpace.Create({textBox2.Text})  -- TimeSpace ID");
            luaScript.AppendLine($"    .SetMaps({{{mapNames}}})");
            luaScript.AppendLine($"    .SetSpawn(Location.InMap({mapTabs[0].MapName}).At({textBox3.Text}, {textBox5.Text}))");
            luaScript.AppendLine($"    .SetLives({textBox4.Text})");
            luaScript.AppendLine($"    .SetObjectives(objectives)");
            luaScript.AppendLine($"    .SetDurationInSeconds({textBox7.Text})");
            luaScript.AppendLine($"    .SetBonusPointItemDropChance({textBox8.Text})");
            luaScript.AppendLine("return ts");

            // Save the script to a file  
            var script = luaScript.ToString();
            using (SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Lua files (*.lua)|*.lua",
                Title = "Save Lua Script"
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, script);
                }
            }
        }
        private List<string> GetMapNames()
        {
            return mapTabs.Select(m => m.MapName).ToList();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            string mapsPath = "";
            using (var openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.Description = "Select Maps Path";

                var result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    mapsPath = openFileDialog.SelectedPath;
                }
                else
                {
                    if (config.GameDataPath != null)
                        mapsPath = config.GameMapsPath;
                    else
                        throw new InvalidOperationException("A required path was not provided.");
                }
            }
            config.GameMapsPath = mapsPath;
            textBox11.Text = config.GameMapsPath;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string DatPath = "";
            using (var openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.Description = "Select Dat Path";

                var result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    DatPath = openFileDialog.SelectedPath;
                }
                else
                {
                    if (config.GameMapsPath != null)
                        DatPath = config.GameDataPath;
                    else
                        throw new InvalidOperationException("A required path was not provided.");
                }
            }
            config.GameDataPath = DatPath;
            textBox12.Text = config.GameDataPath;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            string timespaceConfig = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    timespaceConfig = openFileDialog.FileName;
                }
                else
                {
                    if (config.TimeSpaceConfigPath != null)
                        timespaceConfig = config.TimeSpaceConfigPath;
                    else
                        throw new InvalidOperationException("A required path was not provided.");
                }
            }
            config.TimeSpaceConfigPath = timespaceConfig;
            textBox13.Text = config.TimeSpaceConfigPath;
        }
    }
}
