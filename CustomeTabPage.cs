using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSpace
{
    public class MapGridPanel : Panel
    {
        private byte[] _grid;
        private int _width;
        private int _height;
        private int _cellSize;

        private const int FixedPanelWidth = 790;
        private const int FixedPanelHeight = 570;

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
        private Task _gridCreationTask;
        private readonly TextBox txtMapVNUM;
        private readonly TextBox txtMapCoordinates;
        private readonly TextBox txtTaskText;
        private readonly ComboBox cboTaskType;
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
        public CustomeTabPage(string MapName,Form1 form)
        {
            Text = MapName;
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
            var portalPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 130),
                Width = 800,
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };
            var btnAddPortal = new Button { Text = "Add Portal", Location = new Point(10, 390) };
            //btnAddPortal.Click += BtnAddPortal_Click;
            var btnRemovePortal = new Button { Text = "Remove Last Portal", Location = new Point(150, 390) };
            //btnRemovePortal.Click += BtnRemovePortal_Click;
            var btnSavePortal = new Button { Text = "Save Portals", Location = new Point(290, 390) };
            //btnSavePortal.Click += BtnSavePortal_Click;

            // Initialize event panel  
            var eventPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 420),
                Width = 800,
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };
            var btnAddEvent = new Button { Text = "Add Monster Event", Location = new Point(10, 680) };
            //btnAddEvent.Click += BtnAddEvent_Click;
            var btnRemoveEvent = new Button { Text = "Remove Last Monster Event", Location = new Point(150, 680) };
            //btnRemoveEvent.Click += BtnRemoveEvent_Click;
            var btnSaveMonster = new Button { Text = "Save Monsters", Location = new Point(290, 680) };
            //btnSaveMonster.Click += BtnSaveMonster_Click;

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
            var objectivePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Width = 980,
                Height = 250,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            var btnAddObjective = new Button { Text = "Add Objective", Dock = DockStyle.Bottom, Height = 30 };
            //btnAddObjective.Click += BtnAddObjective_Click;
            var btnRemoveObjective = new Button { Text = "Remove Last Objective", Dock = DockStyle.Bottom, Height = 30 };
            //btnRemoveObjective.Click += BtnRemoveObjective_Click;
            var btnSaveObjective = new Button { Text = "Save Objectives", Dock = DockStyle.Bottom, Height = 30 };
            //btnSaveObjective.Click += BtnSaveObjective_Click;

            rightPanel.Controls.Add(mapGridPanel);
            rightPanel.Controls.Add(objectivePanel);
            rightPanel.Controls.Add(btnAddObjective);
            rightPanel.Controls.Add(btnRemoveObjective);
            rightPanel.Controls.Add(btnSaveObjective);
            rightPanel.Controls.Add(lblHoverPosition);
            containerPanel.Controls.Add(rightPanel);
            containerPanel.Controls.Add(leftPanel);
            this.Controls.Add(containerPanel);
        }
        public string GenerateMapScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"local {txtMapVNUM.Text} = Map.Create().WithMapId({txtMapCoordinates.Text}).SetMapCoordinates({txtMapCoordinates.Text}).WithTask(");
            sb.AppendLine($"    TimeSpaceTask.Create(TimeSpaceTaskType.{cboTaskType.SelectedItem}).WithTaskText(\"{txtTaskText?.Text}\"))");
            return sb.ToString();
        }
    }
}
