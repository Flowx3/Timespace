using MaterialSkin.Controls;
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
        // Constants
        private const int MAX_PORTALS = 4;
        private const int MAX_OBJECTS = 4;

        // Private fields
        private readonly TimeSpaceTool _mainForm;
        private readonly MapEventGenerator _eventGenerator;
        private MapGridPanel _mapGridPanel;
        private byte[] _originalGrid;
        private Point? _currentPosition;
        private bool _isDisposed;

        // UI Controls
        private ModernTabControl _mainTabControl;
        private TabPage _monsterTabPage;
        private TabPage _npcTabPage;
        private FlowLayoutPanel _portalPanel;
        private FlowLayoutPanel _objectivePanel;
        private ModernDataGridView _monsterDataGridView;
        private ModernDataGridView _npcDataGridView;
        private ModernTextBox _txtMapVNum;
        private ModernTextBox _txtMapCoordinates;
        private ModernTextBox _txtTaskText;
        private CustomMaterialStyleComboBox _cboTaskType;
        private ModernNumericUpDown _timeForTask;
        private ModernNumericUpDown _waveCountInput;
        private ModernNumericUpDown _waveDelayInput;
        private ModernCheckBox _useWavesCheckbox;
        private Label _lblTimeForTask;
        private Label _lblWaveDelay;
        private Label _lblWaveCount;
        private ModernButton _btnTaskFailEvents;
        private ModernButton _btnTaskFinishEvents;
        private DataGridViewTextBoxColumn _txtBoxColumnWave;

        // Collections
        private static readonly object _positionsLock = new object();
        private static readonly List<Point> _sharedTakenPositions = new List<Point>();
        private readonly List<string> _allPortalsList = new List<string>();
        private readonly List<string> _lockedPortalsList = new List<string>();
        private readonly List<CustomTabPage> _mapTabs;
        private List<string> taskFinishEvents = new List<string>();
        private List<string> taskFailEvents = new List<string>();

        // Properties
        public string MapName { get; private set; }
        public Func<List<string>> GetMapNames { get; }
        public Dictionary<string, Panel> MapPortalPanels { get; set; }
        public List<Portal> Portals { get; private set; } = new List<Portal>();
        public List<MapObject> Objects { get; private set; } = new List<MapObject>();
        public List<Monster> MonsterEvents { get; private set; } = new List<Monster>();
        public List<Npc> NpcEvents { get; private set; } = new List<Npc>();
        public Dictionary<string, List<string>> EventManagerScripts { get; } = new Dictionary<string, List<string>>();
        public DataGridView MonsterDataGridView => _monsterDataGridView;


        public CustomTabPage(string mapName, TimeSpaceTool form, Func<List<string>> getMapNames)
        {
            MapName = mapName;
            _mainForm = form;
            GetMapNames = getMapNames;
            Text = mapName;
            _mapTabs = TimeSpaceTool.mapTabs;

            InitializeComponents(form);
            _eventGenerator = new MapEventGenerator(_waveDelayInput, EventManagerScripts);
            MapPortalPanels = new Dictionary<string, Panel>();
        }

        private void InitializeComponents(TimeSpaceTool form)
        {
            var containerPanel = CreateContainerPanel();
            var leftPanel = CreateLeftPanel(form);
            var rightPanel = CreateRightPanel();

            containerPanel.Controls.Add(rightPanel);
            containerPanel.Controls.Add(leftPanel);
            Controls.Add(containerPanel);
        }
        private Panel CreateContainerPanel() =>
            new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(50, 50, 50)
            };
        private Panel CreateLeftPanel(TimeSpaceTool form)
        {
            var leftPanel = new Panel { Width = 1000, Dock = DockStyle.Left, BackColor = Color.FromArgb(50, 50, 50) };

            _txtMapVNum = new ModernTextBox { Name = "txtMapVNUM", Location = new Point(150, 10), Width = 200, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White };
            _txtMapCoordinates = new ModernTextBox { Location = new Point(150, 40), Width = 200, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White };
            _cboTaskType = new CustomMaterialStyleComboBox
            {
                Location = new Point(150, 70),
                Items = { "None", "KillAllMonsters", "Survive" },
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                SelectedValue = "None",
                Size = new Size(200, 20)
            };
            _cboTaskType.SelectedValueChanged += _cboTaskType_SelectedValueChanged;

            _txtTaskText = new ModernTextBox { Location = new Point(150, 100), Width = 200, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White };
            _lblTimeForTask = new Label { Text = "Time for Task (s):", Location = new Point(600, 80), ForeColor = Color.White, Visible = false };
            _timeForTask = new ModernNumericUpDown
            {
                Size = new Size(75, 25),
                Minimum = 0,
                Location = new Point(700, 75),
                Maximum = 600,
                Increment = 10,
                Visible = false
            };

            _portalPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 130),
                Width = 980,  
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5),
                BackColor = Color.FromArgb(50, 50, 50)
            };

            _mainTabControl = new ModernTabControl
            {
                Location = new Point(10, 390),  
                Width = 980,  
                Height = 350,
                Dock = DockStyle.None,  
                Appearance = TabAppearance.Normal,
                BackColor = Color.FromArgb(50, 50, 50)
            };

            _monsterTabPage = new TabPage
            {
                Text = "Monsters",
                BackColor = Color.FromArgb(50, 50, 50),
                Padding = new Padding(3),
            };

            _npcTabPage = new TabPage
            {
                Text = "NPCs",
                BackColor = Color.FromArgb(50, 50, 50),
                Padding = new Padding(3)
            };

            _btnTaskFailEvents = new ModernButton
            {
                Text = "Task Fail Events",
                Location = new Point(470, 70),
                AutoSize = false, // Ensure AutoSize is set to false
                Size = new Size(100, 30), // Set the size directly
                Visible = false
            };
            _btnTaskFailEvents.Click += (s, e) => OpenTaskEventManager("TaskFail");

            _btnTaskFinishEvents = new ModernButton
            {
                Text = "Task Finish Events",
                Location = new Point(360, 70),
                AutoSize = false, // Ensure AutoSize is set to false
                Size = new Size(100, 30), // Set the size directly
                Visible = false
            };
            _btnTaskFinishEvents.Click += (s, e) => OpenTaskEventManager("TaskFinish");

            InitializeMonsterDataGridView();
            InitializeNpcDataGridView();

            _mainTabControl.TabPages.AddRange(new TabPage[] { _monsterTabPage, _npcTabPage });

            AddControlsToLeftPanel(leftPanel, form);

            return leftPanel;
        }

        private void _cboTaskType_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (_cboTaskType.SelectedItem == "None")
            {
                _timeForTask.Visible = false;
                _lblTimeForTask.Visible = false;
                _btnTaskFinishEvents.Visible = false;
                _btnTaskFailEvents.Visible = false;
            }
            else
            {
                _timeForTask.Visible = true;
                _lblTimeForTask.Visible = true;
                _btnTaskFinishEvents.Visible = true;
                _btnTaskFailEvents.Visible = true;  
            }
        }

        private void AddControlsToLeftPanel(Panel leftPanel, TimeSpaceTool form)
        {
            leftPanel.Controls.AddRange(new Control[]
            {
                new Label { Text = "Map Vnum:", Location = new Point(10, 10), ForeColor = Color.White },
                _txtMapVNum,
                CreateButton("Load Map", new Point(360, 10), (sender, e) => LoadMap(form), 100, 25),
                new Label { Text = "Map Coordinates:", Location = new Point(10, 40), ForeColor = Color.White },
                _txtMapCoordinates,
                CreateButton("...", new Point(_txtMapCoordinates.Right + 10, 40), BtnSelectCoordinates_Click, 30, _txtMapCoordinates.Height),
                new Label { Text = "Task Type:", Location = new Point(10, 70), ForeColor = Color.White },
                _cboTaskType,
                new Label { Text = "Task Text:", Location = new Point(10, 100), ForeColor = Color.White },
                _txtTaskText,
                _lblTimeForTask,
                _timeForTask,
                _portalPanel,
                CreateButton("Add Portal", new Point(10, 390), BtnAddPortal_Click),
                CreateButton("Remove Last Portal", new Point(150, 390), BtnRemovePortal_Click),
                CreateButton("Save Portals", new Point(290, 390), SaveAllValues),
                _mainTabControl, 
                CreateButton("Save All", new Point(10, 900), SaveAllValues),
                _btnTaskFinishEvents,
                _btnTaskFailEvents
            });

            _mainTabControl.Location = new Point(10, 450);
            _mainTabControl.Height = 500;
        }

        private Panel CreateRightPanel()
        {
            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(50, 50, 50) };
            var mapGridPanel = new MapGridPanel();
            mapGridPanel.CellClicked += MapGridPanel_CellClicked;

            _objectivePanel = new FlowLayoutPanel
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
                _objectivePanel,
                CreateButton("Add Objective", new Point(10, 470), BtnAddObjective_Click, width: 150, height: 30),
                CreateButton("Remove Last Objective", new Point(170, 470), BtnRemoveObjective_Click, width: 150, height: 30),
                CreateButton("Save Objectives", new Point(330, 470), SaveAllValues, width: 150, height: 30),
                new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Hover Position: ", ForeColor = Color.White }
            });
            return rightPanel;
        }

        private ModernButton CreateButton(string text, Point location, EventHandler clickHandler, int width = 100, int height = 30)
        {
            var button = new ModernButton
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

        private void OpenTaskEventManager(string eventType)
        {
            var eventList = eventType == "TaskFinish" ? taskFinishEvents : taskFailEvents;
            using (var taskEventManager = new EventManagerForm(MapName, GetMapNames,_lockedPortalsList, eventList))
            {
                if (taskEventManager.ShowDialog() == DialogResult.OK)
                {
                    if (eventType == "TaskFinish")
                        taskFinishEvents = taskEventManager.Events;
                    else
                        taskFailEvents = taskEventManager.Events;
                }
                else
                {
                    if (eventType == "TaskFinish")
                        taskFinishEvents = taskEventManager.Events;
                    else
                        taskFailEvents = taskEventManager.Events;
                    return;
                }
            }
        }

        public List<string> GetTaskFinishEvents() => taskFinishEvents;
        public List<string> GetTaskFailEvents() => taskFailEvents;

        private void InitializeMonsterDataGridView()
        {
            _monsterDataGridView = new ModernDataGridView
            {
                Location = new Point(10, 10),
                Width = 950,
                Height = 250,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false
            };

            _monsterDataGridView.Columns.AddRange(new DataGridViewColumn[]
            {
            new DataGridViewTextBoxColumn { Name = "Vnum", HeaderText = "Vnum" },
            new DataGridViewTextBoxColumn { Name = "X", HeaderText = "X" },
            new DataGridViewTextBoxColumn { Name = "Y", HeaderText = "Y" },
            _txtBoxColumnWave = new DataGridViewTextBoxColumn { Name = "Wave", HeaderText = "Wave", Visible = false },
            new DataGridViewCheckBoxColumn
            {
                Name = "AsTarget",
                HeaderText = "As Target",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            },
            new DataGridViewTextBoxColumn { Name = "Attributes", HeaderText = "Additional Attributes", ReadOnly = true }
            });
            _monsterDataGridView.CellDoubleClick += MonsterDataGridView_CellDoubleClick;

            var btnAddMonster = CreateButton("Add Monster", new Point(10, 270), BtnAddEvent_Click);
            var btnRemoveMonster = CreateButton("Remove Last Monster", new Point(150, 270), BtnRemoveEvent_Click);
            var btnSaveMonsters = CreateButton("Save Monsters", new Point(290, 270), SaveAllValues);
            _useWavesCheckbox = new ModernCheckBox
            {
                Text = "Use Waves",
                Location = new Point(425, 265),
                Size = new Size(90, 35)
            };
            _useWavesCheckbox.CheckedChanged += UseWavesCheckbox_CheckedChanged;

            _waveCountInput = new ModernNumericUpDown
            {
                Location = new Point(660, 270),
                Size = new Size(75, 25),
                Increment = 1,
                Visible = false
            };
            _waveDelayInput = new ModernNumericUpDown
            {
                Location = new Point(850, 270),
                Size = new Size(75, 25),
                Visible = false
            };
            _lblWaveDelay = new Label 
            {
                Text = "Wave Delay (s)",
                Location = new Point(750, 275),
                ForeColor = Color.White,
                Width = 200,
                Visible = false
            };
            _lblWaveCount = new Label 
            { 
                Text = "Amount of Waves",
                Location = new Point(525, 275),
                ForeColor = Color.White,
                Width = 200,
                Visible = false
            };
            _monsterTabPage.Controls.AddRange(new Control[]
            {
                _monsterDataGridView,
                btnAddMonster,
                btnRemoveMonster,
                btnSaveMonsters,
                _useWavesCheckbox,
                _waveDelayInput,
                _waveCountInput,
                _lblWaveCount,
                _lblWaveDelay
        });
        }
        private void InitializeNpcDataGridView()
        {
            _npcDataGridView = new ModernDataGridView
            {
                Location = new Point(10, 10),
                Width = 950,
                Height = 250,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false
            };

            _npcDataGridView.Columns.AddRange(new DataGridViewColumn[]
            {
        new DataGridViewTextBoxColumn { Name = "Vnum", HeaderText = "Vnum" },
        new DataGridViewTextBoxColumn { Name = "X", HeaderText = "X" },
        new DataGridViewTextBoxColumn { Name = "Y", HeaderText = "Y" },
        new DataGridViewTextBoxColumn { Name = "Event", HeaderText = "Event" }
            });

            var btnAddNpc = CreateButton("Add NPC", new Point(10, 270), BtnAddNpc_Click);
            var btnRemoveNpc = CreateButton("Remove Last NPC", new Point(150, 270), BtnRemoveNpc_Click);
            var btnSaveNpcs = CreateButton("Save NPCs", new Point(290, 270), SaveAllValues);

            _npcTabPage.Controls.AddRange(new Control[]
            {
                _npcDataGridView,
                btnAddNpc,
                btnRemoveNpc,
                btnSaveNpcs
            });
        }

        private void BtnAddNpc_Click(object sender, EventArgs e)
        {
            object[] rowData = new object[]
            {
        "",     // Vnum
        "",     // X
        "",     // Y
        ""      // Event
            };
            NpcEvents.Add(new Npc(MapName));
            _npcDataGridView.Rows.Add(rowData);
        }

        private void BtnRemoveNpc_Click(object sender, EventArgs e)
        {
            if (_npcDataGridView.Rows.Count > 0)
            {
                NpcEvents.RemoveAt(_npcDataGridView.Rows.Count - 1);
                _npcDataGridView.Rows.RemoveAt(_npcDataGridView.Rows.Count - 1);
            }
        }

        private void UseWavesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _waveCountInput.Visible = _useWavesCheckbox.Checked;
            _waveDelayInput.Visible = _useWavesCheckbox.Checked;
            _lblWaveCount.Visible = _useWavesCheckbox.Checked;
            _lblWaveDelay.Visible = _useWavesCheckbox.Checked;
            _txtBoxColumnWave.Visible = _useWavesCheckbox.Checked;
        }
        private void MonsterGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_mapGridPanel != null)
            {
                if (e.RowIndex < 0 || e.RowIndex >= _monsterDataGridView.Rows.Count) return;

                var row = _monsterDataGridView.Rows[e.RowIndex];
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
        public void LoadMap(TimeSpaceTool form)
        {
            if (!int.TryParse(_txtMapVNum.Text, out int mapVnum))
            {
                MessageBox.Show("Invalid map Id", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Invalid map number  
            }

            var mapData = form.loadedMaps.FirstOrDefault(m => m.Id == mapVnum);
            if (mapData == null || mapData.Grid == null || mapData.Width <= 0 || mapData.Height <= 0)
            {
                MessageBox.Show("Invalid map data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Invalid map data  
            }

            try
            {
                _originalGrid = new byte[mapData.Grid.Length];
                Array.Copy(mapData.Grid, _originalGrid, mapData.Grid.Length);

                MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).FirstOrDefault();
                _mapGridPanel = mapGridPanel;

                if (mapGridPanel != null)
                {
                    mapGridPanel.ResetGrid();

                    DisplayMapGrid(mapData);
                    mapGridPanel.RecalculateCellSize(); 
                    mapGridPanel.UpdateMapMarkings(this, MapName, _originalGrid);
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
            if (e.RowIndex >= 0 && _monsterDataGridView.Columns[e.ColumnIndex].Name == "Attributes")
            {
                BtnAddAttribute_Click(sender, e);
            }
        }

        private void BtnSelectCoordinates_Click(object sender, EventArgs e)
        {
            lock (_positionsLock)
            {
                var tempTakenPositions = new List<Point>(_sharedTakenPositions);
                if (_currentPosition.HasValue)
                {
                    tempTakenPositions.Remove(_currentPosition.Value);
                }

                using (var gridSelector = new GridSelectorForm(tempTakenPositions, _currentPosition))
                {
                    if (gridSelector.ShowDialog() == DialogResult.OK && gridSelector.SelectedCoordinates.HasValue)
                    {
                        if (_currentPosition.HasValue)
                        {
                            _sharedTakenPositions.Remove(_currentPosition.Value);
                        }

                        Point selectedPos = gridSelector.SelectedCoordinates.Value;
                        _txtMapCoordinates.Text = $"{selectedPos.X}, {selectedPos.Y}";
                        Text = $"map_{selectedPos.X}_{selectedPos.Y}";
                        MapName = $"map_{selectedPos.X}_{selectedPos.Y}";
                        _currentPosition = selectedPos;
                        _sharedTakenPositions.Add(selectedPos);
                    }
                }
            }
        }

        public Point? GetCoordinates()
        {
            return _currentPosition;
        }

        public void SetCoordinates(Point point)
        {
            lock (_positionsLock)
            {
                if (_currentPosition.HasValue)
                {
                    _sharedTakenPositions.Remove(_currentPosition.Value);
                }

                _txtMapCoordinates.Text = $"{point.X}, {point.Y}";
                _currentPosition = point;
                _sharedTakenPositions.Add(point);
            }
        }

        public void ClearCoordinates()
        {
            lock (_positionsLock)
            {
                if (_currentPosition.HasValue)
                {
                    _sharedTakenPositions.Remove(_currentPosition.Value);
                    _currentPosition = null;
                }
                _txtMapCoordinates.Text = string.Empty;
            }
        }

        // Optional: Method to get all taken positions (might be useful for debugging)
        public static List<Point> GetAllTakenPositions()
        {
            lock (_positionsLock)
            {
                return new List<Point>(_sharedTakenPositions);
            }
        }

        // Optional: Method to clear all positions (might be useful when starting fresh)
        public static void ClearAllPositions()
        {
            lock (_positionsLock)
            {
                _sharedTakenPositions.Clear();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    CleanupCoordinates();
                }
                _isDisposed = true;
            }
            base.Dispose(disposing);
        }

        public void CleanupCoordinates()
        {
            lock (_positionsLock)
            {
                if (_currentPosition.HasValue)
                {
                    _sharedTakenPositions.Remove(_currentPosition.Value);
                    _currentPosition = null;
                    _txtMapCoordinates?.Clear();
                }
            }
        }
        private void BtnAddAttribute_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> currentAttributes = new Dictionary<string, object>();
            if (_monsterDataGridView.CurrentRow != null &&
                MonsterEvents != null &&
                MonsterEvents.Count > _monsterDataGridView.CurrentRow.Index)
            {
                foreach (var attr in MonsterEvents[_monsterDataGridView.CurrentRow.Index].Attributes)
                {
                    if (IsEventAttribute(attr.Key))
                    {
                        var events = ParseEventString(attr.Value);
                        currentAttributes[attr.Key] = events;
                    }
                    else
                    {
                        currentAttributes[attr.Key] = attr.Value;
                    }
                }
            }

            var attributeForm = new MonsterAttributeForm(currentAttributes);
            if (attributeForm.ShowDialog() == DialogResult.OK)
            {
                if (_monsterDataGridView.CurrentRow != null &&
                    MonsterEvents != null &&
                    MonsterEvents.Count > _monsterDataGridView.CurrentRow.Index)
                {
                    var monster = MonsterEvents[_monsterDataGridView.CurrentRow.Index];
                    monster.Attributes.Clear();
                    foreach (var attr in attributeForm.SelectedAttributes)
                    {
                        if (attr.Value is List<string> events)
                        {
                            monster.Attributes[attr.Key] = ConvertEventsToString(events);
                        }
                        else
                        {
                            monster.Attributes[attr.Key] = attr.Value.ToString();
                        }
                    }
                    UpdateAttributeDisplay(_monsterDataGridView.CurrentRow.Index);
                }
                else
                {
                    MonsterEvents = MonsterEvents ?? new List<Monster>();
                    var monster = new Monster(MapName);
                    foreach (var attr in attributeForm.SelectedAttributes)
                    {
                        if (attr.Value is List<string> events)
                        {
                            monster.Attributes[attr.Key] = ConvertEventsToString(events);
                        }
                        else
                        {
                            monster.Attributes[attr.Key] = attr.Value.ToString();
                        }
                    }
                    MonsterEvents.Add(monster);
                    _monsterDataGridView.Refresh();
                    UpdateAttributeDisplay(MonsterEvents.Count - 1);
                }
                _monsterDataGridView.Refresh();
            }
        }

        private bool IsEventAttribute(string attributeName)
        {
            return attributeName == "OnThreeFourthsHp" ||
                   attributeName == "OnHalfHp" ||
                   attributeName == "OnQuarterHp" ||
                   attributeName == "OnDeath";
        }

        private List<string> ParseEventString(string eventString)
        {
            var events = new List<string>();
            if (string.IsNullOrEmpty(eventString)) return events;

            eventString = eventString.Trim('{', '}', ' ');

            var currentEvent = "";
            var bracketCount = 0;

            foreach (var c in eventString)
            {
                if (c == '{') bracketCount++;
                else if (c == '}') bracketCount--;

                if (c == ',' && bracketCount == 0)
                {
                    if (!string.IsNullOrWhiteSpace(currentEvent))
                    {
                        events.Add(currentEvent.Trim());
                        currentEvent = "";
                    }
                }
                else
                {
                    currentEvent += c;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentEvent))
            {
                events.Add(currentEvent.Trim());
            }

            return events;
        }
        private string ConvertEventsToString(List<string> events)
        {
            if (events == null || events.Count == 0) return "{}";
            return "{" + string.Join(",", events) + "}";
        }
        private void UpdateAttributeDisplay(int rowIndex)
        {
            var monster = MonsterEvents[rowIndex];
            var attributeText = string.Join(", ", monster.Attributes.Select(a => $"{a.Key}={a.Value}"));
            _monsterDataGridView.Rows[rowIndex].Cells["Attributes"].Value = attributeText;
        }

        public string GenerateMapScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"local {MapName} = Map.Create().WithMapId({_txtMapVNum.Text}).SetMapCoordinates({_txtMapCoordinates.Text}).WithTask(");
            sb.AppendLine($"    TimeSpaceTask.Create(TimeSpaceTaskType.{_cboTaskType.SelectedItem}, {_timeForTask.Value}).WithTaskText(\"{_txtTaskText?.Text}\")" +
                          $"\n)");
            return sb.ToString();
        }
        public void AddPortalToMap(string mapName, Portal portal)
        {
            var test = _mapTabs.FirstOrDefault(x => x.MapName == mapName);
            if (test != null)
            {
                test._portalPanel.Controls.Add(portal.CreatePortal());
            }
            else
            {
                MessageBox.Show($"Map '{mapName}' does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnAddPortal_Click(object sender, EventArgs e)
        {
            if (Portals.Count >= MAX_PORTALS)
            {
                MessageBox.Show("You don't need more than 4 Portals.");
                return;
            }

            var portal = new Portal(
                this.MapName,
                "DefaultTo",
                "Type1",
                "North",
                null,
                null,
                null,
                null,
                GetMapNames,
                this  
            );

            Portals.Add(portal);
            this._portalPanel.Controls.Add(portal.CreatePortal());
            SaveAndRefreshPortals(sender, e, false);
        }
        private void BtnRemovePortal_Click(object sender, EventArgs e)
        {
            if (Portals.Count > 0)
            {
                var lastPortal = Portals.Last();
                Portals.Remove(lastPortal);
                _portalPanel.Controls.Remove(lastPortal.Panel);
                _portalPanel.Refresh();
            }
            SaveAndRefreshPortals(sender,e, false);
        }
        public void RefreshGridMarkings()
        {
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).FirstOrDefault();
            if (mapGridPanel != null)
            {
                mapGridPanel.UpdateMapMarkings(this, MapName, _originalGrid);
            }
        }
        public void SaveAndRefreshPortals(object sender, EventArgs e, bool generateScripts)
        {
            if (generateScripts)
            {
                var localPortalScript = new StringBuilder();
                var addPortalScript = new StringBuilder();
                _allPortalsList.Clear();
                _lockedPortalsList.Clear();

                foreach (var mapTab in _mapTabs)
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
                        _allPortalsList.Add(portal.GeneratePortalIdentifier());

                        if (portal.PortalType == "Locked")
                        {
                            _lockedPortalsList.Add(portal.GeneratePortalIdentifier());
                        }
                    }
                }

                File.WriteAllText("localPortals.lua", localPortalScript.ToString());
                File.WriteAllText("addPortals.lua", addPortalScript.ToString());
            }
            else
            {
                _lockedPortalsList.Clear();
                _lockedPortalsList.Add("");

                foreach (var tab in _mapTabs)
                {
                    foreach (var portal in tab.Portals)
                    {
                        if (portal.cboMapFrom?.SelectedItem != null &&
                            portal.cboMapTo?.SelectedItem != null &&
                            portal.PortalType == "Locked")
                        {
                            _lockedPortalsList.Add(portal.GeneratePortalIdentifier());
                        }
                    }
                }
            }

            foreach (var mapObject in Objects)
            {
                mapObject.UpdatePortalComboboxes(_lockedPortalsList);
            }

            foreach (var portal in Portals)
            {
                portal.RefreshMapComboboxes();
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
            MonsterEvents.Add(monster);
            object[] rowData = new object[]
            {
            "",     // Vnum
            "",    // X
            "",    // Y
            "",    // Wave
            false,  // AsTarget
            ""      // Attributes
            };

            _monsterDataGridView.Rows.Add(rowData);
        }
        private void BtnRemoveEvent_Click(object sender, EventArgs e)
        {
            if (_monsterDataGridView.Rows.Count > 0)
            {
                _monsterDataGridView.Rows.RemoveAt(_monsterDataGridView.Rows.Count - 1);
            }
        }
        public void BtnSaveMonsterAndObjective_Click(object sender, EventArgs e)
        {
            if (_mapTabs == null)
                return;
            if (ValidateAndUpdateMonsterPositions())
            {
                _eventGenerator.GenerateEvents(_mapTabs);
            }
        }
        private bool ValidateAndUpdateMonsterPositions()
        {
            bool hasErrors = false;
            List<string> errorMessages = new List<string>();
            List<(DataGridViewRow Row, int Vnum, int X, int Y)> invalidPositions = new List<(DataGridViewRow, int, int, int)>();
            MapGridPanel mapGridPanel = (MapGridPanel)this.Controls.Find("mapGridPanel", true).First();
            foreach (DataGridViewRow row in _monsterDataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                try
                {
                    var vnum = row.Cells["vnum"].Value?.ToString() ?? "";

                    int x = -1;
                    int y = -1;

                    if (string.IsNullOrWhiteSpace(vnum))
                        continue;

                    bool needsPosition = !int.TryParse(row.Cells["X"].Value?.ToString(), out x) ||
                                       !int.TryParse(row.Cells["Y"].Value?.ToString(), out y);

                    if (needsPosition)
                    {
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
            if (Objects.Count >= MAX_OBJECTS)
            {
                MessageBox.Show("You dont need more than 4 Objects.");
                return;
            }
            var Object = new MapObject(MapName, "Object", 0, 0, _allPortalsList);
            Objects.Add(Object);
            this._objectivePanel.Controls.Add(Object.CreateObject());
        }
        private void BtnRemoveObjective_Click(object sender, EventArgs e)
        {
            if (Objects.Count > 0)
            {
                var lastObject = Objects.Last();
                Objects.Remove(lastObject);
                _objectivePanel.Controls.Remove(lastObject.Panel);
                _objectivePanel.Refresh();
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
            this._txtMapVNum.Text = vnum.ToString();
        }
        public void SetMapCoordinates(string coords)
        {
            this._txtMapCoordinates.Text = coords;
        }
        public void SetTaskType(string taskType) => _cboTaskType.SelectedItem = taskType;
        public void AddMonster(int vnum, int x, int y ,bool asTarget, Dictionary<string, string> attributes, int Wave = -1)
        {
            var monster = new Monster(MapName)
            {
                Vnum = vnum,
                X = x,
                Y = y,
                Wave = Wave,
                AsTarget = asTarget,
                Attributes = attributes
            };

            int currentWave = 1;
            if (_useWavesCheckbox?.Checked == true && _waveCountInput?.Enabled == true)
            {
                currentWave = (int)_waveCountInput.Value;
            }

            try
            {
                _monsterDataGridView.SuspendLayout();

                object[] rowValues = new object[]
                {
            monster.Vnum.ToString(),
            monster.X.ToString(),
            monster.Y.ToString(),
            monster.Wave.ToString(),
            monster.AsTarget,
            string.Join(", ", monster.Attributes.Select(a => $"{a.Key}={a.Value}"))
                };

                int rowIndex = _monsterDataGridView.Rows.Add(rowValues);
                var row = _monsterDataGridView.Rows[rowIndex];

                // Immediately apply styling
                row.DefaultCellStyle = _monsterDataGridView.DefaultCellStyle.Clone();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style = _monsterDataGridView.DefaultCellStyle.Clone();
                }

                MonsterEvents ??= new List<Monster>();
                MonsterEvents.Add(monster);

                _monsterDataGridView.ResumeLayout();
                _monsterDataGridView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding monster: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void AddMapObject(string objectType, int x, int y, List<string> linkedPortals = null)
        {
            var mapObject = new MapObject(MapName, objectType, x, y, linkedPortals ?? new List<string>());

            Objects.Add(mapObject);

            this._objectivePanel.Controls.Add(mapObject.CreateObject());
            mapObject.SetPosition(x, y);
            mapObject.SetObjectivesAndPortals(objectType, Portals);
            mapObject.ObjectType = objectType;
        }
    }
}
