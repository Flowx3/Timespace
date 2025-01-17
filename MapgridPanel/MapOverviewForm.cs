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
    private const int MAP_SIZE = 450;
    private const float MIN_ZOOM = 0.1f;
    private const float MAX_ZOOM = 5.0f;
    private Dictionary<CustomTabPage, MapGridPanel> gridPanels;
    private DoubleBufferedPanel containerPanel;
    private float currentZoom = 1.0f;
    private Point lastMousePosition;
    private bool isDragging;
    private TabControl _mapTabControl;
    private List<CustomTabPage> _tabPages;
    private Dictionary<CustomTabPage, Point> originalPositions;
    private bool isConnectingPortals;
    private ContextMenuStrip _contextMenu;
    private (int x, int y) _contextMenuPosition;
    private Portal selectedPortal;
    private MapGridPanel selectedPanel;
    private Dictionary<(CustomTabPage, Point), (CustomTabPage, Point)> portalConnections;
    private event EventHandler<PortalClickEventArgs> PortalClicked;

    public MapOverviewForm(TabControl mapTabControl)
    {
        _mapTabControl = mapTabControl ?? throw new ArgumentNullException(nameof(mapTabControl));
        _tabPages = new List<CustomTabPage>();
        gridPanels = new Dictionary<CustomTabPage, MapGridPanel>();
        originalPositions = new Dictionary<CustomTabPage, Point>();

        InitializeComponent();
        InitializeGridPanels();

        if (!gridPanels.Any())
        {
            ShowNoMapsMessage();
        }
        else
        {
            PositionPanelsBasedOnCoordinates();
        }

        portalConnections = new Dictionary<(CustomTabPage, Point), (CustomTabPage, Point)>();
        InitializePortalConnections();
        AddContextMenuToGridPanels();

        // Subscribe to the Paint event
        containerPanel.Paint += ContainerPanel_Paint;
    }

    private void InitializeComponent()
    {
        Text = "Map Overview";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(50, 50, 50);

        containerPanel = new DoubleBufferedPanel
        {
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(40, 40, 40),
            Visible = true
        };

        containerPanel.MouseDown += ContainerPanel_MouseDown;
        containerPanel.MouseMove += ContainerPanel_MouseMove;
        containerPanel.MouseUp += ContainerPanel_MouseUp;
        containerPanel.MouseWheel += ContainerPanel_MouseWheel;

        Controls.Add(containerPanel);
    }

    private void InitializeGridPanels()
    {
        containerPanel.Controls.Clear();
        gridPanels.Clear();
        _tabPages.Clear();

        System.Diagnostics.Debug.WriteLine($"Total TabPages: {_mapTabControl.Controls.Count}");

        foreach (CustomTabPage tabPage in _mapTabControl.Controls.OfType<CustomTabPage>())
        {
            if (tabPage._mapGridPanel == null)
            {
                System.Diagnostics.Debug.WriteLine($"Skipping tab {tabPage.Text} - no MapGridPanel");
                continue;
            }

            _tabPages.Add(tabPage);
            System.Diagnostics.Debug.WriteLine($"Processing tab {tabPage.Text}");

            // Get coordinates directly from the MapGridPanel or CustomTabPage properties
            Point coordinates = tabPage._txtMapCoordinates.Text.Split(',')
                .Select(int.Parse)
                .Aggregate(new Point(), (p, v) => new Point(p.X == 0 ? v : p.X, p.X != 0 ? v : p.Y)); // Get coordinates from the property
            System.Diagnostics.Debug.WriteLine($"Coordinates found: X={coordinates.X}, Y={coordinates.Y}");

            var mapGridPanel = new MapGridPanel
            {
                Name = $"MapGrid_{tabPage.Text}",
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = true,
                AutoScroll = false,
                Size = new Size(MAP_SIZE, MAP_SIZE)
            };

            mapGridPanel.SetGrid(
                tabPage.MapName,
                tabPage._mapGridPanel._width,
                tabPage._mapGridPanel._height,
                tabPage._mapGridPanel._grid
            );

            gridPanels.Add(tabPage, mapGridPanel);
            containerPanel.Controls.Add(mapGridPanel);
        }

        System.Diagnostics.Debug.WriteLine($"Created {gridPanels.Count} MapGridPanels");
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
    }
    private void AddContextMenuToGridPanels()
    {
        foreach (var kvp in gridPanels)
        {
            var panel = kvp.Value;
            var tabPage = kvp.Key;

            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Add Portal", null, (s, e) => AddPortal_Click(tabPage, panel, _contextMenuPosition));
            _contextMenu.Items.Add("Add Monster", null, (s, e) => AddMonster_Click(tabPage, panel, _contextMenuPosition));
            _contextMenu.Items.Add("Add NPC", null, (s, e) => AddNpc_Click(tabPage, panel, _contextMenuPosition));
            _contextMenu.Items.Add("Add Objective", null, (s, e) => AddObjective_Click(tabPage, panel, _contextMenuPosition));
            _contextMenu.Items.Add("Connect Portal", null, (s, e) => StartConnectingPortals(tabPage, panel));
            _contextMenu.Items.Add("Remove Element", null, (s, e) => RemoveElement_Click(tabPage, panel, _contextMenuPosition));
            panel.MouseClick += (s, e) => MapGridPanel_MouseClick(tabPage, panel, e);
        }
    }
    private void MapGridPanel_MouseClick(CustomTabPage tabPage, MapGridPanel panel, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
            return;

        int cellX = e.X / panel._cellSize;
        int cellY = e.Y / panel._cellSize;

        if (!panel.IsValidPosition(cellX, cellY))
            return;

        _contextMenuPosition = (cellX, cellY);

        bool isWalkable = panel.IsWalkable(cellX, cellY);
        if (isWalkable)
        {
            _contextMenu.Items[0].Visible = true;
            _contextMenu.Items[1].Visible = true;
            _contextMenu.Items[2].Visible = true;
            _contextMenu.Items[3].Visible = true;
            _contextMenu.Items[4].Visible = false;
        }
        else
        {
            _contextMenu.Items[0].Visible = false;
            _contextMenu.Items[1].Visible = false;
            _contextMenu.Items[2].Visible = false;
            _contextMenu.Items[3].Visible = false;
            _contextMenu.Items[4].Visible = true;
        }

        // Manually show the context menu at the mouse location
        _contextMenu.Show(panel, e.Location);
    }

    private void StartConnectingPortals(CustomTabPage tabPage, MapGridPanel panel)
    {
        if (!isConnectingPortals)
        {
            isConnectingPortals = true;
            PortalClicked += HandlePortalConnection;
        }
    }

    private void HandlePortalConnection(object sender, PortalClickEventArgs e)
    {
        if (selectedPortal == null)
        {
            selectedPortal = e.Portal;
            selectedPanel = e.Panel;
            return;
        }

        // Connect the portals
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
        // Update source portal
        source.MapTo = target.MapFrom;
        source.ToX = target.FromX;
        source.ToY = target.FromY;
        source.cboMapTo.SelectedItem = target.MapFrom;
        source.txtToX.Text = target.FromX.ToString();
        source.txtToY.Text = target.FromY.ToString();

        // Update target portal
        target.MapTo = source.MapFrom;
        target.ToX = source.FromX;
        target.ToY = source.FromY;
        target.cboMapTo.SelectedItem = source.MapFrom;
        target.txtToX.Text = source.FromX.ToString();
        target.txtToY.Text = source.FromY.ToString();

        // Update connections dictionary
        var sourceTab = _tabPages.First(t => t.Text == source.MapFrom);
        var targetTab = _tabPages.First(t => t.Text == target.MapFrom);

        var sourceKey = (sourceTab, new Point(source.FromX.Value, source.FromY.Value));
        var targetKey = (targetTab, new Point(target.FromX.Value, target.FromY.Value));

        portalConnections[sourceKey] = targetKey;
        portalConnections[targetKey] = sourceKey;
    }

    private void PositionPanelsBasedOnCoordinates()
    {
        if (!gridPanels.Any()) return;

        var coordinates = new Dictionary<CustomTabPage, Point>();
        originalPositions.Clear();

        // Collect coordinates
        foreach (var kvp in gridPanels)
        {
            var tabPage = kvp.Key;
            Point mapCoords = tabPage._txtMapCoordinates.Text.Split(',')
            .Select(int.Parse)
            .Aggregate(new Point(), (p, v) => new Point(p.X == 0 ? v : p.X, p.X != 0 ? v : p.Y)); // Get coordinates from the property

            if (mapCoords != Point.Empty)
            {
                coordinates[tabPage] = mapCoords;
            }
        }

        if (!coordinates.Any()) return;

        // Find boundaries
        int minX = coordinates.Values.Min(p => p.X);
        int minY = coordinates.Values.Min(p => p.Y);
        int maxX = coordinates.Values.Max(p => p.X);
        int maxY = coordinates.Values.Max(p => p.Y);

        // Calculate base spacing (at zoom level 1.0)
        int baseSpacing = MAP_SIZE + PADDING;

        // Position each panel
        foreach (var kvp in coordinates)
        {
            var tabPage = kvp.Key;
            var mapCoord = kvp.Value;
            var panel = gridPanels[tabPage];

            // Calculate and store the original (unzoomed) position
            int xPos = PADDING + ((mapCoord.X - minX) * baseSpacing);
            int yPos = PADDING + ((mapCoord.Y - minY) * baseSpacing);
            originalPositions[tabPage] = new Point(xPos, yPos);

            // Set initial position
            panel.Location = originalPositions[tabPage];
            panel.Size = new Size(MAP_SIZE, MAP_SIZE);
            panel.Visible = true;
        }

        // Set container size
        UpdateContainerSize();
    }
    private void UpdateContainerSize()
    {
        if (!originalPositions.Any()) return;

        // Find the furthest point considering both position and panel size
        int maxRight = originalPositions.Values.Max(p => p.X);
        int maxBottom = originalPositions.Values.Max(p => p.Y);

        // Calculate total size with zoom factor
        int totalWidth = (int)((maxRight + MAP_SIZE + PADDING) * currentZoom);
        int totalHeight = (int)((maxBottom + MAP_SIZE + PADDING) * currentZoom);

        containerPanel.AutoScrollMinSize = new Size(totalWidth, totalHeight);
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
        if (isDragging)
        {
            int deltaX = lastMousePosition.X - e.X;
            int deltaY = lastMousePosition.Y - e.Y;

            // Get current scroll position
            Point currentScroll = containerPanel.AutoScrollPosition;

            // Update scroll position (note that AutoScrollPosition uses negative values)
            containerPanel.AutoScrollPosition = new Point(
                -currentScroll.X + deltaX,
                -currentScroll.Y + deltaY
            );

            lastMousePosition = e.Location;
        }
    }

    private void ContainerPanel_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        containerPanel.Cursor = Cursors.Default;
    }
    private void ContainerPanel_MouseWheel(object sender, MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            // Store old mouse position relative to content
            Point oldMousePos = containerPanel.PointToClient(Cursor.Position);
            oldMousePos.X += -containerPanel.AutoScrollPosition.X;
            oldMousePos.Y += -containerPanel.AutoScrollPosition.Y;

            // Calculate new zoom
            float zoomDelta = e.Delta > 0 ? 1.1f : 0.9f;
            float newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, currentZoom * zoomDelta));

            if (newZoom != currentZoom)
            {
                // Update all panels based on their original positions
                foreach (var kvp in gridPanels)
                {
                    var tabPage = kvp.Key;
                    var panel = kvp.Value;
                    var originalPos = originalPositions[tabPage];

                    // Scale from original position
                    int newX = (int)(originalPos.X * newZoom);
                    int newY = (int)(originalPos.Y * newZoom);
                    int newSize = (int)(MAP_SIZE * newZoom);

                    panel.Location = new Point(newX, newY);
                    panel.Size = new Size(newSize, newSize);
                    panel.UpdateScale(newZoom);
                }

                // Update container size
                currentZoom = newZoom;
                UpdateContainerSize();

                // Calculate new mouse position and adjust scroll
                Point newMousePos = containerPanel.PointToClient(Cursor.Position);
                int scrollX = (int)(oldMousePos.X * newZoom / currentZoom) - newMousePos.X;
                int scrollY = (int)(oldMousePos.Y * newZoom / currentZoom) - newMousePos.Y;

                containerPanel.AutoScrollPosition = new Point(scrollX, scrollY);

                containerPanel.Invalidate();
            }
        }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
    }
    private void DrawPortalConnections(Graphics g)
    {
        if (!portalConnections.Any()) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var pen = new Pen(Color.LightBlue, 2f);
        pen.CustomEndCap = new AdjustableArrowCap(5, 5);

        foreach (var connection in portalConnections)
        {
            var sourcePanel = gridPanels[connection.Key.Item1];
            var targetPanel = gridPanels[connection.Value.Item1];

            Point sourcePoint = new Point(
                sourcePanel.Location.X + (int)(connection.Key.Item2.X * sourcePanel._cellSize * currentZoom) + (int)(sourcePanel._cellSize * currentZoom / 2),
                sourcePanel.Location.Y + (int)(connection.Key.Item2.Y * sourcePanel._cellSize * currentZoom) + (int)(sourcePanel._cellSize * currentZoom / 2)
            );

            Point targetPoint = new Point(
                targetPanel.Location.X + (int)(connection.Value.Item2.X * targetPanel._cellSize * currentZoom) + (int)(targetPanel._cellSize * currentZoom / 2),
                targetPanel.Location.Y + (int)(connection.Value.Item2.Y * targetPanel._cellSize * currentZoom) + (int)(targetPanel._cellSize * currentZoom / 2)
            );

            sourcePoint.X -= -containerPanel.AutoScrollPosition.X;
            sourcePoint.Y -= -containerPanel.AutoScrollPosition.Y;
            targetPoint.X -= -containerPanel.AutoScrollPosition.X;
            targetPoint.Y -= -containerPanel.AutoScrollPosition.Y;

            g.DrawLine(pen, sourcePoint, targetPoint);
        }
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
            DoubleBuffered = true;
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true
            );
        }
    }

    private void PositionConnectedPanels(string currentMapId, Dictionary<string, Point> coordinates, HashSet<string> positioned)
    {
        var tabPage = _tabPages.FirstOrDefault(x => x.Text == currentMapId);

        foreach (var portal in tabPage.Portals)
        {
            if (string.IsNullOrEmpty(portal.MapTo) || !gridPanels.ContainsKey(_tabPages.FirstOrDefault(x => x.Text == portal.MapTo)) || positioned.Contains(portal.MapTo))
                continue;

            // Determine relative position based on portal orientation
            Point currentPos = coordinates[currentMapId];
            Point newPos = portal.MinimapOrientation switch
            {
                "North" => new Point(currentPos.X, currentPos.Y - 1),
                "South" => new Point(currentPos.X, currentPos.Y + 1),
                "East" => new Point(currentPos.X + 1, currentPos.Y),
                "West" => new Point(currentPos.X - 1, currentPos.Y),
                _ => currentPos
            };

            coordinates[portal.MapTo] = newPos;
            positioned.Add(portal.MapTo);
            PositionConnectedPanels(portal.MapTo, coordinates, positioned);
        }
    }
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

        // Update marking and force refresh
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
    public bool IsValidPosition(MapGridPanel panel,int? x, int? y)
    {
        return x >= 0 && x < panel._width && y >= 0 && y < panel._height && panel._grid != null;
    }
    public void RemoveElement_Click(CustomTabPage tabPage, MapGridPanel panel, (int X, int Y) location)
    {
        byte elementType = GetMarking(panel, location.X, location.Y);

        switch (elementType)
        {
            case 0x40: // Portal
                RemovePortal(tabPage ,location.X, location.Y);
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
    private void RemovePortal(CustomTabPage tabPage,int x, int y)
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
    private void ContainerPanel_Paint(object sender, PaintEventArgs e)
    {
        DrawPortalConnections(e.Graphics);
    }
}
