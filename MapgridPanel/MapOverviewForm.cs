using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TimeSpace;
using TimeSpace.MapgridPanel;

public class MapOverviewForm : Form
{
    private const int PADDING = 30;
    private const int GRID_SIZE = 11;
    private const int HEADER_SIZE = 40;
    private const int MAP_SIZE = 300;
    private Dictionary<CustomTabPage, MapGridPanel> gridPanels;
    private DoubleBufferedPanel containerPanel;
    private Point lastMousePosition;
    private bool isDragging;
    private TabControl _mapTabControl;
    private List<CustomTabPage> _tabPages;
    private Dictionary<CustomTabPage, Point> originalPositions;
    private bool isConnectingPortals;
    private (int x, int y) _contextMenuPosition;
    private Portal selectedPortal;
    private MapGridPanel selectedPanel;
    private Dictionary<(CustomTabPage, Point), (CustomTabPage, Point)> portalConnections;
    private event EventHandler<PortalClickEventArgs> PortalClicked;
    private List<PortalConnection> connections = new List<PortalConnection>();
    private readonly System.Diagnostics.Stopwatch _drawStopwatch = new System.Diagnostics.Stopwatch();
    private bool _isDrawing = false;
    private bool isDrawingConnection;
    private Point currentMousePosition;
    private Portal sourcePortal;
    private MapGridPanel sourcePanel;
    private Point sourcePortalPoint;
    private CustomTabPage sourceTabPage;
    private MenuStrip menuStrip;
    private TimeSpaceTool _mainForm;

    private class PortalConnection
    {
        public MapGridPanel StartPanel { get; set; }
        public MapGridPanel EndPanel { get; set; }
        public Point StartGrid { get; set; }
        public Point EndGrid { get; set; }
    }

    #region Constructor
    public MapOverviewForm(TabControl mapTabControl, TimeSpaceTool mainForm)
    {
        _mapTabControl = mapTabControl ?? throw new ArgumentNullException(nameof(mapTabControl));
        _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        _tabPages = new List<CustomTabPage>();
        gridPanels = new Dictionary<CustomTabPage, MapGridPanel>();
        originalPositions = new Dictionary<CustomTabPage, Point>();
        portalConnections = new Dictionary<(CustomTabPage, Point), (CustomTabPage, Point)>();

        InitializeComponent();
        InitializeGridPanels();
        InitializePortalConnections();

        if (!gridPanels.Any())
        {
            ShowNoMapsMessage();
        }
        else
        {
            PositionPanelsBasedOnCoordinates();
            try
            {
                UpdateConnections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating connections: {ex}");
            }
        }
    }
    #endregion

    #region Initialization
    private void InitializeComponent()
    {
        Text = "Map Overview";
        Size = Screen.PrimaryScreen.WorkingArea.Size;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(50, 50, 50);

        // Create MenuStrip
        menuStrip = new MenuStrip
        {
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            Renderer = new CustomToolStripRenderer()
        };
        var createMapMenuItem = new ToolStripMenuItem("Create New Map", null, CreateMapMenuItem_Click)
        {
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White
        };
        menuStrip.Items.Add(createMapMenuItem);
        menuStrip.Dock = DockStyle.Top;

        // Add the MenuStrip to the form
        Controls.Add(menuStrip);

        containerPanel = new DoubleBufferedPanel();
        containerPanel.Paint += ContainerPanel_Paint;
        containerPanel.AutoScroll = true;
        containerPanel.Dock = DockStyle.Fill;
        containerPanel.BackColor = Color.FromArgb(40, 40, 40);

        containerPanel.MouseDown += ContainerPanel_MouseDown;
        containerPanel.MouseMove += ContainerPanel_MouseMove;
        containerPanel.MouseUp += ContainerPanel_MouseUp;

        Controls.Add(containerPanel);

        // Add row headers (Y-axis)
        for (int i = 0; i <= GRID_SIZE; i++)
        {
            Label rowLabel = new Label
            {
                Text = $"[{i}]",
                Location = new Point(5, HEADER_SIZE + (i * (MAP_SIZE + PADDING))),
                Size = new Size(MAP_SIZE, MAP_SIZE),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            containerPanel.Controls.Add(rowLabel);
        }

        // Add column headers (X-axis)
        for (int j = 0; j <= GRID_SIZE; j++)
        {
            Label colLabel = new Label
            {
                Text = $"[{j}]",
                Location = new Point(HEADER_SIZE + (j * (MAP_SIZE + PADDING)), 5),
                Size = new Size(MAP_SIZE, MAP_SIZE),
                TextAlign = ContentAlignment.BottomCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            containerPanel.Controls.Add(colLabel);
        }
    }
    private void InitializeGridPanels()
    {
        containerPanel.Controls.Clear();
        gridPanels.Clear();
        _tabPages.Clear();

        foreach (CustomTabPage tabPage in _mapTabControl.Controls.OfType<CustomTabPage>())
        {
            if (tabPage._mapGridPanel == null)
            {
                System.Diagnostics.Debug.WriteLine($"Skipping tab {tabPage.Text} - no MapGridPanel");
                continue;
            }

            _tabPages.Add(tabPage);

            var mapGridPanel = new MapGridPanel
            {
                Name = $"MapGrid_{tabPage.Text}",
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = true,
                AutoScroll = false,
                Size = new Size(MAP_SIZE, MAP_SIZE),
                Tag = tabPage
            };

            var setStyleMethod = typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setStyleMethod.Invoke(mapGridPanel, new object[] { ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true });

            mapGridPanel.SetGrid(
                tabPage.MapName,
                tabPage._mapGridPanel._width,
                tabPage._mapGridPanel._height,
                tabPage._mapGridPanel._grid,
                true
            );

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Add Portal", null, (s, e) => AddPortal_Click(tabPage, mapGridPanel, _contextMenuPosition));
            contextMenu.Items.Add("Add Monster", null, (s, e) => AddMonster_Click(tabPage, mapGridPanel, _contextMenuPosition));
            contextMenu.Items.Add("Add NPC", null, (s, e) => AddNpc_Click(tabPage, mapGridPanel, _contextMenuPosition));
            contextMenu.Items.Add("Add Objective", null, (s, e) => AddObjective_Click(tabPage, mapGridPanel, _contextMenuPosition));
            contextMenu.Items.Add("Connect Portal", null, (s, e) => StartConnectingPortals(tabPage, mapGridPanel));
            contextMenu.Items.Add("Toggle Portal", null, (s, e) => TogglePortal_Click(tabPage, mapGridPanel, _contextMenuPosition));
            contextMenu.Items.Add("Remove Element", null, (s, e) => RemoveElement_Click(tabPage, mapGridPanel, _contextMenuPosition));

            mapGridPanel.Tag = (contextMenu, tabPage);
            mapGridPanel.MouseClick += (s, e) => MapGridPanel_MouseClick(mapGridPanel, e);
            mapGridPanel.DoubleClick += (s, e) => MapGridPanel_DoubleClick(mapGridPanel, e);
            mapGridPanel.InitializeDragAndDrop(tabPage);
            gridPanels.Add(tabPage, mapGridPanel);
            containerPanel.Controls.Add(mapGridPanel);
        }

        if (gridPanels.Any())
        {
            PositionPanelsBasedOnCoordinates();
            UpdateConnections();
        }
        else
        {
            ShowNoMapsMessage();
        }
    }
    private void MapGridPanel_DoubleClick(MapGridPanel panel, EventArgs e)
    {
        _mainForm.Invoke(new Action(() =>
        {
            _mainForm.tabControl1.SelectedTab = _mainForm.tabPage3;
            _mapTabControl.SelectedTab = panel._parentTab;
            Close();
        }));
    }
    private void InitializePortalConnections()
    {
        foreach (CustomTabPage sourceTab in _tabPages)
        {
            foreach (Portal sourcePortal in sourceTab.Portals)
            {
                if (string.IsNullOrEmpty(sourcePortal.MapTo) || sourcePortal.MapTo == "UNKNOWN")
                    continue;

                CustomTabPage targetTab = _tabPages.FirstOrDefault(t => t.Text.Contains(sourcePortal.MapTo));
                if (targetTab == null)
                    continue;

                Portal targetPortal = targetTab.Portals.FirstOrDefault(p =>
                    targetTab.Text.Contains(p.MapFrom) &&
                    sourceTab.Text.Contains(p.MapTo) &&
                    p.ToX == sourcePortal.FromX &&
                    p.ToY == sourcePortal.FromY
                );

                if (targetPortal != null)
                {
                    var sourceKey = (sourceTab, new Point(sourcePortal.FromX.Value, sourcePortal.FromY.Value));
                    var targetKey = (targetTab, new Point(targetPortal.FromX.Value, targetPortal.FromY.Value));
                    portalConnections[sourceKey] = targetKey;
                }
            }
        }
        containerPanel.MouseMove += ContainerPanel_MouseMoveForConnection;
        containerPanel.MouseClick += ContainerPanel_MouseClickForConnection;
    }
    #endregion

    #region Event Handlers
    private void CreateMapMenuItem_Click(object sender, EventArgs e)
    {
        // Collect existing positions
        List<Point> existingPositions = new List<Point>();
        foreach (var tabPage in _tabPages)
        {
            string[] coords = tabPage._txtMapCoordinates.Text.Split(',');
            if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
            {
                existingPositions.Add(new Point(x, y));
            }
        }

        // Open the GridSelector form to create new maps
        var gridSelector = new GridSelectorForm(existingPositions);
        gridSelector.FormClosed += (s, args) =>
        {
            if (gridSelector.DialogResult == DialogResult.OK && gridSelector.SelectedCoordinates.HasValue)
            {
                // Prompt for Map ID
                string mapId = PromptForMapId();
                if (!string.IsNullOrEmpty(mapId))
                {
                    CreateNewMapAtCoordinates(gridSelector.SelectedCoordinates.Value, mapId);

                    // Refresh MapOverviewForm to include the new maps
                    InitializeGridPanels();
                    InitializePortalConnections();
                    PositionPanelsBasedOnCoordinates();
                    try
                    {
                        UpdateConnections();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error updating connections: {ex}");
                    }
                    //CenterViewOnFirstMap();
                    containerPanel.Invalidate();
                }
                else
                {
                    MessageBox.Show("Map creation canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        };
        gridSelector.ShowDialog();
    }

    private void ContainerPanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isDragging = true;
            lastMousePosition = e.Location;
            containerPanel.Cursor = Cursors.Hand;
        }
    }

    private void ContainerPanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isDragging) return;

        int deltaX = e.X - lastMousePosition.X;
        int deltaY = e.Y - lastMousePosition.Y;

        Point currentScroll = containerPanel.AutoScrollPosition;
        Point newScrollPosition = new Point(
            Math.Max(0, -(currentScroll.X + deltaX)),
            Math.Max(0, -(currentScroll.Y + deltaY))
        );

        if (newScrollPosition != containerPanel.AutoScrollPosition)
        {
            containerPanel.AutoScrollPosition = newScrollPosition;
        }

        lastMousePosition = e.Location;
    }

    private void ContainerPanel_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        containerPanel.Cursor = Cursors.Default;
    }

    private void ContainerPanel_Paint(object sender, PaintEventArgs e)
    {
        if (_isDrawing) return;

        _isDrawing = true;
        _drawStopwatch.Restart();

        try
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw existing connections
            foreach (var connection in connections)
            {
                DrawConnection(g, connection);
            }

            // Draw the current connection being made
            if (isDrawingConnection && sourcePanel != null)
            {
                var startPoint = GetConnectionPoint(sourcePanel, sourcePortalPoint);
                using (var pen = new Pen(Color.FromArgb(180, 135, 206, 250), 2))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, startPoint, currentMousePosition);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ContainerPanel_Paint: {ex}");
        }
        finally
        {
            _drawStopwatch.Stop();
            _isDrawing = false;
        }
    }

    private void ContainerPanel_MouseMoveForConnection(object sender, MouseEventArgs e)
    {
        if (isDrawingConnection)
        {
            currentMousePosition = e.Location;
            containerPanel.Invalidate(); // Trigger repaint to update the line
        }
    }

    private void ContainerPanel_MouseClickForConnection(object sender, MouseEventArgs e)
    {
        if (!isDrawingConnection || e.Button != MouseButtons.Left) return;

        // Find the panel and grid position at click location
        foreach (var kvp in gridPanels)
        {
            var panel = kvp.Value;
            var tabPage = kvp.Key;

            // Convert click position to panel coordinates
            Point localPoint = new Point(
                e.X - panel.Location.X,
                e.Y - panel.Location.Y
            );

            // Check if click is within panel bounds
            if (localPoint.X >= 0 && localPoint.X < panel.Width &&
                localPoint.Y >= 0 && localPoint.Y < panel.Height)
            {
                // Convert to grid coordinates
                int gridX = localPoint.X / panel._cellSize;
                int gridY = localPoint.Y / panel._cellSize;

                // Check if clicked on a portal
                if (GetMarking(panel, gridX, gridY) == 0x40)
                {
                    var targetPortal = tabPage.Portals.FirstOrDefault(p =>
                        p.FromX == gridX &&
                        p.FromY == gridY);

                    if (targetPortal != null && targetPortal != sourcePortal)
                    {
                        ConnectPortals(sourcePortal, targetPortal);
                        FinishConnection();
                        return;
                    }
                }
            }
        }
    }
    #endregion

    #region Portal Management
    private void StartConnectingPortals(CustomTabPage tabPage, MapGridPanel panel)
    {
        if (!isConnectingPortals)
        {
            // Starting a new connection
            isConnectingPortals = true;
            sourcePanel = panel;
            sourceTabPage = tabPage;
            sourcePortalPoint = new Point(_contextMenuPosition.x, _contextMenuPosition.y);
            sourcePortal = tabPage.Portals.FirstOrDefault(p =>
                p.FromX == _contextMenuPosition.x &&
                p.FromY == _contextMenuPosition.y);

            containerPanel.Cursor = Cursors.Cross;
            MessageBox.Show("Select another portal to connect to.\nPress ESC or left-click anywhere else to cancel.",
                          "Connect Portals", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            // Attempting to complete a connection
            if (sourcePortal != null)
            {
                var targetPortal = tabPage.Portals.FirstOrDefault(p =>
                    p.FromX == _contextMenuPosition.x &&
                    p.FromY == _contextMenuPosition.y);

                if (targetPortal != null && targetPortal != sourcePortal)
                {
                    ConnectPortals(sourcePortal, targetPortal);
                    isConnectingPortals = false;
                    sourcePortal = null;
                    sourcePanel = null;
                    sourceTabPage = null;
                    containerPanel.Cursor = Cursors.Default;
                }
                else if (targetPortal == sourcePortal)
                {
                    MessageBox.Show("Cannot connect a portal to itself.",
                                  "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
    private void TogglePortal_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) Location)
    {
        var portal = tabPage.Portals.FirstOrDefault(p =>
            p.FromX == Location.X &&
            p.FromY == Location.Y);
        if (portal.PortalType == "Locked")
        {
            portal.PortalType = "TsNormal";
        }
        else if (portal.PortalType == "TsNormal")
        {
            portal.PortalType = "Locked";
        }
        else if (portal.PortalType == "TSEnd")
        {
            portal.PortalType = "TSEndClosed";
        }
        else
        {
            portal.PortalType = "TSEnd";
        }
        Refresh();
    }
    private void CancelPortalConnection()
    {
        isConnectingPortals = false;
        sourcePortal = null;
        sourcePanel = null;
        sourceTabPage = null;
        containerPanel.Cursor = Cursors.Default;
        // Add visual feedback
        MessageBox.Show("Portal connection cancelled.", "Connection Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void HandlePortalConnection(object sender, PortalClickEventArgs e)
    {
        if (selectedPortal == null)
        {
            selectedPortal = e.Portal;
            selectedPanel = e.Panel;
            return;
        }

        if (selectedPortal != e.Portal)
        {
            ConnectPortals(selectedPortal, e.Portal);
            selectedPortal = null;
            selectedPanel = null;
            isConnectingPortals = false;
            PortalClicked -= HandlePortalConnection;
            Refresh();
        }
    }

    private void ConnectPortals(Portal source, Portal target)
    {
        try
        {
            source.MapTo = target.MapFrom;
            source.ToX = target.FromX;
            source.ToY = target.FromY;
            source.cboMapTo.SelectedItem = target.MapFrom;
            source.txtToX.Text = target.FromX.ToString();
            source.txtToY.Text = target.FromY.ToString();

            target.MapTo = source.MapFrom;
            target.ToX = source.FromX;
            target.ToY = source.FromY;
            target.cboMapTo.SelectedItem = source.MapFrom;
            target.txtToX.Text = source.FromX.ToString();
            target.txtToY.Text = source.FromY.ToString();

            var sourceKey = (sourceTabPage, new Point(source.FromX.Value, source.FromY.Value));
            var targetKey = (target.customTabPage, new Point(target.FromX.Value, target.FromY.Value));

            portalConnections[sourceKey] = targetKey;
            portalConnections[targetKey] = sourceKey;

            UpdateConnections();
            MessageBox.Show("Portals connected successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error connecting portals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void FinishConnection()
    {
        isDrawingConnection = false;
        sourcePortal = null;
        sourcePanel = null;
        containerPanel.Cursor = Cursors.Default;
        containerPanel.Invalidate();
    }
    #endregion

    #region Utility Methods
    private string PromptForMapId()
    {
        using (var inputDialog = new InputBox("Enter Map ID", "Please enter the Map ID:"))
        {
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                return inputDialog.InputText;
            }
            else
            {
                return null;
            }
        }
    }
    private void CreateNewMapAtCoordinates(Point coordinates, string mapId)
    {
        // Load map data using the provided mapId
        var mapData = _mainForm.loadedMaps.FirstOrDefault(m => m.Id.ToString() == mapId);
        if (mapData == null)
        {
            MessageBox.Show($"Map with ID {mapId} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Create a new CustomTabPage and set coordinates
        var newTabPage = new CustomTabPage($"map_{mapId}", _mainForm, _mainForm.getMapNames)
        {
            AutoScroll = true
        };
        newTabPage._txtMapCoordinates.Text = $"{coordinates.X},{coordinates.Y}";

        // Initialize the new map grid panel
        newTabPage._mapGridPanel = new MapGridPanel();
        newTabPage.SetMapVnum(int.Parse(mapId));
        newTabPage.LoadMap(_mainForm);
        newTabPage.DisplayMapGrid(mapData);

        // Add the new tab to the TabControl
        _mapTabControl.TabPages.Add(newTabPage);
        TimeSpaceTool.mapTabs.Add(newTabPage);
        _mapTabControl.SelectedTab = newTabPage;
    }
    private void MapGridPanel_MouseClick(MapGridPanel panel, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && isConnectingPortals)
        {
            // Calculate click position using the panel's cell size
            var clickX = e.X / panel._cellSize;
            var clickY = e.Y / panel._cellSize;

            if (GetMarking(panel, clickX, clickY) != 0x40)
            {
                CancelPortalConnection();
                return;
            }
        }

        if (e.Button == MouseButtons.Right)
        {
            var (contextMenu, tabPage) = ((ContextMenuStrip, CustomTabPage))panel.Tag;

            // Use the panel's cell size for calculation
            int cellX = e.X / panel._cellSize;
            int cellY = e.Y / panel._cellSize;

            if (!panel.IsValidPosition(cellX, cellY))
                return;

            _contextMenuPosition = (cellX, cellY);

            bool isWalkable = panel.IsWalkable(cellX, cellY);
            bool isPortal = GetMarking(panel, cellX, cellY) == 0x40;

            foreach (ToolStripItem item in contextMenu.Items)
            {
                if (item.Text == "Remove Element")
                    item.Visible = !isWalkable;
                else if (item.Text == "Connect Portal" || item.Text == "Toggle Portal")
                    item.Visible = isPortal;
                else
                    item.Visible = isWalkable && !isPortal;
            }

            contextMenu.Show(panel, e.Location);
        }
    }
    private void CenterViewOnFirstMap()
    {
        if (!gridPanels.Any()) return;

        var firstPanel = gridPanels.First().Value;

        // Calculate the point to center on
        int panelCenterX = firstPanel.Location.X + firstPanel.Width / 2;
        int panelCenterY = firstPanel.Location.Y + firstPanel.Height / 2;

        // Set AutoScrollPosition (negative values)
        containerPanel.AutoScrollPosition = new Point(
            panelCenterX - containerPanel.ClientSize.Width / 2,
            panelCenterY - containerPanel.ClientSize.Height / 2
        );
    }
    #endregion
    #region Panel Positioning and Drawing
    private void PositionPanelsBasedOnCoordinates()
    {
        if (!gridPanels.Any()) return;

        originalPositions.Clear();

        foreach (var kvp in gridPanels)
        {
            var tabPage = kvp.Key;
            var panel = kvp.Value;

            string[] coords = tabPage._txtMapCoordinates.Text.Split(',');
            if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
            {
                // Position based on grid coordinates with padding
                int xPos = HEADER_SIZE + (x * (MAP_SIZE + PADDING));
                int yPos = HEADER_SIZE + (y * (MAP_SIZE + PADDING));

                originalPositions[tabPage] = new Point(xPos, yPos);
                panel.Location = originalPositions[tabPage];
                panel.Size = new Size(MAP_SIZE, MAP_SIZE);
                panel.Visible = true;
            }
        }

        UpdateContainerSize();
    }
    private void UpdateConnections()
    {
        connections.Clear();
        int addedCount = 0;

        foreach (var connection in portalConnections)
        {
            if (!gridPanels.TryGetValue(connection.Key.Item1, out var sourcePanel))
            {
                continue;
            }

            if (!gridPanels.TryGetValue(connection.Value.Item1, out var targetPanel))
            {
                continue;
            }

            var newConnection = new PortalConnection
            {
                StartPanel = sourcePanel,
                EndPanel = targetPanel,
                StartGrid = connection.Key.Item2,
                EndGrid = connection.Value.Item2
            };

            connections.Add(newConnection);
            addedCount++;
        }

        containerPanel.Invalidate();
    }
    private void UpdateContainerSize()
    {
        int totalWidth = HEADER_SIZE + (GRID_SIZE + 1) * (MAP_SIZE + PADDING);
        int totalHeight = HEADER_SIZE + (GRID_SIZE + 1) * (MAP_SIZE + PADDING);

        containerPanel.AutoScrollMinSize = new Size(totalWidth, totalHeight);
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
    }
    private void DrawConnection(Graphics g, PortalConnection connection)
    {
        try
        {
            var startPoint = GetConnectionPoint(connection.StartPanel, connection.StartGrid);
            var endPoint = GetConnectionPoint(connection.EndPanel, connection.EndGrid);

            using (var pen = new Pen(Color.FromArgb(180, 135, 206, 250), 2))
            {
                g.DrawLine(pen, startPoint, endPoint);

                // Draw arrowhead
                float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
                float arrowLength = 15;

                PointF arrowPoint1 = new PointF(
                    endPoint.X - arrowLength * (float)Math.Cos(angle - Math.PI / 6),
                    endPoint.Y - arrowLength * (float)Math.Sin(angle - Math.PI / 6));

                PointF arrowPoint2 = new PointF(
                    endPoint.X - arrowLength * (float)Math.Cos(angle + Math.PI / 6),
                    endPoint.Y - arrowLength * (float)Math.Sin(angle + Math.PI / 6));

                using (var brush = new SolidBrush(Color.FromArgb(180, 135, 206, 250)))
                {
                    g.FillPolygon(brush, new PointF[] { endPoint, arrowPoint1, arrowPoint2 });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error drawing connection {connection}: {ex}");
        }
    }
    private PointF GetConnectionPoint(MapGridPanel panel, Point gridPosition)
    {
        float cellX = gridPosition.X * panel._cellSize;
        float cellY = gridPosition.Y * panel._cellSize;

        float centerX = panel.Location.X + cellX + panel._cellSize / 2;
        float centerY = panel.Location.Y + cellY + panel._cellSize / 2;

        return new PointF(centerX, centerY);
    }
    private void ShowNoMapsMessage()
    {
        var noMapsLabel = new Label
        {
            Text = "No maps available to display.\nPlease open some map tabs first.",
            AutoSize = true,
            Location = new Point(PADDING, PADDING),
            ForeColor = Color.White
        };
        containerPanel.Controls.Add(noMapsLabel);
    }

    private class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true
            );
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }
    }
    #endregion

    #region MapEvents Management
    private void AddPortal_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) location)
    {
        if (tabPage.Portals.Count >= CustomTabPage.MAX_PORTALS)
        {
            MessageBox.Show("You don't need more than 4 Portals.");
            return;
        }

        string orientation = DetermineOrientation(location.X, location.Y, panel._width, panel._height);

        var portal = new Portal(
            tabPage.MapName,
            "DefaultTo",
            "Type1",
            orientation,
            location.X,
            location.Y,
            null,
            null,
            tabPage.GetMapNames,
            tabPage
        );

        tabPage.Portals.Add(portal);
        tabPage._portalPanel.Controls.Add(portal.CreatePortal());
        tabPage.SaveAndRefreshPortals(this, EventArgs.Empty, false);

        panel._grid[location.Y * panel._width + location.X] = 0x40;
        panel.Refresh();
        Refresh();
    }
    private void AddMonster_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) location)
    {
        var monster = new Monster(tabPage.MapName);
        tabPage.MonsterEvents.Add(monster);

        object[] rowData = new object[]
        {
            "",     // Vnum
            location.X,    // X
            location.Y,    // Y
            "",    // Wave
            false,  // AsTarget
            ""      // Attributes
        };

        tabPage._monsterDataGridView.Rows.Add(rowData);

        panel._grid[location.Y * panel._width + location.X] = 0x80;
        Refresh();
    }
    private void AddNpc_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) location)
    {
        var npc = new Npc(tabPage.MapName);
        tabPage.NpcEvents.Add(npc);

        object[] rowData = new object[]
        {
                "",     // Vnum
                location.X,    // X
                location.Y,    // Y
                ""      // Attributes
        };
        tabPage._npcDataGridView.Rows.Add(rowData);

        panel._grid[location.Y * panel._width + location.X] = 0x50;
        Refresh();
    }
    private void AddObjective_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) location)
    {
        if (tabPage.Objects.Count >= CustomTabPage.MAX_OBJECTS)
        {
            MessageBox.Show("You don't need more than 4 Objects.");
            return;
        }

        var mapObject = new MapObject(
            tabPage.MapName,
            "Object",
            location.X,
            location.Y,
            tabPage._allPortalsList,
            tabPage
        );

        tabPage.Objects.Add(mapObject);
        tabPage._objectivePanel.Controls.Add(mapObject.CreateObject());

        panel._grid[location.Y * panel._width + location.X] = 0x30;
        Refresh();
    }
    private string DetermineOrientation(int x, int y, int width, int height)
    {
        double centerX = width / 2.0;
        double centerY = height / 2.0;
        double angle = Math.Atan2(y - centerY, x - centerX) * (180 / Math.PI);

        return angle switch
        {
            <= 45 and > -45 => "East",
            <= 135 and > 45 => "South",
            <= -45 and > -135 => "North",
            _ => "West"
        };
    }
    public byte GetMarking(MapGridPanel panel, int x, int y)
    {
        return IsValidPosition(panel, x, y) ? panel._grid[y * panel._width + x] : (byte)0;
    }
    public bool IsValidPosition(MapGridPanel panel, int? x, int? y)
    {
        return x >= 0 && x < panel._width && y >= 0 && y < panel._height && panel._grid != null;
    }
    public void RemoveElement_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) location)
    {
        byte elementType = GetMarking(panel, location.X, location.Y);

        switch (elementType)
        {
            case 0x40: // Portal
                RemovePortal(tabPage, location.X, location.Y);
                break;
            case 0x30: // Objective
                RemoveObjective(tabPage, location.X, location.Y);
                break;
            case 0x50: // NPC
                RemoveNpc(tabPage, location.X, location.Y);
                break;
            case 0x80: // Monster
            case 0x90: // Target Monster
                RemoveMonster(tabPage, location.X, location.Y);
                break;
            default:
                return;
        }

        panel._grid[location.Y * panel._width + location.X] = 0;
        Refresh();
    }
    private void RemovePortal(CustomTabPage tabPage, int x, int y)
    {
        var portal = tabPage.Portals.FirstOrDefault(p => p.FromX == x && p.FromY == y);
        if (portal != null)
        {
            tabPage.Portals.Remove(portal);
            tabPage._portalPanel.Controls.Remove(portal.Panel);
            tabPage._portalPanel.Refresh();
            tabPage.SaveAndRefreshPortals(this, EventArgs.Empty, false);
        }
    }

    private void RemoveObjective(CustomTabPage tabPage, int x, int y)
    {
        var objective = tabPage.Objects.FirstOrDefault(o => o.GetX() == x && o.GetY() == y);
        if (objective != null)
        {
            tabPage.Objects.Remove(objective);
            tabPage._objectivePanel.Controls.Remove(objective.Panel);
            tabPage._objectivePanel.Refresh();
        }
    }

    private void RemoveMonster(CustomTabPage tabPage, int x, int y)
    {
        for (int i = tabPage._monsterDataGridView.Rows.Count - 1; i >= 0; i--)
        {
            var row = tabPage._monsterDataGridView.Rows[i];
            if (!row.IsNewRow &&
                Convert.ToInt32(row.Cells["X"].Value) == x &&
                Convert.ToInt32(row.Cells["Y"].Value) == y)
            {
                tabPage._monsterDataGridView.Rows.RemoveAt(i);
                break;
            }
        }
    }
    private void RemoveNpc(CustomTabPage tabPage, int x, int y)
    {
        for (int i = tabPage._npcDataGridView.Rows.Count - 1; i >= 0; i--)
        {
            var row = tabPage._npcDataGridView.Rows[i];
            if (!row.IsNewRow &&
                Convert.ToInt32(row.Cells["X"].Value) == x &&
                Convert.ToInt32(row.Cells["Y"].Value) == y)
            {
                tabPage._npcDataGridView.Rows.RemoveAt(i);
                break;
            }
        }
    }
    #endregion
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        CenterViewOnFirstMap();
    }
}
