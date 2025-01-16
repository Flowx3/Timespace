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
    private const int BASE_CELL_SIZE = 40;
    private const float MIN_ZOOM = 0.1f;
    private const float MAX_ZOOM = 5.0f;
    private Dictionary<CustomTabPage, MapGridPanel> gridPanels;
    private Panel viewportPanel;
    public float currentZoom = 1.0f;
    private Point lastMousePosition;
    private bool isDragging;
    private TabControl _mapTabControl;
    private List<CustomTabPage> _tabPages;
    private Point minCoordinates;
    private Point maxCoordinates;

    public MapOverviewForm(TabControl mapTabControl)
    {
        // Initialize fields first
        _mapTabControl = mapTabControl ?? throw new ArgumentNullException(nameof(mapTabControl));
        _tabPages = new List<CustomTabPage>();
        gridPanels = new Dictionary<CustomTabPage, MapGridPanel>();

        // Then initialize components
        InitializeComponent();

        // After components are initialized, set up the panels
        InitializeGridPanels();

        if (gridPanels.Any())
        {
            CalculateMapBoundaries();
            PositionPanelsBasedOnCoordinates();
            CenterAllMaps();
        }
        else
        {
            ShowNoMapsMessage();
        }
    }

    private void InitializeComponent()
    {
        Text = "Map Overview";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(50, 50, 50);

        viewportPanel = new DoubleBufferedPanel
        {
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(40, 40, 40),
            Visible = true
        };

        // Add event handlers
        viewportPanel.MouseDown += ViewportPanel_MouseDown;
        viewportPanel.MouseMove += ViewportPanel_MouseMove;
        viewportPanel.MouseUp += ViewportPanel_MouseUp;
        viewportPanel.MouseWheel += ViewportPanel_MouseWheel;
        viewportPanel.Paint += ViewportPanel_Paint;

        // Add the panel to controls
        Controls.Add(viewportPanel);
    }

    private void CalculateMapBoundaries()
    {
        minCoordinates = new Point(int.MaxValue, int.MaxValue);
        maxCoordinates = new Point(int.MinValue, int.MinValue);

        foreach (var tabPage in _tabPages)
        {
            var xControl = tabPage.Controls["txtbox_mapcoordinates_x"];
            var yControl = tabPage.Controls["txtbox_mapcoordinates_y"];

            if (xControl != null && yControl != null &&
                int.TryParse(xControl.Text, out int x) &&
                int.TryParse(yControl.Text, out int y))
            {
                minCoordinates.X = Math.Min(minCoordinates.X, x);
                minCoordinates.Y = Math.Min(minCoordinates.Y, y);
                maxCoordinates.X = Math.Max(maxCoordinates.X, x);
                maxCoordinates.Y = Math.Max(maxCoordinates.Y, y);
            }
        }

        if (minCoordinates.X == int.MaxValue)
        {
            minCoordinates = new Point(0, 0);
            maxCoordinates = new Point(0, 0);
        }
    }

    private void PositionPanelsBasedOnCoordinates()
    {
        if (!gridPanels.Any()) return;

        foreach (var kvp in gridPanels)
        {
            var tabPage = kvp.Key;
            var panel = kvp.Value;

            if (int.TryParse(tabPage.Controls["txtbox_mapcoordinates_x"]?.Text, out int mapX) &&
                int.TryParse(tabPage.Controls["txtbox_mapcoordinates_y"]?.Text, out int mapY))
            {
                // Calculate position relative to minimum coordinates
                int relativeX = mapX - minCoordinates.X;
                int relativeY = mapY - minCoordinates.Y;

                // Calculate actual position with padding and grid size
                int x = PADDING + (relativeX * (11 * BASE_CELL_SIZE + PADDING));
                int y = PADDING + (relativeY * (11 * BASE_CELL_SIZE + PADDING));

                panel.Location = new Point(x, y);
                panel._cellSize = BASE_CELL_SIZE;
                panel.Size = new Size(11 * BASE_CELL_SIZE, 11 * BASE_CELL_SIZE);

                // Apply current zoom
                panel.Scale(new SizeF(currentZoom, currentZoom));
            }
        }

        // Update viewport scroll size
        UpdateViewportScrollSize();
    }

    private void UpdateViewportScrollSize()
    {
        // Guard against null or empty collections
        if (gridPanels == null || !gridPanels.Any()) return;

        int gridWidth = maxCoordinates.X - minCoordinates.X + 1;
        int gridHeight = maxCoordinates.Y - minCoordinates.Y + 1;

        int totalWidth = (int)((gridWidth * (11 * BASE_CELL_SIZE + PADDING) + PADDING) * currentZoom);
        int totalHeight = (int)((gridHeight * (11 * BASE_CELL_SIZE + PADDING) + PADDING) * currentZoom);

        // Ensure minimum size is at least the client size
        viewportPanel.AutoScrollMinSize = new Size(
            Math.Max(totalWidth, viewportPanel.ClientSize.Width),
            Math.Max(totalHeight, viewportPanel.ClientSize.Height)
        );
    }

    private void CenterAllMaps()
    {
        if (!gridPanels.Any()) return;

        // Calculate the center of all maps
        int centerX = (int)((minCoordinates.X + maxCoordinates.X) / 2.0f * (11 * BASE_CELL_SIZE + PADDING) * currentZoom);
        int centerY = (int)((minCoordinates.Y + maxCoordinates.Y) / 2.0f * (11 * BASE_CELL_SIZE + PADDING) * currentZoom);

        // Calculate scroll position to center the maps
        int scrollX = Math.Max(0, centerX - (viewportPanel.ClientSize.Width / 2));
        int scrollY = Math.Max(0, centerY - (viewportPanel.ClientSize.Height / 2));

        viewportPanel.AutoScrollPosition = new Point(scrollX, scrollY);
    }

    private void ViewportPanel_MouseWheel(object sender, MouseEventArgs e)
    {
        if (ModifierKeys != Keys.Control) return;

        // Store mouse position relative to viewport
        Point mousePos = viewportPanel.PointToClient(Cursor.Position);

        // Calculate zoom
        float zoomDelta = e.Delta > 0 ? 1.1f : 0.9f;
        float newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, currentZoom * zoomDelta));

        if (newZoom != currentZoom)
        {
            // Store scroll position before zoom
            Point scrollPos = new Point(-viewportPanel.AutoScrollPosition.X, -viewportPanel.AutoScrollPosition.Y);

            // Calculate new scroll position to keep mouse position fixed
            float zoomFactor = newZoom / currentZoom;
            int newScrollX = (int)((mousePos.X + scrollPos.X) * zoomFactor - mousePos.X);
            int newScrollY = (int)((mousePos.Y + scrollPos.Y) * zoomFactor - mousePos.Y);

            // Apply new zoom
            currentZoom = newZoom;

            // Update all panels with new zoom
            foreach (var panel in gridPanels.Values)
            {
                panel.Scale(new SizeF(currentZoom, currentZoom));
                panel.Invalidate();
            }

            // Update viewport
            UpdateViewportScrollSize();
            viewportPanel.AutoScrollPosition = new Point(newScrollX, newScrollY);
            viewportPanel.Invalidate();
        }
    }

    private void ViewportPanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
        {
            isDragging = true;
            lastMousePosition = e.Location;
            viewportPanel.Cursor = Cursors.Hand;
        }
    }

    private void ViewportPanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            int deltaX = lastMousePosition.X - e.X;
            int deltaY = lastMousePosition.Y - e.Y;

            Point currentScroll = new Point(-viewportPanel.AutoScrollPosition.X, -viewportPanel.AutoScrollPosition.Y);
            viewportPanel.AutoScrollPosition = new Point(currentScroll.X + deltaX, currentScroll.Y + deltaY);

            lastMousePosition = e.Location;
            viewportPanel.Invalidate();
        }
    }

    private void ViewportPanel_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        viewportPanel.Cursor = Cursors.Default;
    }

    private void ViewportPanel_Resize(object sender, EventArgs e)
    {
        UpdateViewportScrollSize();
    }
    private void InitializeGridPanels()
    {
        // Clear existing panels
        viewportPanel.Controls.Clear();
        gridPanels.Clear();
        _tabPages.Clear();

        // Get all CustomTabPage controls from the tab control
        foreach (CustomTabPage tabPage in _mapTabControl.Controls.OfType<CustomTabPage>())
        {
            if (tabPage._mapGridPanel == null) continue;

            _tabPages.Add(tabPage);

            var mapGridPanel = new MapGridPanel
            {
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = true,
                AutoScroll = false
            };

            mapGridPanel.SetGrid(
                tabPage.MapName,
                tabPage._mapGridPanel._width,
                tabPage._mapGridPanel._height,
                tabPage._mapGridPanel._grid
            );

            gridPanels.Add(tabPage, mapGridPanel);
            viewportPanel.Controls.Add(mapGridPanel);
        }
    }
    private void ScrollToFirstMap()
    {
        if (_tabPages.Count > 0 && gridPanels.Any())
        {
            var firstTab = _tabPages[0];
            if (gridPanels.TryGetValue(firstTab, out var firstPanel))
            {
                // Calculate scroll position to center the first map
                int scrollX = Math.Max(0, firstPanel.Location.X - (viewportPanel.ClientSize.Width - firstPanel.Width) / 2);
                int scrollY = Math.Max(0, firstPanel.Location.Y - (viewportPanel.ClientSize.Height - firstPanel.Height) / 2);

                // Set scroll position
                viewportPanel.AutoScrollPosition = new Point(scrollX, scrollY);
            }
        }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        DrawPortalConnections(e.Graphics);
    }
    private void DrawPortalConnections(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0, 150, 136), 2);

        foreach (var sourcePanel in gridPanels.Values)
        {
            var sourceTabPage = _tabPages.FirstOrDefault(x => x.Text == sourcePanel._currentMapId);

            if (sourceTabPage == null) continue;

            foreach (var portal in sourceTabPage.Portals)
            {
                if (string.IsNullOrEmpty(portal.MapTo)) continue;

                var targetTabPage = _tabPages.FirstOrDefault(x => x.Text == portal.MapTo);
                if (targetTabPage == null) continue;

                if (!gridPanels.TryGetValue(targetTabPage, out var targetPanel)) continue;

                var targetPortal = targetTabPage.Portals.FirstOrDefault(p =>
                    p.MapTo == sourceTabPage.Text && p.ToX == portal.FromX && p.ToY == portal.FromY);

                if (targetPortal == null) continue;

                Point startPoint = GetPortalConnectionPoint(sourcePanel, portal);
                Point endPoint = GetPortalConnectionPoint(targetPanel, targetPortal);
                DrawArrowLine(g, pen, startPoint, endPoint);
            }
        }
    }

    private Point GetPortalConnectionPoint(MapGridPanel panel, Portal portal)
    {
        // Calculate the center point of the cell  
        float cellCenterX = panel.Location.X + (portal.FromX.Value * BASE_CELL_SIZE + BASE_CELL_SIZE / 2) * currentZoom;
        float cellCenterY = panel.Location.Y + (portal.FromY.Value * BASE_CELL_SIZE + BASE_CELL_SIZE / 2) * currentZoom;

        // Adjust the connection point based on the portal's orientation  
        float offset = BASE_CELL_SIZE * currentZoom / 2;
        return portal.MinimapOrientation switch
        {
            "North" => new Point((int)cellCenterX, (int)(cellCenterY - offset)),
            "South" => new Point((int)cellCenterX, (int)(cellCenterY + offset)),
            "East" => new Point((int)(cellCenterX + offset), (int)cellCenterY),
            "West" => new Point((int)(cellCenterX - offset), (int)cellCenterY),
            _ => new Point((int)cellCenterX, (int)cellCenterY)
        };
    }

    private void DrawArrowLine(Graphics g, Pen pen, Point start, Point end)
    {
        // Calculate the vector between points  
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float length = (float)Math.Sqrt(dx * dx + dy * dy);

        // Calculate unit vector  
        float ux = dx / length;
        float uy = dy / length;

        // Calculate arrow head size based on zoom level  
        float arrowSize = 10f * currentZoom;

        // Draw the main line  
        g.DrawLine(pen, start, end);

        // Calculate arrow head points  
        PointF[] arrowHead = new PointF[]
        {
        end,
        new PointF(
            end.X - arrowSize * (float)(Math.Cos(Math.Atan2(dy, dx) + Math.PI / 6)),
            end.Y - arrowSize * (float)(Math.Sin(Math.Atan2(dy, dx) + Math.PI / 6))
        ),
        new PointF(
            end.X - arrowSize * (float)(Math.Cos(Math.Atan2(dy, dx) - Math.PI / 6)),
            end.Y - arrowSize * (float)(Math.Sin(Math.Atan2(dy, dx) - Math.PI / 6))
        )
        };

        // Draw the arrow head  
        g.FillPolygon(new SolidBrush(pen.Color), arrowHead);
    }
    private Point GetPortalPoint(MapGridPanel panel, Portal portal)
    {
        return new Point(
            (int)((portal.FromX.Value * BASE_CELL_SIZE + BASE_CELL_SIZE / 2) * currentZoom),
            (int)((portal.FromY.Value * BASE_CELL_SIZE + BASE_CELL_SIZE / 2) * currentZoom)
        );
    }
    private void ViewportPanel_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0, 150, 136), 2);

        foreach (var panelKvp in gridPanels)
        {
            var panel = panelKvp.Value;
            var tabPage = _tabPages.FirstOrDefault(x => x.Text == panel._currentMapId);
            if (tabPage == null) continue;

            foreach (var portal in tabPage.Portals)
            {
                if (string.IsNullOrEmpty(portal.MapTo)) continue;

                var targetTabPage = _tabPages.FirstOrDefault(x => x.Text == portal.MapTo);
                if (targetTabPage == null) continue;

                if (!gridPanels.TryGetValue(targetTabPage, out var targetPanel)) continue;

                var targetPortal = targetTabPage.Portals.FirstOrDefault(p =>
                    p.MapTo == tabPage.Text && p.ToX == portal.FromX && p.ToY == portal.FromY);

                if (targetPortal == null) continue;

                // Calculate connection points in panel coordinates  
                Point startPoint = GetPortalConnectionPoint(panel, portal);
                Point endPoint = GetPortalConnectionPoint(targetPanel, targetPortal);

                // Convert to viewport coordinates  
                startPoint = panel.Parent.PointToClient(panel.PointToScreen(startPoint));
                endPoint = targetPanel.Parent.PointToClient(targetPanel.PointToScreen(endPoint));

                // Draw connection line with arrow  
                DrawArrowLine(e.Graphics, pen, startPoint, endPoint);
            }
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
        viewportPanel.Controls.Add(noMapsLabel);
    }
    private class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
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

    private void ConnectPortals(CustomTabPage sourceTab, Portal source, CustomTabPage targetTab, Portal target)
    {
        // Set source portal connections
        source.MapTo = targetTab.Text;
        source.ToX = target.FromX;
        source.ToY = target.FromY;

        // Set target portal connections
        target.MapTo = sourceTab.Text;
        target.ToX = source.FromX;
        target.ToY = source.FromY;

        // Set orientations based on relative positions
        if (source.FromY > target.FromY)
        {
            source.MinimapOrientation = "North";
            target.MinimapOrientation = "South";
        }
        else if (source.FromY < target.FromY)
        {
            source.MinimapOrientation = "South";
            target.MinimapOrientation = "North";
        }
        else if (source.FromX < target.FromX)
        {
            source.MinimapOrientation = "East";
            target.MinimapOrientation = "West";
        }
        else
        {
            source.MinimapOrientation = "West";
            target.MinimapOrientation = "East";
        }

        sourceTab.SaveAndRefreshPortals(this, EventArgs.Empty, false);
        targetTab.SaveAndRefreshPortals(this, EventArgs.Empty, false);
    }
}
