using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private FlowLayoutPanel portalPanel;
        private DataGridView monsterDataGridView;
        private FlowLayoutPanel eventPanel;
        private FlowLayoutPanel objectivePanel;
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
            var btnAddEvent = new Button { Text = "Add Monster Event", Location = new Point(10, 680) };
            btnAddEvent.Click += BtnAddEvent_Click;
            var btnRemoveEvent = new Button { Text = "Remove Last Monster Event", Location = new Point(150, 680) };
            btnRemoveEvent.Click += BtnRemoveEvent_Click;
            var btnSaveMonster = new Button { Text = "Save Monsters", Location = new Point(290, 680) };
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
        }
        private void InitializeDataGridView()
        {
            monsterDataGridView = new DataGridView
            {
                Width = 800,
                Height = 200,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false
            };

            monsterDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vnum", HeaderText = "Vnum" });
            monsterDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "X", HeaderText = "X" });
            monsterDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Y", HeaderText = "Y" });
            monsterDataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "AdditionalAttribute",
                HeaderText = "Additional Attribute",
                Items =
                {
                    "SpawnAfterMobsKilled", "WithCustomLevel", "SpawnAfterTaskStart",
                    "OnThreeFourthsHP", "OnHalfHp", "OnQuarterHp", "none"
                }
            });
            monsterDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "AdditionalValue", HeaderText = "Additional Value" });
            monsterDataGridView.Columns.Add(new DataGridViewCheckBoxColumn { Name = "AsTarget", HeaderText = "As Target" });

            eventPanel.Controls.Add(monsterDataGridView);
        }
        public string GenerateMapScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"local {MapName} = Map.Create().WithMapId({txtMapVNUM.Text}).SetMapCoordinates({txtMapCoordinates.Text}).WithTask(");
            sb.AppendLine($"    TimeSpaceTask.Create(TimeSpaceTaskType.{cboTaskType.SelectedItem}).WithTaskText(\"{txtTaskText?.Text}\")" +
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
            RefreshLeverPortalComboboxes(sender,e);
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
            RefreshLeverPortalComboboxes(sender,e);
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
            // Generate monster scripts  
            foreach (var tab in mapTabs)
            {
                var monsterDataGridView = tab.monsterDataGridView;
                string mapName = tab.MapName;

                if (!mapScripts.ContainsKey(mapName))
                {
                    mapScripts[mapName] = new StringBuilder();
                }

                var monsters = new List<string>();
                foreach (DataGridViewRow row in monsterDataGridView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var monster = new Monster(mapName)
                        {
                            Vnum = row.Cells["Vnum"].Value?.ToString(),
                            X = int.Parse(row.Cells["X"].Value?.ToString()),
                            Y = int.Parse(row.Cells["Y"].Value?.ToString()),
                            AdditionalAttribute = row.Cells["AdditionalAttribute"].Value?.ToString(),
                            AdditionalValue = row.Cells["AdditionalValue"].Value?.ToString(),
                            AsTarget = Convert.ToBoolean(row.Cells["AsTarget"]?.Value)
                        };
                        var monsterScript = monster.GenerateMonsterScript(row);
                        monsters.Add(monsterScript);
                        MonsterEvents.Add(monster);
                    }
                }

                if (monsters.Any())
                {
                    mapScripts[mapName].AppendLine($"{mapName}.AddMonsters({{");
                    mapScripts[mapName].AppendLine(string.Join(", \n", monsters));
                    mapScripts[mapName].AppendLine("})");
                }
            }

            // Generate object scripts  
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
                    // Skip if the object is not properly configured  
                    if (string.IsNullOrEmpty(mapObject.ObjectType) ||
                        mapObject.GetX() == 1500 ||
                        mapObject.GetY() == 1500)
                    {
                        continue;
                    }

                    // Generate script for the object  
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

            // Generate OnMapJoin scripts  
            foreach (var mapTab in mapTabs)
            {
                string mapName = mapTab.MapName;

                if (!mapScripts.ContainsKey(mapName))
                {
                    mapScripts[mapName] = new StringBuilder();
                }

                mapScripts[mapName].AppendLine($"{mapName}.OnMapJoin({{");
                mapScripts[mapName].AppendLine($"    Event.TryStartTaskForMap({mapName}),");
                mapScripts[mapName].AppendLine("})");
            }

            // Save to file  
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
        public void RefreshLeverPortalComboboxes(object sender, EventArgs e)
        {
            SaveAllValues(sender, e);
            // Clear and regenerate the allPortalsList
            allPortalsList.Clear();

            // Get portals from ALL tabs, not just the current one
            foreach (var tab in mapTabs)  // Make sure mapTabs is accessible here
            {
                foreach (var portal in tab.Portals)
                {
                    // Only add to portal list if the portal has valid selections
                    if (portal.cboMapFrom?.SelectedItem != null &&
                        portal.cboMapTo?.SelectedItem != null)
                    {
                        allPortalsList.Add(portal.GeneratePortalIdentifier());
                    }
                }
            }

            // Update each MapObject's portal comboboxes in the current tab
            foreach (var mapObject in Objects)
            {
                mapObject.UpdatePortalComboboxes(allPortalsList);
            }

            // Update each Portal's map comboboxes in the current tab
            foreach (var portal in Portals)
            {
                portal.RefreshMapComboboxes();
            }
        }
    }
}
