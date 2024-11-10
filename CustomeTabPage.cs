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
    public class CustomeTabPage : TabPage
    {
        public string MapName { get; private set; }
        public Func<List<string>> getMapNames;
        public List<Portal> Portals { get; private set; } = new List<Portal>();
        public List<MapObject> Objects { get; private set; } = new List<MapObject>();
        private List<string> allPortalsList = new List<string>();
        public List<Monster> MonsterEvents { get; private set; } = new List<Monster>();
        private List<DataGridView> monsterDataGrids;
        private List<CustomeTabPage> mapTabs = Form1.mapTabs;
        private Task _gridCreationTask;
        private readonly TextBox txtMapVNUM;
        private readonly TextBox txtMapCoordinates;
        private readonly TextBox txtTaskText;
        private readonly ComboBox cboTaskType;
        private ComboBox[] taskFinishPortals;
        private ComboBox[] taskFailPortals;
        private ComboBox eventTypeComboBox;
        private readonly NumericUpDown addTimeEvent;
        private readonly CheckBox despawnAllMobsInRoom;
        private readonly NumericUpDown timeForTask;
        private readonly NumericUpDown removeTimeEvent;
        private FlowLayoutPanel portalPanel;
        private DataGridView monsterDataGridView;
        private FlowLayoutPanel eventPanel;
        private FlowLayoutPanel objectivePanel;
        private static List<Point> sharedTakenPositions = new List<Point>();
        private Point? currentPosition = null;
        private static readonly object positionsLock = new object();
        private Dictionary<string, List<string>> eventManagerScripts = new Dictionary<string, List<string>>();
        private bool isDisposed = false;
        private Button btnAddAttribute;
        private Button applyButton;
        private NumericUpDown waveCountInput;
        private NumericUpDown waveDelayInput;
        public CheckBox useWavesCheckbox;
        private void DisplayMapGrid(MapDataDTO mapData)
        {
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).First();
            // Populate cellColors array with your grid data
            mapGridPanel.SetGrid(mapData.Width, mapData.Height, mapData.Grid);
        }

        private void MapGridPanel_CellClicked(object sender, CellClickedEventArgs e)
        {
            // Handle the cell click event
            MessageBox.Show($"Cell clicked at ({e.CellX}, {e.CellY})");
        }
        public CustomeTabPage(string MapName,Form1 form, Func<List<string>> getMapNames)
        {
            MonsterEvents = new List<Monster>();
            Text = MapName;
            this.getMapNames = getMapNames;
            this.MapName = MapName;
            var containerPanel = new Panel { Dock = DockStyle.Fill };
            var leftPanel = new Panel { Width = 1000, Dock = DockStyle.Left };
            var lblMapVNUM = new Label { Text = "Map Vnum:", Location = new Point(10, 10) };
            txtMapVNUM = new TextBox { Name = "txtMapVNUM", Location = new Point(150, 10), Width = 200 };
            var btnLoadMap = new Button { Text = "Load Map", Location = new Point(360, 10) };
            btnLoadMap.Click += (sender, e) =>
            {
                if (int.TryParse(this.Controls.Find("txtMapVNUM",true).First().Text, out int mapVnum))
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
            };
            var lblMapCoordinates = new Label { Text = "Map Coordinates:", Location = new Point(10, 40) };
            txtMapCoordinates = new TextBox { Location = new Point(150, 40), Width = 200 };
            var btnSelectCoordinates = new Button
            {
                Text = "...",
                Location = new Point(txtMapCoordinates.Right + 10, 40),
                Width = 30,
                Height = txtMapCoordinates.Height
            };
            btnSelectCoordinates.Click += BtnSelectCoordinates_Click;
            var lblTaskType = new Label { Text = "Task Type:", Location = new Point(10, 70) };
            cboTaskType = new ComboBox
            {
                Location = new Point(150, 70),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "None", "KillAllMonsters", "Survive" }
            };
            var lblTaskText = new Label { Text = "Task Text:", Location = new Point(10, 100) };
            txtTaskText = new TextBox { Location = new Point(150, 100), Width = 200 };
            var lblTimeforTask = new Label { Text = "Time for Task:", Location = new Point(360, 105), Width = 80 };
            timeForTask = new NumericUpDown
            {
                Width = 50,
                Location = new Point(440, 100),
                Maximum = new decimal(600),
                Increment = new decimal(10)
            };

            // Initialize portal panel  
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
            var btnAddPortal = new Button { Text = "Add Portal", Location = new Point(10, 390) };
            btnAddPortal.Click += BtnAddPortal_Click;
            var btnRemovePortal = new Button { Text = "Remove Last Portal", Location = new Point(150, 390) };
            btnRemovePortal.Click += BtnRemovePortal_Click;
            var btnSavePortal = new Button { Text = "Save Portals", Location = new Point(290, 390) };
            btnSavePortal.Click += BtnSavePortal_Click;

            // Initialize event panel  
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
            var btnAddEvent = new Button { Text = "Add Monster Event", Location = new Point(10, 730) };
            btnAddEvent.Click += BtnAddEvent_Click;
            var btnRemoveEvent = new Button { Text = "Remove Last Monster Event", Location = new Point(150, 730) };
            btnRemoveEvent.Click += BtnRemoveEvent_Click;
            var btnSaveMonster = new Button { Text = "Save Monsters", Location = new Point(290, 730) };
            btnSaveMonster.Click += BtnSaveMonsterAndObjective_Click;

            leftPanel.Controls.Add(lblMapVNUM);
            leftPanel.Controls.Add(txtMapVNUM);
            leftPanel.Controls.Add(btnLoadMap);
            leftPanel.Controls.Add(lblMapCoordinates);
            leftPanel.Controls.Add(txtMapCoordinates);
            leftPanel.Controls.Add(lblTaskType);
            leftPanel.Controls.Add(cboTaskType);
            leftPanel.Controls.Add(lblTaskText);
            leftPanel.Controls.Add(txtTaskText);
            leftPanel.Controls.Add(portalPanel);
            leftPanel.Controls.Add(btnAddPortal);
            leftPanel.Controls.Add(btnRemovePortal);
            leftPanel.Controls.Add(btnSavePortal);
            leftPanel.Controls.Add(eventPanel);
            leftPanel.Controls.Add(btnAddEvent);
            leftPanel.Controls.Add(btnRemoveEvent);
            leftPanel.Controls.Add(btnSaveMonster);
            leftPanel.Controls.Add(btnSelectCoordinates);
            leftPanel.Controls.Add(lblTimeforTask);
            leftPanel.Controls.Add(timeForTask);

            // Create right panel for map grid and objectives  
            var rightPanel = new Panel { Dock = DockStyle.Fill };
            var mapGridPanel = new MapGridPanel();

            mapGridPanel.CellClicked += MapGridPanel_CellClicked;
            var lblHoverPosition = new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Hover Position: " };
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
            var btnAddObjective = new Button { Text = "Add Objective", Location = new Point(10, 470) };
            btnAddObjective.Click += BtnAddObjective_Click;
            var btnRemoveObjective = new Button { Text = "Remove Last Objective", Location = new Point(100, 470) };
            btnRemoveObjective.Click += BtnRemoveObjective_Click;
            var btnSaveObjective = new Button { Text = "Save Objectives", Location = new Point(200, 470) };
            btnSaveObjective.Click += BtnSaveMonsterAndObjective_Click;

            rightPanel.Controls.Add(mapGridPanel);
            rightPanel.Controls.Add(objectivePanel);
            rightPanel.Controls.Add(btnAddObjective);
            rightPanel.Controls.Add(btnRemoveObjective);
            rightPanel.Controls.Add(btnSaveObjective);
            rightPanel.Controls.Add(lblHoverPosition);
            containerPanel.Controls.Add(rightPanel);
            containerPanel.Controls.Add(leftPanel);
            this.Controls.Add(containerPanel);
            InitializeDataGridView();
            var btnManageEvents = new Button
            {
                Text = "Manage Events",
                Location = new Point(360, 70),
                Width = 100
            };
            btnManageEvents.Click += (s, e) =>
            {
                var eventManager = new TaskEventManagerForm(MapName, allPortalsList);
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
            };
            leftPanel.Controls.Add(btnManageEvents);
        }
        private void InitializeDataGridView()
        {
            // Initialize the useWavesCheckbox  
            useWavesCheckbox = new CheckBox
            {
                Text = "Use Waves",
                Location = new Point(10, 10),
                AutoSize = true
            };
            useWavesCheckbox.CheckedChanged += UseWavesCheckbox_CheckedChanged;

            // Initialize the waveCountLabel  
            var waveCountLabel = new Label
            {
                Text = "Number of Waves:",
                Location = new Point(120, 12),
                AutoSize = true
            };

            // Initialize the waveCountInput  
            waveCountInput = new NumericUpDown
            {
                Location = new Point(230, 10),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Enabled = false
            };

            // Initialize the waveDelayLabel  
            var waveDelayLabel = new Label
            {
                Text = "Wave Delay (seconds):",
                Location = new Point(300, 12),
                AutoSize = true
            };

            // Initialize the waveDelayInput  
            waveDelayInput = new NumericUpDown
            {
                Location = new Point(430, 10),
                Width = 60,
                Minimum = 0,
                Maximum = 300,
                Value = 30,
                Enabled = false
            };

            // Initialize the wavePanel  
            var wavePanel = new Panel
            {
                Location = new Point(0, 0), // Top of the eventPanel  
                Width = 800,
                Height = 40
            };

            // Add controls to the wavePanel  
            wavePanel.Controls.AddRange(new Control[] { useWavesCheckbox, waveCountLabel, waveCountInput, waveDelayLabel, waveDelayInput });

            // Initialize the btnAddAttribute button  
            btnAddAttribute = new Button
            {
                Text = "Manage Attributes",
                Location = new Point(0, wavePanel.Bottom + 10),
                Width = 120,
                Height = 30
            };
            btnAddAttribute.Click += BtnAddAttribute_Click;

            // Initialize the monsterDataGridView  
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
        new DataGridViewTextBoxColumn { Name = "Wave", HeaderText = "Wave" },
        new DataGridViewCheckBoxColumn { Name = "AsTarget", HeaderText = "As Target" },
        new DataGridViewTextBoxColumn
        {
            Name = "Attributes",
            HeaderText = "Additional Attributes",
            ReadOnly = true
        }
            });

            monsterDataGridView.CellDoubleClick += MonsterDataGridView_CellDoubleClick;

            // Add controls to the eventPanel  
            eventPanel.Controls.Add(wavePanel);
            eventPanel.Controls.Add(btnAddAttribute);
            eventPanel.Controls.Add(monsterDataGridView);

            // Ensure the eventPanel is tall enough to display all controls  
            eventPanel.Height = monsterDataGridView.Bottom + 20;
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
                        txtMapCoordinates.Text = $"{selectedPos.X}, {selectedPos.Y}";
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
            monsterDataGridView.Columns["Wave"].Visible = useWavesCheckbox.Checked;
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
        private void BtnAddPortal_Click(object sender, EventArgs e)
        {
            if (Portals.Count >= 4)
            {
                MessageBox.Show("You dont need more than 4 Portals.");
                return;
            }
            var portal = new Portal("DefaultFrom", "DefaultTo", "Type1", "North", 0, 0, 0, 0, getMapNames);
            Portals.Add(portal);
            this.portalPanel.Controls.Add(portal.CreatePortal());
            RefreshLeverPortalComboboxes(sender,e, true);
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
            var mapScripts = new Dictionary<string, StringBuilder>();
            MonsterEvents.Clear();

            foreach (var tab in mapTabs)
            {
                var monsterDataGridView = tab.monsterDataGridView;
                string mapName = tab.MapName;

                if (!mapScripts.ContainsKey(mapName))
                {
                    mapScripts[mapName] = new StringBuilder();
                }

                var waves = new Dictionary<int, List<string>>();
                foreach (DataGridViewRow row in monsterDataGridView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var wave = int.Parse(row.Cells["Wave"].Value?.ToString());
                        var monster = new Monster(mapName)
                        {
                            Vnum = int.Parse(row.Cells["Vnum"].Value?.ToString()),
                            X = int.Parse(row.Cells["X"].Value?.ToString()),
                            Y = int.Parse(row.Cells["Y"].Value?.ToString()),
                            AsTarget = Convert.ToBoolean(row.Cells["AsTarget"]?.Value)
                        };
                        var attributesCell = row.Cells["Attributes"].Value?.ToString();
                        if (!string.IsNullOrEmpty(attributesCell))
                        {
                            var attributes = attributesCell.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var attribute in attributes)
                            {
                                var parts = attribute.Split('=');
                                if (parts.Length == 2)
                                {
                                    monster.Attributes[parts[0].Trim()] = parts[1].Trim();
                                }
                            }
                        }
                        var monsterScript = monster.GenerateMonsterScript(row);
                        if (!waves.ContainsKey(wave))
                        {
                            waves[wave] = new List<string>();
                        }
                        waves[wave].Add(monsterScript);
                        MonsterEvents.Add(monster);
                    }
                }

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

            foreach (var mapTab in mapTabs)
            {
                string mapName = mapTab.MapName;
                if (!mapScripts.ContainsKey(mapName))
                {
                    mapScripts[mapName] = new StringBuilder();
                }
                mapScripts[mapName].AppendLine($"{mapName}.OnMapJoin({{");
                mapScripts[mapName].AppendLine($"    Event.TryStartTaskForMap({mapName}),");
                // Append event manager scripts here  
                if (eventManagerScripts.ContainsKey(mapName))
                {
                    foreach (var script in eventManagerScripts[mapName])
                    {
                        mapScripts[mapName].AppendLine(script);
                    }
                }
                mapScripts[mapName].AppendLine("})");
            }

            var finalScript = new StringBuilder();
            foreach (var mapName in mapScripts.Keys)
            {
                finalScript.Append(mapScripts[mapName].ToString());
            }
            File.WriteAllText("CombinedEvents.lua", finalScript.ToString());
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
            allPortalsList.Clear();
            allPortalsList.Add("");

            foreach (var tab in mapTabs)
            {
                foreach (var portal in tab.Portals)
                {
                    if (portal.cboMapFrom?.SelectedItem != null &&
                        portal.cboMapTo?.SelectedItem != null)
                    {
                        allPortalsList.Add(portal.GeneratePortalIdentifier());
                    }
                }
            }

            foreach (var mapObject in Objects)
            {
                mapObject.UpdatePortalComboboxes(allPortalsList);
            }

            foreach (var portal in Portals)
            {
                portal.RefreshMapComboboxes();
            }
       
        }
    }
}
