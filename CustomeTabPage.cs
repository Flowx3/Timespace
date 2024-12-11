using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSpace
{
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
        private DataGridView npcDataGridView;
        private List<Npc> NpcEvents { get; set; } = new List<Npc>();
        private MapGridPanel _mapGridPanel;
        private byte[] originalGrid;
        private Form1 _mainForm;
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

        private TabControl mainTabControl;
        private TabPage monsterTabPage;
        private TabPage waveTabPage;
        private TabPage npcTabPage;

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

        private Panel wavePanel;
        private NumericUpDown waveCountInput;
        private NumericUpDown waveDelayInput;
        public CheckBox useWavesCheckbox;

        public CustomTabPage(string mapName, Form1 form, Func<List<string>> getMapNames)
        {
            _mainForm = form;
            MonsterEvents = new List<Monster>();
            Text = mapName;
            this.getMapNames = getMapNames;
            this.MapName = mapName;
            InitializeComponents(form);
            eventGenerator = new MapEventGenerator(waveDelayInput, eventManagerScripts);
            MapPortalPanels = new Dictionary<string, Panel>();
        }

        private void InitializeComponents(Form1 form)
        {
            var containerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(50, 50, 50) };
            var leftPanel = CreateLeftPanel(form);
            var rightPanel = CreateRightPanel();

            containerPanel.Controls.Add(rightPanel);
            containerPanel.Controls.Add(leftPanel);
            this.Controls.Add(containerPanel);
        }

        private Panel CreateLeftPanel(Form1 form)
        {
            var leftPanel = new Panel { Width = 1000, Dock = DockStyle.Left, BackColor = Color.FromArgb(50, 50, 50) };

            // Initialize controls
            txtMapVNUM = new TextBox { Name = "txtMapVNUM", Location = new Point(150, 10), Width = 200, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White };
            txtMapCoordinates = new TextBox { Location = new Point(150, 40), Width = 200, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White };
            cboTaskType = new ComboBox
            {
                Location = new Point(150, 70),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "None", "KillAllMonsters", "Survive" },
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };
            txtTaskText = new TextBox { Location = new Point(150, 100), Width = 200, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White };
            timeForTask = new NumericUpDown
            {
                Width = 50,
                Location = new Point(440, 100),
                Maximum = 600,
                Increment = 10,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            // Portal panel remains at the top
            portalPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 130),
                Width = 980,  // Slightly reduced to account for panel padding
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5),
                BackColor = Color.FromArgb(50, 50, 50)
            };

            // Position TabControl below the portal panel
            mainTabControl = new TabControl
            {
                Location = new Point(10, 390),  // Position starts right after portal panel
                Width = 980,  // Match portal panel width
                Height = 500,
                Dock = DockStyle.None,  // Remove docking to maintain specific position
                Appearance = TabAppearance.Normal,
                BackColor = Color.FromArgb(50, 50, 50)
            };

            // Initialize TabPages
            monsterTabPage = new TabPage
            {
                Text = "Monsters",
                BackColor = Color.FromArgb(50, 50, 50),
                Padding = new Padding(3),
            };

            waveTabPage = new TabPage
            {
                Text = "Waves",
                BackColor = Color.FromArgb(50, 50, 50),
                Padding = new Padding(3)
            };

            npcTabPage = new TabPage
            {
                Text = "NPCs",
                BackColor = Color.FromArgb(50, 50, 50),
                Padding = new Padding(3)
            };

            // Initialize the controls for each tab
            InitializeMonsterDataGridView();
            InitializeWaveControls();
            InitializeNpcDataGridView();

            // Add TabPages to the TabControl
            mainTabControl.TabPages.AddRange(new TabPage[] { monsterTabPage, waveTabPage, npcTabPage });

            // Add all controls to the left panel
            AddControlsToLeftPanel(leftPanel, form);

            return leftPanel;
        }

        private void AddControlsToLeftPanel(Panel leftPanel, Form1 form)
        {
            leftPanel.Controls.AddRange(new Control[]
            {
                new Label { Text = "Map Vnum:", Location = new Point(10, 10), ForeColor = Color.White },
                txtMapVNUM,
                CreateButton("Load Map", new Point(360, 10), (sender, e) => LoadMap(form)),
                new Label { Text = "Map Coordinates:", Location = new Point(10, 40), ForeColor = Color.White },
                txtMapCoordinates,
                CreateButton("...", new Point(txtMapCoordinates.Right + 10, 40), BtnSelectCoordinates_Click, 30, txtMapCoordinates.Height),
                new Label { Text = "Task Type:", Location = new Point(10, 70), ForeColor = Color.White },
                cboTaskType,
                new Label { Text = "Task Text:", Location = new Point(10, 100), ForeColor = Color.White },
                txtTaskText,
                new Label { Text = "Time for Task:", Location = new Point(360, 105), Width = 80, ForeColor = Color.White },
                timeForTask,
                portalPanel,
                CreateButton("Add Portal", new Point(10, 390), BtnAddPortal_Click),
                CreateButton("Remove Last Portal", new Point(150, 390), BtnRemovePortal_Click),
                CreateButton("Save Portals", new Point(290, 390), SaveAllValues),
                mainTabControl, // Add the TabControl here
                CreateButton("Save All", new Point(10, 900), SaveAllValues),
                CreateButton("Manage Events", new Point(360, 70), (s, e) => ManageEvents())
            });

            // Adjust the position of the mainTabControl to be below the portal buttons
            mainTabControl.Location = new Point(10, 450);
            mainTabControl.Height = 500;
        }

        private Panel CreateRightPanel()
        {
            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(50, 50, 50) };
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
                Padding = new Padding(0, 5, 0, 5),
                BackColor = Color.FromArgb(50, 50, 50)
            };

            rightPanel.Controls.AddRange(new Control[]
            {
                mapGridPanel,
                objectivePanel,
                CreateButton("Add Objective", new Point(10, 470), BtnAddObjective_Click, width: 150, height: 30),
                CreateButton("Remove Last Objective", new Point(170, 470), BtnRemoveObjective_Click, width: 150, height: 30),
                CreateButton("Save Objectives", new Point(330, 470), SaveAllValues, width: 150, height: 30),
                new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Hover Position: ", ForeColor = Color.White }
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
                Height = height,
                BackColor = Color.FromArgb(28, 28, 28),
                ForeColor = Color.White
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

        private void InitializeMonsterDataGridView()
        {
            // First create and position the DataGridView
            monsterDataGridView = new DataGridView
            {
                Location = new Point(10, 10),  // Start at top of tab
                Width = 950,
                Height = 350,  // Reduced height to make room for buttons
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false,
                BackgroundColor = Color.FromArgb(28, 28, 28),
                ForeColor = Color.White
            };
            monsterDataGridView.Columns.AddRange(new DataGridViewColumn[]
            {
        new DataGridViewTextBoxColumn { Name = "Vnum", HeaderText = "Vnum" },
        new DataGridViewTextBoxColumn { Name = "X", HeaderText = "X" },
        new DataGridViewTextBoxColumn { Name = "Y", HeaderText = "Y" },
        new DataGridViewTextBoxColumn { Name = "Wave", HeaderText = "Wave" },
        new DataGridViewCheckBoxColumn { Name = "AsTarget", HeaderText = "As Target" },
        new DataGridViewTextBoxColumn { Name = "Attributes", HeaderText = "Additional Attributes", ReadOnly = true }
            });
            monsterDataGridView.CellDoubleClick += MonsterDataGridView_CellDoubleClick;

            // Position buttons below the DataGridView
            var btnAddMonster = CreateButton("Add Monster", new Point(10, 370), BtnAddEvent_Click);
            var btnRemoveMonster = CreateButton("Remove Last Monster", new Point(150, 370), BtnRemoveEvent_Click);
            var btnSaveMonsters = CreateButton("Save Monsters", new Point(290, 370), SaveAllValues);

            monsterTabPage.Controls.AddRange(new Control[]
            {
        monsterDataGridView,
        btnAddMonster,
        btnRemoveMonster,
        btnSaveMonsters
            });
        }

        private void InitializeWaveControls()
        {
            wavePanel = new Panel
            {
                Location = new Point(10, 10),
                Width = 950,
                Height = 50,
                BackColor = Color.FromArgb(50, 50, 50)
            };

            useWavesCheckbox = new CheckBox
            {
                Text = "Use Waves",
                Location = new Point(10, 15),
                AutoSize = true,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            useWavesCheckbox.CheckedChanged += UseWavesCheckbox_CheckedChanged;

            waveCountInput = new NumericUpDown
            {
                Location = new Point(150, 15),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Enabled = false,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            waveDelayInput = new NumericUpDown
            {
                Location = new Point(350, 15),
                Width = 60,
                Minimum = 0,
                Maximum = 300,
                Value = 30,
                Enabled = false,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            wavePanel.Controls.AddRange(new Control[]
            {
        useWavesCheckbox,
        new Label { Text = "Number of Waves:", Location = new Point(220, 17), AutoSize = true, ForeColor = Color.White },
        waveCountInput,
        new Label { Text = "Wave Delay (seconds):", Location = new Point(420, 17), AutoSize = true, ForeColor = Color.White },
        waveDelayInput
            });
        }

        private void InitializeNpcDataGridView()
        {
            npcDataGridView = new DataGridView
            {
                Location = new Point(10, 10),  // Start at top of tab
                Width = 950,
                Height = 350,  // Reduced height to make room for buttons
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false,
                BackgroundColor = Color.FromArgb(28, 28, 28),
                ForeColor = Color.White
            };
            npcDataGridView.Columns.AddRange(new DataGridViewColumn[]
            {
        new DataGridViewTextBoxColumn { Name = "Vnum", HeaderText = "Vnum" },
        new DataGridViewTextBoxColumn { Name = "X", HeaderText = "X" },
        new DataGridViewTextBoxColumn { Name = "Y", HeaderText = "Y" },
        new DataGridViewTextBoxColumn { Name = "Event", HeaderText = "Event" }
            });

            // Position buttons below the DataGridView
            var btnAddNpc = CreateButton("Add NPC", new Point(10, 370), BtnAddNpc_Click);
            var btnRemoveNpc = CreateButton("Remove Last NPC", new Point(150, 370), BtnRemoveNpc_Click);
            var btnSaveNpcs = CreateButton("Save NPCs", new Point(290, 370), SaveAllValues);

            npcTabPage.Controls.AddRange(new Control[]
            {
        npcDataGridView,
        btnAddNpc,
        btnRemoveNpc,
        btnSaveNpcs
            });
        }

        // Event handlers for NPC DataGridView
        private void BtnAddNpc_Click(object sender, EventArgs e)
        {
            NpcEvents.Add(new Npc(MapName));
            npcDataGridView.Rows.Add();
        }

        private void BtnRemoveNpc_Click(object sender, EventArgs e)
        {
            if (npcDataGridView.Rows.Count > 0)
            {
                NpcEvents.RemoveAt(npcDataGridView.Rows.Count - 1);
                npcDataGridView.Rows.RemoveAt(npcDataGridView.Rows.Count - 1);
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
        private void MonsterGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_mapGridPanel != null)
            {
                if (e.RowIndex < 0 || e.RowIndex >= monsterDataGridView.Rows.Count) return;

                var row = monsterDataGridView.Rows[e.RowIndex];
                if (row.IsNewRow) return;

                if (row.Cells["X"].Value != null && row.Cells["Y"].Value != null)
                {
                    int x = Convert.ToInt32(row.Cells["X"].Value);
                    int y = Convert.ToInt32(row.Cells["Y"].Value);
                    _mapGridPanel.HighlightPosition(x, y);
                }
            }
        }

        private void MonsterGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (_mapGridPanel != null)
                _mapGridPanel.ClearHighlight();
        }

        private void MonsterGrid_MouseLeave(object sender, EventArgs e)
        {
            if (_mapGridPanel != null)
                _mapGridPanel.ClearHighlight();
        }
        public void LoadMap(Form1 form)
        {
            if (!int.TryParse(txtMapVNUM.Text, out int mapVnum))
            {
                return; // Invalid map number  
            }

            var mapData = form.loadedMaps.FirstOrDefault(m => m.Id == mapVnum);
            if (mapData == null || mapData.Grid == null || mapData.Width <= 0 || mapData.Height <= 0)
            {
                return; // Invalid map data  
            }

            try
            {
                originalGrid = new byte[mapData.Grid.Length];
                Array.Copy(mapData.Grid, originalGrid, mapData.Grid.Length);

                MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).FirstOrDefault();
                _mapGridPanel = mapGridPanel;

                if (mapGridPanel != null)
                {
                    // Clear existing markings  
                    mapGridPanel.ResetGrid();

                    // Display the new map grid  
                    DisplayMapGrid(mapData);
                    mapGridPanel.RecalculateCellSize(); // Now with proper validation  
                    mapGridPanel.UpdateMapMarkings(this, MapName, originalGrid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayMapGrid(MapDataDTO mapData)
        {
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).First();
            mapGridPanel.ResetGrid();
            mapGridPanel.SetGrid(MapName, mapData.Width, mapData.Height, mapData.Grid);
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
                var tempTakenPositions = new List<Point>(sharedTakenPositions);
                if (currentPosition.HasValue)
                {
                    tempTakenPositions.Remove(currentPosition.Value);
                }

                using (var gridSelector = new GridSelectorForm(tempTakenPositions, currentPosition))
                {
                    if (gridSelector.ShowDialog() == DialogResult.OK && gridSelector.SelectedCoordinates.HasValue)
                    {
                        if (currentPosition.HasValue)
                        {
                            sharedTakenPositions.Remove(currentPosition.Value);
                        }

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
        public void RefreshGridMarkings()
        {
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).FirstOrDefault();
            if (mapGridPanel != null)
            {
                mapGridPanel.UpdateMapMarkings(this, MapName, originalGrid);
            }
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
            LoadMap(_mainForm);
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
            mapObject.SetPosition(x, y);
            mapObject.SetObjectivesAndPortals(objectType, Portals);
            mapObject.ObjectType = objectType;

            if (objectType == "Lever")
            {
                if (taskEventManagerForm != null)
                {
                    taskEventManagerForm.UpdatePortalComboboxes(lockedPortalsList);
                }
            }
        }
    }
}
