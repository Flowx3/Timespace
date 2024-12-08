﻿using System;
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
        public bool IsWalkable(int x, int y)
        {
            string errorMessage;
            return !IsBlockingZone(x, y, out errorMessage);
        }
        public (int x, int y) GenerateWalkablePosition()
        {
            // If grid is empty or null, return default position
            if (_grid == null || _grid.Length == 0)
            {
                return (0, 0);
            }

            Random random = new Random();
            int maxAttempts = _width * _height; // Maximum attempts to find a walkable position
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                int x = random.Next(0, _width);
                int y = random.Next(0, _height);

                string errorMessage;
                if (!IsBlockingZone(x, y, out errorMessage))
                {
                    return (x, y);
                }

                attempts++;
            }

            // If no walkable position found after max attempts, throw exception
            throw new InvalidOperationException("Could not find a walkable position in the grid after maximum attempts.");
        }
        public bool IsBlockingZone(int x, int y, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check if position is within bounds
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                errorMessage = $"Position ({x}, {y}) is out of bounds. Grid size is {_width}x{_height}.";
                return true;
            }

            // Get the grid value at the position
            int index = y * _width + x;
            byte value = _grid[index];

            // Check if the position has any flags (not white)
            Color positionColor = GetColor(value);
            if (positionColor != Color.White)
            {
                errorMessage = $"Position ({x}, {y}) is invalid. It contains a blocking flag (Color: {positionColor}).";
                return true;
            }

            return false;
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
        { 0x1, Color.Gray },     // IsWalkingDisabled
        { 0x2, Color.Red },      // IsAttackDisabledThrough
        { 0x4, Color.Blue },     // UnknownYet
        { 0x8, Color.Green },    // IsMonsterAggroDisabled
        { 0x10, Color.Purple },  // IsPvpDisabled
        { 0x20, Color.Orange },  // MateDoll
        { 0x40, Color.LightBlue }, // Portal
        { 0x80, Color.Red },     // Monster
        { 0x100, Color.Purple }, // Target Monster
        { 0x200, Color.Gold }    // Objective
    };

        // Add methods to update grid with new element types
        public void MarkPortal(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0x40; // Portal flag
                Invalidate();
            }
        }

        public void MarkMonster(int x, int y, bool isTarget = false)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = (byte)(isTarget ? 0x100 : 0x80); // Target or regular monster flag
                Invalidate();
            }
        }

        public void MarkObjective(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0x200; // Objective flag
                Invalidate();
            }
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height && _grid != null;
        }

        // Method to clear specific type of marking
        public void ClearMarking(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0; // Reset to default (white)
                Invalidate();
            }
        }

        // Helper method to get existing marking at position
        public byte GetMarking(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _grid[y * _width + x];
            }
            return 0;
        }
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
        private readonly MapEventGenerator eventGenerator;
        public DataGridView MonsterDataGridView => monsterDataGridView;
        public string MapName { get; private set; }
        public Func<List<string>> getMapNames;
        public Dictionary<string, Panel> MapPortalPanels { get; set; }
        public List<Portal> Portals { get; private set; } = new List<Portal>();
        public List<MapObject> Objects { get; private set; } = new List<MapObject>();
        public List<Monster> MonsterEvents { get; private set; } = new List<Monster>();

        private List<string> allPortalsList = new List<string>();
        private List<string> lockedPortalsList = new List<string>();
        private List<CustomTabPage> mapTabs = Form1.mapTabs;
        private static List<Point> sharedTakenPositions = new List<Point>();
        private static readonly object positionsLock = new object();
        public Dictionary<string, List<string>> eventManagerScripts = new Dictionary<string, List<string>>();
        private bool isDisposed = false;

        private TextBox txtMapVNUM;
        private TextBox txtMapCoordinates;
        private TextBox txtTaskText;
        private ComboBox cboTaskType;
        private NumericUpDown timeForTask;
        private FlowLayoutPanel portalPanel;
        private DataGridView monsterDataGridView;
        private FlowLayoutPanel eventPanel;
        private FlowLayoutPanel objectivePanel;
        private Point? currentPosition = null;
        private Button btnAddAttribute;
        private NumericUpDown waveCountInput;
        private NumericUpDown waveDelayInput;
        public CheckBox useWavesCheckbox;

        public CustomTabPage(string mapName, Form1 form, Func<List<string>> getMapNames)
        {
            MonsterEvents = new List<Monster>();
            Text = mapName;
            this.getMapNames = getMapNames;
            this.MapName = mapName;
            eventGenerator = new MapEventGenerator(waveDelayInput, eventManagerScripts);
            MapPortalPanels = new Dictionary<string, Panel>();
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
            CreateButton("Save Portals", new Point(290, 390), SaveAllValues),
            eventPanel,
            CreateButton("Add Monster", new Point(10, 730), BtnAddEvent_Click),
            CreateButton("Remove Last Monster", new Point(150, 730), BtnRemoveEvent_Click),
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
            List<string> existingEvents = null;
            if (eventManagerScripts.ContainsKey(MapName))
            {
                existingEvents = eventManagerScripts[MapName];
            }

            var eventManager = new TaskEventManagerForm(MapName, lockedPortalsList, existingEvents);
            if (eventManager.ShowDialog() == DialogResult.OK)
            {
                string generatedScript = eventManager.Tag as string;
                if (!string.IsNullOrEmpty(generatedScript))
                {
                    if (!eventManagerScripts.ContainsKey(MapName))
                    {
                        eventManagerScripts[MapName] = new List<string>();
                    }
                    eventManagerScripts[MapName].Clear(); // Clear existing events
                    eventManagerScripts[MapName].Add(generatedScript); // Add the new event
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
        public void LoadMap(Form1 form)
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
            Dictionary<string, string> currentAttributes = new Dictionary<string, string>();

            if (monsterDataGridView.CurrentRow != null &&
                MonsterEvents != null &&
                MonsterEvents.Count > monsterDataGridView.CurrentRow.Index)
            {
                currentAttributes = MonsterEvents[monsterDataGridView.CurrentRow.Index].Attributes;
            }

            var attributeForm = new MonsterAttributeForm(currentAttributes);

            if (attributeForm.ShowDialog() == DialogResult.OK)
            {
                if (monsterDataGridView.CurrentRow != null &&
                    MonsterEvents != null &&
                    MonsterEvents.Count > monsterDataGridView.CurrentRow.Index)
                {
                    var monster = MonsterEvents[monsterDataGridView.CurrentRow.Index];
                    monster.Attributes.Clear();
                    foreach (var attr in attributeForm.SelectedAttributes)
                    {
                        monster.Attributes[attr.Key] = attr.Value;
                    }
                    UpdateAttributeDisplay(monsterDataGridView.CurrentRow.Index);
                }
                else
                {
                    MonsterEvents = MonsterEvents ?? new List<Monster>();
                    var monster = new Monster(MapName);
                    foreach (var attr in attributeForm.SelectedAttributes)
                    {
                        monster.Attributes[attr.Key] = attr.Value;
                    }
                    MonsterEvents.Add(monster);
                    monsterDataGridView.Refresh();
                    UpdateAttributeDisplay(MonsterEvents.Count - 1);
                }
                monsterDataGridView.Refresh();
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
            var test = mapTabs.FirstOrDefault(x => x.MapName == mapName);
            if (test != null)
            {
                test.portalPanel.Controls.Add(portal.CreatePortal());
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
            SaveAndRefreshPortals(sender, e, false);
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
            SaveAndRefreshPortals(sender,e, false);
        }
        public void SaveAndRefreshPortals(object sender, EventArgs e, bool generateScripts)
        {
            if (generateScripts)
            {
                var localPortalScript = new StringBuilder();
                var addPortalScript = new StringBuilder();
                allPortalsList.Clear();
                lockedPortalsList.Clear();

                foreach (var mapTab in mapTabs)
                {
                    foreach (var portal in mapTab.Portals)
                    {
                        if (!IsValidPortal(portal))
                        {
                            continue;
                        }

                        UpdatePortalProperties(portal);

                        if (!IsValidDestination(portal))
                        {
                            continue;
                        }

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
            }
            else
            {
                lockedPortalsList.Clear();
                lockedPortalsList.Add("");

                foreach (var tab in mapTabs)
                {
                    foreach (var portal in tab.Portals)
                    {
                        if (portal.cboMapFrom?.SelectedItem != null &&
                            portal.cboMapTo?.SelectedItem != null &&
                            portal.PortalType == "Locked")
                        {
                            lockedPortalsList.Add(portal.GeneratePortalIdentifier());
                        }
                    }
                }
            }

            // Update UI components
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

        private bool IsValidPortal(Portal portal)
        {
            return portal.cboMapFrom?.SelectedItem != null &&
                   portal.cboMapTo?.SelectedItem != null &&
                   portal.cboPortalType?.SelectedItem != null &&
                   portal.cboMinimapOrientation?.SelectedItem != null &&
                   !string.IsNullOrWhiteSpace(portal.txtFromX?.Text) &&
                   !string.IsNullOrWhiteSpace(portal.txtFromY?.Text);
        }

        private bool IsValidDestination(Portal portal)
        {
            if (portal.MapTo == "UNKNOWN")
            {
                portal.ToX = 1;
                portal.ToY = 1;
                return true;
            }

            return !string.IsNullOrWhiteSpace(portal.txtToX?.Text) &&
                   !string.IsNullOrWhiteSpace(portal.txtToY?.Text);
        }

        private void UpdatePortalProperties(Portal portal)
        {
            portal.MapFrom = portal.cboMapFrom.SelectedItem.ToString();
            portal.MapTo = portal.cboMapTo.SelectedItem.ToString();
            portal.PortalType = portal.cboPortalType.SelectedItem.ToString();
            portal.MinimapOrientation = portal.cboMinimapOrientation.SelectedItem.ToString();
            portal.FromX = int.Parse(portal.txtFromX.Text);
            portal.FromY = int.Parse(portal.txtFromY.Text);

            if (portal.MapTo != "UNKNOWN")
            {
                portal.ToX = int.Parse(portal.txtToX.Text);
                portal.ToY = int.Parse(portal.txtToY.Text);
            }
        }

        private void BtnAddEvent_Click(object sender, EventArgs e)
        {
            var monster = new Monster(MapName);
            monsterDataGridView.Rows.Add();
        }
        private void BtnRemoveEvent_Click(object sender, EventArgs e)
        {
            if (monsterDataGridView.Rows.Count > 0)
            {
                monsterDataGridView.Rows.RemoveAt(monsterDataGridView.Rows.Count - 1);
            }
        }
        public void BtnSaveMonsterAndObjective_Click(object sender, EventArgs e)
        {
            if (mapTabs == null)
                return;
            if (ValidateAndUpdateMonsterPositions())
            {
                eventGenerator.GenerateEvents(mapTabs);
            }
        }
        private bool ValidateAndUpdateMonsterPositions()
        {
            bool hasErrors = false;
            List<string> errorMessages = new List<string>();
            List<(DataGridViewRow Row, int Vnum, int X, int Y)> invalidPositions = new List<(DataGridViewRow, int, int, int)>();
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).First();
            foreach (DataGridViewRow row in monsterDataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                try
                {
                    var vnum = row.Cells["vnum"].Value?.ToString() ?? "";

                    int x = -1;
                    int y = -1;

                    if (string.IsNullOrWhiteSpace(vnum))
                        continue;

                    // Parse X and Y values
                    bool needsPosition = !int.TryParse(row.Cells["X"].Value?.ToString(), out x) ||
                                       !int.TryParse(row.Cells["Y"].Value?.ToString(), out y);

                    if (needsPosition)
                    {
                        // Generate new walkable position
                        try
                        {
                            var position = mapGridPanel.GenerateWalkablePosition();
                            x = position.x;
                            y = position.y;
                            row.Cells["X"].Value = x;
                            row.Cells["Y"].Value = y;
                        }
                        catch (InvalidOperationException ex)
                        {
                            errorMessages.Add($"Could not find walkable position for monster VNUM {vnum}: {ex.Message}");
                            hasErrors = true;
                            continue;
                        }
                    }
                        string validationError;
                        if (mapGridPanel.IsBlockingZone(x, y, out validationError))
                        {
                            errorMessages.Add($"Invalid position for monster VNUM {vnum}: {validationError}");
                            hasErrors = true;
                            HighlightInvalidRow(row, true);
                            invalidPositions.Add((row, int.Parse(vnum), x, y));
                        }
                        else
                        {
                            HighlightInvalidRow(row, false);
                        }
                }
                catch (Exception ex)
                {
                    errorMessages.Add($"Error processing row {row.Index + 1}: {ex.Message}");
                    hasErrors = true;
                    HighlightInvalidRow(row, true);
                }
            }
            if (invalidPositions.Any())
            {
                string message = "Invalid positions were found for some monsters.\n\n" +
                               string.Join("\n", errorMessages) +
                               "\n\nDo you want to automatically replace these with random walkable positions?";

                var result = MessageBox.Show(message, "Invalid Positions Detected",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (var invalidPos in invalidPositions)
                    {
                        try
                        {
                            var (row, vnum, _, _) = invalidPos;
                            var newPosition = mapGridPanel.GenerateWalkablePosition();

                            row.Cells["X"].Value = newPosition.x;
                            row.Cells["Y"].Value = newPosition.y;
                            HighlightInvalidRow(row, false);

                            errorMessages.Add($"VNUM {vnum}: Position updated to ({newPosition.x}, {newPosition.y})");
                        }
                        catch (InvalidOperationException ex)
                        {
                            errorMessages.Add($"Could not find walkable position for VNUM {invalidPos.Vnum}: {ex.Message}");
                            hasErrors = true;
                        }
                    }
                    string statusMessage = "Position updates completed:\n\n" + string.Join("\n", errorMessages);
                    MessageBox.Show(statusMessage, "Position Update Status",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Save operation cancelled. Please adjust the positions manually.",
                                  "Operation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            return !hasErrors;
        }

        private void HighlightInvalidRow(DataGridViewRow row, bool isInvalid)
        {
            if (isInvalid)
            {
                row.DefaultCellStyle.BackColor = Color.LightPink;
                row.DefaultCellStyle.SelectionBackColor = Color.Pink;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            }
        }
        //private void ProcessObjectives(Dictionary<string, StringBuilder> mapScripts)
        //{
        //    foreach (var mapTab in mapTabs)
        //    {
        //        string mapName = mapTab.MapName;
        //        if (!mapScripts.ContainsKey(mapName))
        //        {
        //            mapScripts[mapName] = new StringBuilder();
        //        }
        //        var objects = new List<string>();
        //        foreach (var mapObject in mapTab.Objects)
        //        {
        //            if (string.IsNullOrEmpty(mapObject.ObjectType) || mapObject.GetX() == 1500 || mapObject.GetY() == 1500)
        //            {
        //                continue;
        //            }
        //            string objectScript = mapObject.GenerateObjectiveScript();
        //            if (!string.IsNullOrEmpty(objectScript))
        //            {
        //                objects.Add(objectScript);
        //            }
        //        }
        //        if (objects.Any())
        //        {
        //            mapScripts[mapName].AppendLine($"{mapName}.AddObjects({{");
        //            mapScripts[mapName].AppendLine(string.Join(", \n", objects));
        //            mapScripts[mapName].AppendLine("})");
        //        }
        //    }
        //}
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
            SaveAndRefreshPortals(sender, e, true);
        }
        public void SetMapVnum(int vnum)
        {
            this.txtMapVNUM.Text = vnum.ToString();
        }
        public void SetMapCoordinates(string coords)
        {
            this.txtMapCoordinates.Text = coords;
        }
        public void SetTaskType(string taskType)
        {
            this.cboTaskType.SelectedItem = taskType;
        }
        public void AddMonster(int vnum, int x, int y, bool asTarget, Dictionary<string, string> attributes)
        {
            var monster = new Monster(MapName)
            {
                Vnum = vnum,
                X = x,
                Y = y,
                AsTarget = asTarget,
                Attributes = attributes
            };

            int rowIndex = monsterDataGridView.Rows.Add();
            DataGridViewRow row = monsterDataGridView.Rows[rowIndex];
            row.Cells["Vnum"].Value = monster.Vnum;
            row.Cells["X"].Value = monster.X;
            row.Cells["Y"].Value = monster.Y;
            row.Cells["AsTarget"].Value = monster.AsTarget;
            row.Cells["Attributes"].Value = string.Join(", ", attributes.Select(a => $"{a.Key}={a.Value}"));

            MonsterEvents = MonsterEvents ?? new List<Monster>();
            MonsterEvents.Add(monster);
        }
        public void AddMapObject(string objectType, int x, int y, List<string> linkedPortals = null)
        {
            var mapObject = new MapObject(MapName, objectType, x, y, linkedPortals ?? new List<string>());

            Objects.Add(mapObject);

            this.objectivePanel.Controls.Add(mapObject.CreateObject());
 
            if (objectType == "Lever")
            {
                if (taskEventManagerForm != null)
                {
                    taskEventManagerForm.UpdatePortalComboboxes(lockedPortalsList);
                }
                else
                {
                    // Handle the null case appropriately
                }
            }
        }
    }
}
