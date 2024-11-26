using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace TimeSpace
{
    public class MapGridPanel : Panel
    {
        private byte[] _grid;
        private int _width;
        private int _height;
        private int _cellSize;

        private const int FixedPanelWidth = 790;
        private const int FixedPanelHeight = 450;

        public event EventHandler<CellClickedEventArgs> CellClicked;

        public MapGridPanel()
        {
            // Set fixed panel size
            this.Width = FixedPanelWidth;
            this.Height = FixedPanelHeight;

            // Set the panel's Name property
            this.Name = "mapGridPanel";

            this.Size = new Size(FixedPanelWidth, FixedPanelHeight);
            // Enable auto scrolling and double buffering
            //this.AutoScroll = true;
            this.DoubleBuffered = true;
            this.BorderStyle = BorderStyle.FixedSingle;

            this.MouseClick += MapGridPanel_MouseClick; // Subscribe to mouse click event
        }
        public void SetGrid(int width, int height, byte[] grid)
        {
            _width = width;
            _height = height;
            _grid = grid;

            // Calculate the maximum cell size that fits the panel
            int maxCellWidth = FixedPanelWidth / _width;
            int maxCellHeight = FixedPanelHeight / _height;
            _cellSize = Math.Min(maxCellWidth, maxCellHeight);

            // Calculate the total width and height required to display the grid
            int totalWidth = _width * _cellSize;
            int totalHeight = _height * _cellSize;

            // Set the panel's size to fit the grid
            this.AutoScroll = true; // Ensure scrolling is enabled
            this.Width = Math.Max(FixedPanelWidth, totalWidth);
            this.Height = Math.Max(FixedPanelHeight, totalHeight);
            Invalidate(); // Trigger a repaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_grid == null) return;

            var graphics = e.Graphics;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var color = GetColor(_grid[y * _width + x]);
                    using (var brush = new SolidBrush(color))
                    {
                        graphics.FillRectangle(brush, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                    }

                    // Draw the cell border
                    using (var pen = new Pen(Color.Black))
                    {
                        graphics.DrawRectangle(pen, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                    }
                }
            }
        }
        private readonly Dictionary<int, Color> _flagColors = new Dictionary<int, Color>
    {
        { 0x1, Color.Gray }, // IsWalkingDisabled
        { 0x2, Color.Red },  // IsAttackDisabledThrough
        { 0x4, Color.Blue }, // UnknownYet
        { 0x8, Color.Green }, // IsMonsterAggroDisabled
        { 0x10, Color.Purple }, // IsPvpDisabled
        {0xB,Color.FromArgb(255, 152, 100, 100) },
        { 0x20, Color.Orange } // MateDoll
    };
        private Color GetColor(byte value)
        {
            // Customize this method based on your grid values
            // Map each flag value to its corresponding color
            if (_flagColors.TryGetValue(value, out var color))
            {
                return color;
            }
            return Color.White; // Default color for undefined flags
        }

        private void MapGridPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // Calculate cell coordinates based on mouse click position
            int cellX = e.X / _cellSize;
            int cellY = e.Y / _cellSize;

            // Ensure the click is within bounds
            if (cellX >= 0 && cellX < _width && cellY >= 0 && cellY < _height)
            {
                OnCellClicked(new CellClickedEventArgs(cellX, cellY));
            }
        }

        protected virtual void OnCellClicked(CellClickedEventArgs e)
        {
            CellClicked?.Invoke(this, e);
        }
    }

    public class CellClickedEventArgs : EventArgs
    {
        public int CellX { get; }
        public int CellY { get; }

        public CellClickedEventArgs(int cellX, int cellY)
        {
            CellX = cellX;
            CellY = cellY;
        }
    }
    public class CustomTabPage : TabPage
    {
        private TaskEventManagerForm taskEventManagerForm;
        private MonsterManager monsterManager;
        public string MapName { get; private set; }
        public Func<List<string>> getMapNames;
        public Dictionary<string, Panel> MapPortalPanels { get; set; }
        public List<Portal> Portals { get; private set; } = new List<Portal>();
        public List<MapObject> Objects { get; private set; } = new List<MapObject>();
        public List<Monster> MonsterEvents { get; private set; } = new List<Monster>();

        private List<string> allPortalsList = new List<string>();
        private List<string> lockedPortalsList = new List<string>();
        private List<DataGridView> monsterDataGrids;
        private List<CustomTabPage> mapTabs = Form1.mapTabs;
        private Task _gridCreationTask;
        private static List<Point> sharedTakenPositions = new List<Point>();
        private static readonly object positionsLock = new object();
        private Dictionary<string, List<string>> eventManagerScripts = new Dictionary<string, List<string>>();
        private bool isDisposed = false;

        private TextBox txtMapVNUM;
        private TextBox txtMapCoordinates;
        private TextBox txtTaskText;
        private ComboBox cboTaskType;
        private ComboBox[] taskFinishPortals;
        private ComboBox[] taskFailPortals;
        private ComboBox eventTypeComboBox;
        private NumericUpDown addTimeEvent;
        private CheckBox despawnAllMobsInRoom;
        private NumericUpDown timeForTask;
        private NumericUpDown removeTimeEvent;
        private FlowLayoutPanel portalPanel;
        private DataGridView monsterDataGridView;
        private FlowLayoutPanel eventPanel;
        private FlowLayoutPanel objectivePanel;
        private Point? currentPosition = null;
        private Button btnAddAttribute;
        private Button applyButton;
        private NumericUpDown waveCountInput;
        private NumericUpDown waveDelayInput;
        public CheckBox useWavesCheckbox;

        public CustomTabPage(string mapName, Form1 form, Func<List<string>> getMapNames)
        {
            MapPortalPanels = new Dictionary<string, Panel>();
            MonsterEvents = new List<Monster>();
            monsterManager = new MonsterManager(waveDelayInput);
            Text = mapName;
            this.getMapNames = getMapNames;
            this.MapName = mapName;

            InitializeComponents(form);
        }

        private void InitializeComponents(Form1 form)
        {
            var containerPanel = new Panel { Dock = DockStyle.Fill };
            var leftPanel = CreateLeftPanel(form);
            var rightPanel = CreateRightPanel();

            containerPanel.Controls.Add(rightPanel);
            containerPanel.Controls.Add(leftPanel);
            this.Controls.Add(containerPanel);
        }

        private Panel CreateLeftPanel(Form1 form)
        {
            var leftPanel = new Panel { Width = 1000, Dock = DockStyle.Left };

            txtMapVNUM = new TextBox { Name = "txtMapVNUM", Location = new Point(150, 10), Width = 200 };
            txtMapCoordinates = new TextBox { Location = new Point(150, 40), Width = 200 };
            cboTaskType = new ComboBox
            {
                Location = new Point(150, 70),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "None", "KillAllMonsters", "Survive" }
            };
            txtTaskText = new TextBox { Location = new Point(150, 100), Width = 200 };
            timeForTask = new NumericUpDown
            {
                Width = 50,
                Location = new Point(440, 100),
                Maximum = 600,
                Increment = 10
            };

            portalPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 130),
                Width = 1000,
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };

            eventPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 420),
                Width = 1000,
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };

            AddControlsToLeftPanel(leftPanel, form);
            InitializeDataGridView();

            return leftPanel;
        }

        private void AddControlsToLeftPanel(Panel leftPanel, Form1 form)
        {
            leftPanel.Controls.AddRange(new Control[]
            {
            new Label { Text = "Map Vnum:", Location = new Point(10, 10) },
            txtMapVNUM,
            CreateButton("Load Map", new Point(360, 10), (sender, e) => LoadMap(form)),
            new Label { Text = "Map Coordinates:", Location = new Point(10, 40) },
            txtMapCoordinates,
            CreateButton("...", new Point(txtMapCoordinates.Right + 10, 40), BtnSelectCoordinates_Click, 30, txtMapCoordinates.Height),
            new Label { Text = "Task Type:", Location = new Point(10, 70) },
            cboTaskType,
            new Label { Text = "Task Text:", Location = new Point(10, 100) },
            txtTaskText,
            new Label { Text = "Time for Task:", Location = new Point(360, 105), Width = 80 },
            timeForTask,
            portalPanel,
            CreateButton("Add Portal", new Point(10, 390), BtnAddPortal_Click),
            CreateButton("Remove Last Portal", new Point(150, 390), BtnRemovePortal_Click),
            CreateButton("Save Portals", new Point(290, 390), BtnSavePortal_Click),
            eventPanel,
            CreateButton("Add Monster Event", new Point(10, 730), BtnAddEvent_Click),
            CreateButton("Remove Last Monster Event", new Point(150, 730), BtnRemoveEvent_Click),
            CreateButton("Save Monsters", new Point(290, 730), BtnSaveMonsterAndObjective_Click),
            CreateButton("Manage Events", new Point(360, 70), (s, e) => ManageEvents())
            });
        }

        private Panel CreateRightPanel()
        {
            var rightPanel = new Panel { Dock = DockStyle.Fill };
            var mapGridPanel = new MapGridPanel();
            mapGridPanel.CellClicked += MapGridPanel_CellClicked;

            objectivePanel = new FlowLayoutPanel
            {
                Location = new Point(10, 500),
                Width = 980,
                Height = 400,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };

            rightPanel.Controls.AddRange(new Control[]
            {
            mapGridPanel,
            objectivePanel,
            CreateButton("Add Objective", new Point(10, 470), BtnAddObjective_Click, width: 150, height: 30),
            CreateButton("Remove Last Objective", new Point(170, 470), BtnRemoveObjective_Click, width: 150, height: 30),
            CreateButton("Save Objectives", new Point(330, 470), BtnSaveMonsterAndObjective_Click, width: 150, height: 30),
            new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Hover Position: " }
            });
            return rightPanel;
        }

        private Button CreateButton(string text, Point location, EventHandler clickHandler, int width = 120, int height = 30)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Width = width,
                Height = height
            };
            button.Click += clickHandler;
            return button;
        }
        private void ManageEvents()
        {
            var eventManager = new TaskEventManagerForm(MapName, lockedPortalsList);
            if (eventManager.ShowDialog() == DialogResult.OK)
            {
                string generatedScript = eventManager.Tag as string;
                if (!string.IsNullOrEmpty(generatedScript))
                {
                    if (!eventManagerScripts.ContainsKey(MapName))
                    {
                        eventManagerScripts[MapName] = new List<string>();
                    }
                    eventManagerScripts[MapName].Add(generatedScript);
                }
            }
        }

        private void InitializeDataGridView()
        {
            useWavesCheckbox = new CheckBox
            {
                Text = "Use Waves",
                Location = new Point(10, 10),
                AutoSize = true
            };
            useWavesCheckbox.CheckedChanged += UseWavesCheckbox_CheckedChanged;

            waveCountInput = new NumericUpDown
            {
                Location = new Point(230, 10),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Enabled = false
            };

            waveDelayInput = new NumericUpDown
            {
                Location = new Point(430, 10),
                Width = 60,
                Minimum = 0,
                Maximum = 300,
                Value = 30,
                Enabled = false
            };

            var wavePanel = new Panel
            {
                Location = new Point(0, 0),
                Width = 800,
                Height = 40
            };
            wavePanel.Controls.AddRange(new Control[]
            {
        useWavesCheckbox,
        new Label { Text = "Number of Waves:", Location = new Point(120, 12), AutoSize = true },
        waveCountInput,
        new Label { Text = "Wave Delay (seconds):", Location = new Point(300, 12), AutoSize = true },
        waveDelayInput
            });

            btnAddAttribute = new Button
            {
                Text = "Manage Attributes",
                Location = new Point(0, wavePanel.Bottom + 10),
                Width = 120,
                Height = 30
            };
            btnAddAttribute.Click += BtnAddAttribute_Click;

            monsterDataGridView = new DataGridView
            {
                Location = new Point(0, btnAddAttribute.Bottom + 10),
                Width = 800,
                Height = 200,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false
            };
            monsterDataGridView.Columns.AddRange(new DataGridViewColumn[]
            {
        new DataGridViewTextBoxColumn { Name = "Vnum", HeaderText = "Vnum" },
        new DataGridViewTextBoxColumn { Name = "X", HeaderText = "X" },
        new DataGridViewTextBoxColumn { Name = "Y", HeaderText = "Y" },
        new DataGridViewCheckBoxColumn { Name = "AsTarget", HeaderText = "As Target" },
        new DataGridViewTextBoxColumn
        {
            Name = "Attributes",
            HeaderText = "Additional Attributes",
            ReadOnly = true
        }
            });
            monsterDataGridView.CellDoubleClick += MonsterDataGridView_CellDoubleClick;

            eventPanel.Controls.AddRange(new Control[]
            {
        wavePanel,
        btnAddAttribute,
        monsterDataGridView
            });
            eventPanel.Height = monsterDataGridView.Bottom + 20;
        }
        private void LoadMap(Form1 form)
        {
            if (int.TryParse(txtMapVNUM.Text, out int mapVnum))
            {
                var mapData = form.loadedMaps.FirstOrDefault(m => m.Id == mapVnum);
                if (mapData != null)
                {
                    DisplayMapGrid(mapData);
                }
                else
                {
                    MessageBox.Show("Map not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid map number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayMapGrid(MapDataDTO mapData)
        {
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).First();
            mapGridPanel.SetGrid(mapData.Width, mapData.Height, mapData.Grid);
        }

        private void MapGridPanel_CellClicked(object sender, CellClickedEventArgs e)
        {
            MessageBox.Show($"Cell clicked at ({e.CellX}, {e.CellY})");
        }

        private void MonsterDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && monsterDataGridView.Columns[e.ColumnIndex].Name == "Attributes")
            {
                BtnAddAttribute_Click(sender, e);
            }
        }

        private void BtnSelectCoordinates_Click(object sender, EventArgs e)
        {
            lock (positionsLock)
            {
                // Create a temporary list without the current position
                var tempTakenPositions = new List<Point>(sharedTakenPositions);
                if (currentPosition.HasValue)
                {
                    tempTakenPositions.Remove(currentPosition.Value);
                }

                using (var gridSelector = new GridSelectorForm(tempTakenPositions))
                {
                    if (gridSelector.ShowDialog() == DialogResult.OK && gridSelector.SelectedCoordinates.HasValue)
                    {
                        // Remove old position if it exists
                        if (currentPosition.HasValue)
                        {
                            sharedTakenPositions.Remove(currentPosition.Value);
                        }

                        // Update to new position
                        Point selectedPos = gridSelector.SelectedCoordinates.Value;
                        txtMapCoordinates.Text = $"{selectedPos.X}_{selectedPos.Y}";
                        Text = $"map_{selectedPos.X}_{selectedPos.Y}";
                        MapName = $"map_{selectedPos.X}_{selectedPos.Y}";
                        currentPosition = selectedPos;
                        sharedTakenPositions.Add(selectedPos);
                    }
                }
            }
        }

        public Point? GetCoordinates()
        {
            return currentPosition;
        }

        public void SetCoordinates(Point point)
        {
            lock (positionsLock)
            {
                // Remove old position if it exists
                if (currentPosition.HasValue)
                {
                    sharedTakenPositions.Remove(currentPosition.Value);
                }

                txtMapCoordinates.Text = $"{point.X}, {point.Y}";
                currentPosition = point;
                sharedTakenPositions.Add(point);
            }
        }

        public void ClearCoordinates()
        {
            lock (positionsLock)
            {
                if (currentPosition.HasValue)
                {
                    sharedTakenPositions.Remove(currentPosition.Value);
                    currentPosition = null;
                }
                txtMapCoordinates.Text = string.Empty;
            }
        }

        // Optional: Method to get all taken positions (might be useful for debugging)
        public static List<Point> GetAllTakenPositions()
        {
            lock (positionsLock)
            {
                return new List<Point>(sharedTakenPositions);
            }
        }

        // Optional: Method to clear all positions (might be useful when starting fresh)
        public static void ClearAllPositions()
        {
            lock (positionsLock)
            {
                sharedTakenPositions.Clear();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    CleanupCoordinates();
                }
                isDisposed = true;
            }
            base.Dispose(disposing);
        }

        public void CleanupCoordinates()
        {
            lock (positionsLock)
            {
                if (currentPosition.HasValue)
                {
                    sharedTakenPositions.Remove(currentPosition.Value);
                    currentPosition = null;
                    if (txtMapCoordinates != null)
                    {
                        txtMapCoordinates.Text = string.Empty;
                    }
                }
            }
        }
        private void UseWavesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            waveCountInput.Enabled = useWavesCheckbox.Checked;
            waveDelayInput.Enabled = useWavesCheckbox.Checked;

            if (useWavesCheckbox.Checked)
            {
                if (!monsterDataGridView.Columns.Contains("Wave"))
                {
                    monsterDataGridView.Columns.Insert(3, new DataGridViewTextBoxColumn { Name = "Wave", HeaderText = "Wave" });
                }
            }
            else
            {
                if (monsterDataGridView.Columns.Contains("Wave"))
                {
                    monsterDataGridView.Columns.Remove("Wave");
                }
            }
        }

        private void BtnAddAttribute_Click(object sender, EventArgs e)
        {
            if (monsterDataGridView.CurrentRow == null) return;

            int rowIndex = monsterDataGridView.CurrentRow.Index;
            var monster = MonsterEvents[rowIndex];

            var attributeForm = new MonsterAttributeForm(monster.Attributes);

            if (attributeForm.ShowDialog() == DialogResult.OK)
            {
                monster.Attributes.Clear();
                foreach (var attr in attributeForm.SelectedAttributes)
                {
                    monster.Attributes[attr.Key] = attr.Value;
                }
                UpdateAttributeDisplay(rowIndex);
            }
        }

        private void UpdateAttributeDisplay(int rowIndex)
        {
            var monster = MonsterEvents[rowIndex];
            var attributeText = string.Join(", ", monster.Attributes.Select(a => $"{a.Key}={a.Value}"));
            monsterDataGridView.Rows[rowIndex].Cells["Attributes"].Value = attributeText;
        }

        public string GenerateMapScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"local {MapName} = Map.Create().WithMapId({txtMapVNUM.Text}).SetMapCoordinates({txtMapCoordinates.Text}).WithTask(");
            sb.AppendLine($"    TimeSpaceTask.Create(TimeSpaceTaskType.{cboTaskType.SelectedItem}, {timeForTask.Value}).WithTaskText(\"{txtTaskText?.Text}\")" +
                          $"\n)");
            return sb.ToString();
        }
        public void AddPortalToMap(string mapName, Portal portal)
        {
            if (MapPortalPanels.ContainsKey(mapName))
            {
                Panel portalPanel = MapPortalPanels[mapName];
                portalPanel.Controls.Add(portal.CreatePortal());
            }
            else
            {
                MessageBox.Show($"Map '{mapName}' does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnAddPortal_Click(object sender, EventArgs e)
        {
            if (Portals.Count >= 4)
            {
                MessageBox.Show("You don't need more than 4 Portals.");
                return;
            }

            var portal = new Portal(
                this.MapName,
                "DefaultTo",
                "Type1",
                "North",
                0,
                0,
                0,
                0,
                getMapNames,
                this  
            );

            Portals.Add(portal);
            this.portalPanel.Controls.Add(portal.CreatePortal());
            RefreshLeverPortalComboboxes(sender, e, true);
        }
        private void BtnRemovePortal_Click(object sender, EventArgs e)
        {
            if (Portals.Count > 0)
            {
                var lastPortal = Portals.Last();
                Portals.Remove(lastPortal);
                portalPanel.Controls.Remove(lastPortal.Panel);
                portalPanel.Refresh();
            }
            RefreshLeverPortalComboboxes(sender,e, true);
        }
        public void BtnSavePortal_Click(Object sender, EventArgs e)
        {
            var localPortalScript = new StringBuilder();
            var addPortalScript = new StringBuilder();
            allPortalsList.Clear();
            lockedPortalsList.Clear();
            foreach (var mapTab in mapTabs)
            {
                foreach (var portal in mapTab.Portals)
                {
                    // Skip if any essential fields are empty or null
                    if (portal.cboMapFrom.SelectedItem == null ||
                        portal.cboMapTo.SelectedItem == null ||
                        portal.cboPortalType.SelectedItem == null ||
                        portal.cboMinimapOrientation.SelectedItem == null ||
                        string.IsNullOrWhiteSpace(portal.txtFromX.Text) ||
                        string.IsNullOrWhiteSpace(portal.txtFromY.Text))
                    {
                        continue;
                    }

                    portal.MapFrom = portal.cboMapFrom.SelectedItem.ToString();
                    portal.MapTo = portal.cboMapTo.SelectedItem.ToString();
                    portal.PortalType = portal.cboPortalType.SelectedItem.ToString();
                    portal.MinimapOrientation = portal.cboMinimapOrientation.SelectedItem.ToString();
                    portal.FromX = int.Parse(portal.txtFromX.Text);
                    portal.FromY = int.Parse(portal.txtFromY.Text);

                    if (portal.MapTo == "UNKNOWN")
                    {
                        portal.ToX = 1;
                        portal.ToY = 1;
                    }
                    else
                    {
                        // Skip if destination coordinates are empty when MapTo is not UNKNOWN
                        if (string.IsNullOrWhiteSpace(portal.txtToX?.Text) ||
                            string.IsNullOrWhiteSpace(portal.txtToY?.Text))
                        {
                            continue;
                        }
                        portal.ToX = int.Parse(portal.txtToX.Text);
                        portal.ToY = int.Parse(portal.txtToY.Text);
                    }

                    // Generate scripts and add to StringBuilder  
                    localPortalScript.AppendLine(portal.GenerateLocalPortalScript());
                    addPortalScript.AppendLine(portal.GenerateAddPortalScript());
                    allPortalsList.Add(portal.GeneratePortalIdentifier());
                    if (portal.PortalType == "Locked")
                    {
                        lockedPortalsList.Add(portal.GeneratePortalIdentifier());
                    }
                }
            }
            File.WriteAllText("localPortals.lua", localPortalScript.ToString());
            File.WriteAllText("addPortals.lua", addPortalScript.ToString());
            RefreshLeverPortalComboboxes(sender, e, false);
        }

        private void BtnAddEvent_Click(object sender, EventArgs e)
        {
            var monster = new Monster(MapName);
            MonsterEvents.Add(monster);

            // Add a new row to the existing DataGridView  
            monsterDataGridView.Rows.Add();
        }
        private void BtnRemoveEvent_Click(object sender, EventArgs e)
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
        public void BtnSaveMonsterAndObjective_Click(object sender, EventArgs e)
        {
            var mapScripts = monsterManager.ProcessMonsterData(Parent.Controls.OfType<TabPage>().ToList());

            // Process objectives (kept in CustomTabPage as it's separate from monster logic)
            ProcessObjectives(mapScripts);

            // Process map join events
            ProcessMapJoinEvents(mapScripts);

            // Save final script
            var finalScript = new StringBuilder();
            foreach (var mapName in mapScripts.Keys)
            {
                finalScript.Append(mapScripts[mapName].ToString());
            }
            File.WriteAllText("CombinedEvents.lua", finalScript.ToString());
        }
        private void ProcessObjectives(Dictionary<string, StringBuilder> mapScripts)
        {
            foreach (var mapTab in mapTabs)
            {
                string mapName = mapTab.MapName;
                if (!mapScripts.ContainsKey(mapName))
                {
                    mapScripts[mapName] = new StringBuilder();
                }
                var objects = new List<string>();
                foreach (var mapObject in mapTab.Objects)
                {
                    if (string.IsNullOrEmpty(mapObject.ObjectType) || mapObject.GetX() == 1500 || mapObject.GetY() == 1500)
                    {
                        continue;
                    }
                    string objectScript = mapObject.GenerateObjectiveScript();
                    if (!string.IsNullOrEmpty(objectScript))
                    {
                        objects.Add(objectScript);
                    }
                }
                if (objects.Any())
                {
                    mapScripts[mapName].AppendLine($"{mapName}.AddObjects({{");
                    mapScripts[mapName].AppendLine(string.Join(", \n", objects));
                    mapScripts[mapName].AppendLine("})");
                }
            }
        }
    private void BtnAddObjective_Click(object sender, EventArgs e)
        {
            if (Objects.Count >= 4)
            {
                MessageBox.Show("You dont need more than 4 Objects.");
                return;
            }
            var Object = new MapObject(MapName, "Object", 0, 0, allPortalsList);
            Objects.Add(Object);
            this.objectivePanel.Controls.Add(Object.CreateObject());
        }
        private void BtnRemoveObjective_Click(object sender, EventArgs e)
        {
            if (Objects.Count > 0)
            {
                var lastObject = Objects.Last();
                Objects.Remove(lastObject);
                objectivePanel.Controls.Remove(lastObject.Panel);
                objectivePanel.Refresh();
            }
        }

        public void SaveAllValues(object sender, EventArgs e)
        {
            BtnSaveMonsterAndObjective_Click(sender, e);
            BtnSavePortal_Click(sender, e);
        }
        public void RefreshLeverPortalComboboxes(object sender, EventArgs e, bool shouldsave)
        {
            if (shouldsave == true)
            {
                SaveAllValues(sender, e);
            }
            lockedPortalsList.Clear();
            lockedPortalsList.Add("");
            foreach (var tab in mapTabs)
            {
                foreach (var portal in tab.Portals)
                {
                    if (portal.cboMapFrom?.SelectedItem != null &&
                        portal.cboMapTo?.SelectedItem != null && portal.PortalType == "Locked")
                    {
                        lockedPortalsList.Add(portal.GeneratePortalIdentifier());
                    }
                }
            }
            foreach (var mapObject in Objects)
            {
                mapObject.UpdatePortalComboboxes(lockedPortalsList);
            }
            foreach (var portal in Portals)
            {
                portal.RefreshMapComboboxes();
            }

            if (taskEventManagerForm != null)
            {
                taskEventManagerForm.UpdatePortalComboboxes(lockedPortalsList);
            }
        }
    }
}
