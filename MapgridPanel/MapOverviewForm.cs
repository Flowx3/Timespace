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
    private const int PADDING = 15;
    private const int GRID_SIZE = 11;
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
    private const int MAX_PANEL_SIZE = 300; 
    public MapOverviewForm(TabControl mapTabControl)
    {
        _mapTabControl = mapTabControl;
        _tabPages = new List<CustomTabPage>();
        InitializeComponent();
        InitializeGridPanels(mapTabControl);
        PositionPanelsInGrid();
        if (!gridPanels.Any())
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
        viewportPanel.MouseDown += ViewportPanel_MouseDown;
        viewportPanel.MouseMove += ViewportPanel_MouseMove;
        viewportPanel.MouseUp += ViewportPanel_MouseUp;
        viewportPanel.MouseWheel += ViewportPanel_MouseWheel;
        viewportPanel.Paint += ViewportPanel_Paint;
        Controls.Add(viewportPanel);
        gridPanels = new Dictionary<CustomTabPage, MapGridPanel>();
    }

    private void PositionPanelsInGrid()
    {
        if (!gridPanels.Any()) return;

        int totalPanels = gridPanels.Count;
        int numberOfColumns = (int)Math.Ceiling(Math.Sqrt(totalPanels));
        int numberOfRows = (int)Math.Ceiling((double)totalPanels / numberOfColumns);

        int maxPanelWidth = gridPanels.Values.Max(p => p.Width);
        int maxPanelHeight = gridPanels.Values.Max(p => p.Height);

        int i = 0;
        foreach (var panel in gridPanels.Values)
        {
            int row = i / numberOfColumns;
            int col = i % numberOfColumns;

            int x = col * (maxPanelWidth + PADDING * 2) + PADDING;
            int y = row * (maxPanelHeight + PADDING * 2) + PADDING;

            panel.Location = new Point(x, y);
            i++;
        }

        // Update viewport scroll size
        viewportPanel.AutoScrollMinSize = new Size(
            numberOfColumns * (maxPanelWidth + PADDING * 2) + PADDING,
            numberOfRows * (maxPanelHeight + PADDING * 2) + PADDING
        );
    }

    private void InitializeGridPanels(TabControl mapTabControl)
    {
        foreach (CustomTabPage tabPage in mapTabControl.Controls.OfType<CustomTabPage>())
        {
            if (tabPage._mapGridPanel == null) continue;
            _tabPages.Add(tabPage);

            var mapGridPanel = new MapGridPanel
            {
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = true,
                AutoScroll = false // Disable scrolling for overview panels
            };

            // Calculate the appropriate cell size to fit the panel within MAX_PANEL_SIZE
            int maxDimension = Math.Max(tabPage._mapGridPanel._width, tabPage._mapGridPanel._height);
            int cellSize = Math.Min(BASE_CELL_SIZE, MAX_PANEL_SIZE / maxDimension);

            mapGridPanel._cellSize = cellSize; // Set the cell size directly
            mapGridPanel.SetGrid(tabPage.MapName, tabPage._mapGridPanel._width, tabPage._mapGridPanel._height, tabPage._mapGridPanel._grid);

            // Calculate the actual panel size based on the grid dimensions and cell size
            int panelWidth = tabPage._mapGridPanel._width * cellSize;
            int panelHeight = tabPage._mapGridPanel._height * cellSize;

            // Ensure the panel size doesn't exceed MAX_PANEL_SIZE
            if (panelWidth > MAX_PANEL_SIZE || panelHeight > MAX_PANEL_SIZE)
            {
                float scale = Math.Min((float)MAX_PANEL_SIZE / panelWidth, (float)MAX_PANEL_SIZE / panelHeight);
                panelWidth = (int)(panelWidth * scale);
                panelHeight = (int)(panelHeight * scale);
                mapGridPanel._cellSize = (int)(cellSize * scale);
            }

            mapGridPanel.Size = new Size(panelWidth, panelHeight);
            mapGridPanel.Scale(new SizeF(currentZoom, currentZoom));

            gridPanels.Add(tabPage, mapGridPanel);
            viewportPanel.Controls.Add(mapGridPanel);
        }
        ScrollToFirstPanel();
    }
    private void ScrollToFirstPanel()
    {
        if (gridPanels.Any())
        {
            var firstPanel = gridPanels.Values.First();
            int scrollX = Math.Max(0, firstPanel.Location.X - (viewportPanel.ClientSize.Width - firstPanel.Width) / 2);
            int scrollY = Math.Max(0, firstPanel.Location.Y - (viewportPanel.ClientSize.Height - firstPanel.Height) / 2);
            viewportPanel.AutoScrollPosition = new Point(scrollX, scrollY);
        }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        DrawPortalConnections(e.Graphics);
    }
    private void OverviewPanel_Paint(object sender, PaintEventArgs e)
    {
        var panel = (MapGridOverviewPanel)sender;

        // Set up graphics for smooth rendering
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

        // Apply zoom transform
        e.Graphics.ScaleTransform(currentZoom, currentZoom);

        // Let the panel draw its content
        panel.DrawContent(e.Graphics);
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
    private void ViewportPanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
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
            viewportPanel.AutoScrollPosition = new Point(-viewportPanel.AutoScrollPosition.X + deltaX, -viewportPanel.AutoScrollPosition.Y + deltaY);
            lastMousePosition = e.Location;
        }
    }

    private void ViewportPanel_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        viewportPanel.Cursor = Cursors.Default;
    }

    private void ViewportPanel_MouseWheel(object sender, MouseEventArgs e)
    {
        if (ModifierKeys != Keys.Shift) return;
        float zoomDelta = e.Delta > 0 ? 1.1f : 0.9f;
        float newZoom = currentZoom * zoomDelta;
        newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, newZoom));
        if (newZoom != currentZoom)
        {
            Point mousePos = viewportPanel.PointToClient(Cursor.Position);
            Point scrollPos = new Point(-viewportPanel.AutoScrollPosition.X, -viewportPanel.AutoScrollPosition.Y);
            int newScrollX = (int)((mousePos.X + scrollPos.X) * (newZoom / currentZoom) - mousePos.X);
            int newScrollY = (int)((mousePos.Y + scrollPos.Y) * (newZoom / currentZoom) - mousePos.Y);
            currentZoom = newZoom;
            foreach (var panel in gridPanels.Values)
            {
                panel.Scale(new SizeF(currentZoom, currentZoom));
                panel.Invalidate();
            }
            viewportPanel.AutoScrollPosition = new Point(newScrollX, newScrollY);
            viewportPanel.Invalidate();
        }
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
