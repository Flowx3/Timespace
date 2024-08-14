using Newtonsoft.Json;
using System.Text;
using System.Windows.Forms;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TimeSpace
{
    struct Config {
        public string GameDataPath { get; set; }
        public string GameMapsPath { get; set; }
        public string TimespacesFilePath { get; set; }
    }
    public partial class Form1 : Form
    {
        private List<CustomeTabPage> mapTabs = new List<CustomeTabPage>();
        public List<MapDataDTO> loadedMaps = [];
        private Config config;
        private Dictionary<string, string> timespacesData;
        private MapResourceFileLoader mapResourceFileLoader;
        public Form1()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();
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
                string timespaceTranslation = "";
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        timespaceTranslation = openFileDialog.FileName;
                    }
                }
                config = new()
                {
                    GameDataPath = DatPath,
                    GameMapsPath = mapsPath,
                    TimespacesFilePath = timespaceTranslation,
                };
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            else
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("./config.json"));
            }
            textBox1.Text = config.TimespacesFilePath;
            textBox11.Text = config.GameMapsPath;
            textBox12.Text = config.GameDataPath;
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
                    if(config.TimespacesFilePath != null)
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
                timespacesData[tsNameKey] = textBox4.Text;
                timespacesData[tsDescriptionKey] = textBox6.Text;

                var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
                File.WriteAllText(config.TimespacesFilePath, serializer.Serialize(timespacesData));
                MessageBox.Show("TS data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label9.Visible = false;
            label10.Visible = false;
            textBox9.Visible = false;
            textBox10.Visible = false;
            switch (comboBox1.SelectedIndex)
            {
                case 1:
                case 2:
                case 3:
                    {
                        label9.Text = "VNUM";
                        label9.Visible = true;
                        label10.Text = "Amount";
                        label10.Visible = true;
                        textBox9.Visible = true;
                        textBox10.Visible = true;

                    }
                    break;
                case 4:
                    {
                        label9.Visible = true;
                        label9.Text = "Conversation ID";
                        textBox9.Visible = true;
                    }
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var newTab = new CustomeTabPage("test", this);
            tabControl2.TabPages.Add(newTab);
            mapTabs.Add(newTab);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabPages.Count < 1)
                return;

            var selectedTab = tabControl2.SelectedTab;

            if (selectedTab != null)
            {
                tabControl2.TabPages.Remove(selectedTab);

                if (mapTabs.Contains((CustomeTabPage)selectedTab))
                {
                    mapTabs.Remove((CustomeTabPage)selectedTab);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        { 
            loadedMaps = mapResourceFileLoader.LoadAsync().GetAwaiter().GetResult().ToList();
        }

        private void button6_Click(object sender, EventArgs e)
        {
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
            luaScript.AppendLine();

            //Generate objective script
            luaScript.AppendLine($"local objectives = {GenerateObjectiveScript()}");
            luaScript.AppendLine();

            // Generate map scripts  
            var mapScripts = new StringBuilder();
            foreach (var mapTab in mapTabs)
            {
                mapScripts.AppendLine(mapTab.GenerateMapScript());
            }

            // Add an empty line between sections  
            mapScripts.AppendLine();

            // Generate portal scripts  
            var localPortalScript = new StringBuilder();
            var addPortalScript = new StringBuilder();
            //foreach (var mapTab in mapTabs)
            //{
            //    foreach (var portal in mapTab.Portals)
            //    {
            //        localPortalScript.AppendLine(portal.GenerateLocalPortalScript());
            //        addPortalScript.AppendLine(portal.GenerateAddPortalScript());
            //    }
            //}

            // Append the map scripts to the main script  
            luaScript.AppendLine(mapScripts.ToString());

            // Append the portal scripts to the main script  
            luaScript.AppendLine(localPortalScript.ToString());
            luaScript.AppendLine(addPortalScript.ToString());

            // Add an empty line between sections  
            luaScript.AppendLine();

            // Generate event handling scripts  
            var eventHandlingScripts = new StringBuilder();
            //foreach (var mapTab in mapTabs)
            //{
            //    eventHandlingScripts.AppendLine(mapTab.GenerateEventHandlingScript());
            //}

            // Append the event handling scripts to the main script  
            luaScript.AppendLine(eventHandlingScripts.ToString());

            // Final TimeSpace creation script  
            var mapNames = string.Join(", ", mapTabs.Select(m => m.MapName));
            luaScript.AppendLine($"local ts = TimeSpace.Create({textBox2.Text})  -- TimeSpace ID");
            luaScript.AppendLine($"    .SetMaps({{{mapNames}}})");
            luaScript.AppendLine($"    .SetSpawn(Location.InMap({mapTabs[0].MapName}).At({textBox3.Text}, {textBox5.Text}))");
            luaScript.AppendLine($"    .SetLives({numericUpDown1.Value})");
            luaScript.AppendLine($"    .SetObjectives(objectives)");
            luaScript.AppendLine($"    .SetDurationInSeconds({textBox7.Text})");
            luaScript.AppendLine($"    .SetBonusPointItemDropChance({textBox8.Text})");
            luaScript.AppendLine("return ts");

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
        private string GenerateObjectiveScript()
        {
            var objective = comboBox1.SelectedItem?.ToString();
            var script = new StringBuilder($"TimeSpaceObjective.Create().With{objective}()");
            switch (objective)
            {
                case "KillMonsterVnum":
                case "CollectItemVnum":
                case "InteractObjectsVnum":
                    var vnum = textBox9.Text;
                    var amount = textBox10.Text;
                    if (!string.IsNullOrEmpty(vnum))
                    {
                        script.Append(vnum);
                        if (!string.IsNullOrEmpty(amount))
                        {
                            script.Append($", {amount}");
                        }
                    }
                    if (checkBox1.Checked)
                        script.Append(".WithProtectNPC()");
                    break;
                case "Conversation":
                    script.Append($"{textBox9.Text}");
                    break;
                case "GoToExit":
                    if (checkBox1.Checked)
                        script.Append(".WithProtectNPC()");
                    break;
                case "KillAllMonster":
                    if (checkBox1.Checked)
                        script.Append(".WithProtectNPC()");
                    break;
            }
            return script.ToString();
        }
    }
}
