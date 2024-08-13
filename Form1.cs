using Newtonsoft.Json;
using System.Text;
using System.Windows.Forms;
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
            tabControl2.TabPages.Add(new CustomeTabPage("test", this));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabPages.Count < 1)
                return;
            tabControl2.TabPages.Remove(tabControl2.SelectedTab);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            loadedMaps = mapResourceFileLoader.LoadAsync().GetAwaiter().GetResult().ToList();
        }

        private void button6_Click(object sender, EventArgs e)
        {

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
    }
}
