using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TimeSpace;

public class MapOverviewForm : Form
{
    private const int PADDING = 40;
    private const int GRID_SIZE = 11;
    private const int BASE_CELL_SIZE = 40; // Matched to GridSelector
    private const float MIN_ZOOM = 0.1f;
    private const float MAX_ZOOM = 5.0f;
    private Dictionary<string, MapGridOverviewPanel> gridPanels;
    private Panel viewportPanel;
    private float currentZoom = 1.0f;
    private Point lastMousePosition;
    private bool isDragging;
    private Portal selectedPortal;
    private CustomTabPage selectedTabPage;
    private bool isConnectingPortals;

    public MapOverviewForm(TabControl mapTabControl)
    {
        InitializeComponent();
        InitializeGridPanels(mapTabControl);
        PositionPanelsInGrid();

        if (gridPanels.Any())
        {
            CenterViewport();
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

        // Create viewport panel with double buffering
        viewportPanel = new DoubleBufferedPanel
        {
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(40, 40, 40)
        };

        viewportPanel.MouseDown += ViewportPanel_MouseDown;
        viewportPanel.MouseMove += ViewportPanel_MouseMove;
        viewportPanel.MouseUp += ViewportPanel_MouseUp;
        viewportPanel.MouseWheel += ViewportPanel_MouseWheel;

        Controls.Add(viewportPanel);
        gridPanels = new Dictionary<string, MapGridOverviewPanel>();
    }

    private void DrawArrowLine(Graphics g, Pen pen, Point start, Point end)
    {
        g.DrawLine(pen, start, end);

        float arrowSize = 10f * currentZoom;
        float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

        PointF[] arrowHead = new PointF[] {
                end,
                new PointF(
                    end.X - arrowSize * (float)Math.Cos(angle - Math.PI / 6),
                    end.Y - arrowSize * (float)Math.Sin(angle - Math.PI / 6)
                ),
                new PointF(
                    end.X - arrowSize * (float)Math.Cos(angle + Math.PI / 6),
                    end.Y - arrowSize * (float)Math.Sin(angle + Math.PI / 6)
                )
            };

        g.FillPolygon(new SolidBrush(pen.Color), arrowHead);
    }
    private void PositionPanelsInGrid()
    {
        var positioned = new HashSet<string>();
        var coordinates = new Dictionary<string, Point>();

        if (!gridPanels.Any()) return;

        // Start with first panel at (0,0)
        var firstPanel = gridPanels.First();
        coordinates[firstPanel.Key] = new Point(0, 0);
        positioned.Add(firstPanel.Key);

        // Position connected panels
        PositionConnectedPanels(firstPanel.Key, coordinates, positioned);

        // Calculate grid bounds
        int minX = coordinates.Values.Min(p => p.X);
        int minY = coordinates.Values.Min(p => p.Y);
        int maxX = coordinates.Values.Max(p => p.X);
        int maxY = coordinates.Values.Max(p => p.Y);

        // Position panels based on coordinates
        int panelSpacing = GRID_SIZE * BASE_CELL_SIZE + PADDING;
        foreach (var kvp in gridPanels)
        {
            if (coordinates.TryGetValue(kvp.Key, out Point pos))
            {
                kvp.Value.Location = new Point(
                    (pos.X - minX) * panelSpacing + PADDING,
                    (pos.Y - minY) * panelSpacing + PADDING
                );
            }
        }

        // Set viewport size
        viewportPanel.AutoScrollMinSize = new Size(
            (maxX - minX + 1) * panelSpacing + PADDING * 3,
            (maxY - minY + 1) * panelSpacing + PADDING * 3
        );
    }
    private void CenterViewport()
    {
        // Calculate the total bounds of all panels
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (var panel in gridPanels.Values)
        {
            minX = Math.Min(minX, panel.Left);
            minY = Math.Min(minY, panel.Top);
            maxX = Math.Max(maxX, panel.Right);
            maxY = Math.Max(maxY, panel.Bottom);
        }

        // Calculate center point
        int centerX = (maxX + minX) / 2;
        int centerY = (maxY + minY) / 2;

        // Set scroll position to center
        viewportPanel.AutoScrollPosition = new Point(
            centerX - viewportPanel.Width / 2,
            centerY - viewportPanel.Height / 2
        );
    }
    private void InitializeGridPanels(TabControl mapTabControl)
    {
        foreach (CustomTabPage tabPage in mapTabControl.Controls.OfType<CustomTabPage>())
        {
            if (tabPage._mapGridPanel == null) continue;

            var overviewPanel = new MapGridOverviewPanel(tabPage._mapGridPanel, 1.0f)
            {
                Tag = tabPage,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30),
                Size = new Size(GRID_SIZE * BASE_CELL_SIZE, GRID_SIZE * BASE_CELL_SIZE)
            };

            var nameLabel = new Label
            {
                Text = tabPage.Text,
                AutoSize = true,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Location = new Point(0, -20)
            };
            overviewPanel.Controls.Add(nameLabel);

            gridPanels.Add(tabPage.Text, overviewPanel);
            viewportPanel.Controls.Add(overviewPanel);
        }
    }

    private void LayoutGridPanels()
    {
        if (!gridPanels.Any()) return;

        var positioned = new HashSet<string>();
        var coordinates = new Dictionary<string, Point>();
        var centralPanel = gridPanels.First();

        coordinates[centralPanel.Key] = new Point(0, 0);
        positioned.Add(centralPanel.Key);

        PositionConnectedPanels(centralPanel.Key, coordinates, positioned);

        int minX = coordinates.Values.Min(p => p.X);
        int minY = coordinates.Values.Min(p => p.Y);
        int maxX = coordinates.Values.Max(p => p.X);
        int maxY = coordinates.Values.Max(p => p.Y);

        // Calculate spacing between panels
        int spacing = BASE_CELL_SIZE * GRID_SIZE + 40; // Added more space between panels

        // Position panels and adjust viewport
        foreach (var kvp in gridPanels)
        {
            if (coordinates.TryGetValue(kvp.Key, out Point pos))
            {
                kvp.Value.Location = new Point(
                    (pos.X - minX) * spacing + PADDING,
                    (pos.Y - minY) * spacing + PADDING
                );
            }
        }

        // Set minimum size for scrolling
        viewportPanel.AutoScrollMinSize = new Size(
            (maxX - minX + 1) * spacing + PADDING * 2,
            (maxY - minY + 1) * spacing + PADDING * 2
        );
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
            var sourceTabPage = (CustomTabPage)sourcePanel.Tag;
            foreach (var portal in sourceTabPage.Portals)
            {
                if (string.IsNullOrEmpty(portal.MapTo) || !gridPanels.TryGetValue(portal.MapTo, out var targetPanel))
                    continue;

                Point startPoint = GetPortalConnectionPoint(sourcePanel, portal);

                // Get the target tab page
                var targetTabPage = (CustomTabPage)targetPanel.Tag;

                // Find the matching portal in the target map
                var targetPortal = targetTabPage.Portals.FirstOrDefault(p =>
                    p.MapTo == sourceTabPage.Text &&
                    p.ToX == portal.FromX &&
                    p.ToY == portal.FromY);

                if (targetPortal == null) continue;

                Point endPoint = GetPortalConnectionPoint(targetPanel, targetPortal);
                DrawArrowLine(g, pen, startPoint, endPoint);
            }
        }
    }
    private Point GetPortalPoint(MapGridOverviewPanel panel, Portal portal)
    {
        return new Point(
            (int)((portal.FromX.Value * BASE_CELL_SIZE + BASE_CELL_SIZE / 2) * currentZoom),
            (int)((portal.FromY.Value * BASE_CELL_SIZE + BASE_CELL_SIZE / 2) * currentZoom)
        );
    }
    private Point GetPortalConnectionPoint(MapGridOverviewPanel panel, Portal portal)
    {
        int x = panel.Location.X + (int)(portal.FromX.Value * BASE_CELL_SIZE * currentZoom) + (int)(BASE_CELL_SIZE * currentZoom / 2);
        int y = panel.Location.Y + (int)(portal.FromY.Value * BASE_CELL_SIZE * currentZoom) + (int)(BASE_CELL_SIZE * currentZoom / 2);
        return new Point(x, y);
    }
    private void ViewportPanel_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0, 150, 136), 2);

        foreach (var panelKvp in gridPanels)
        {
            var panel = panelKvp.Value;
            var tabPage = (CustomTabPage)panel.Tag;

            foreach (var portal in tabPage.Portals)
            {
                if (string.IsNullOrEmpty(portal.MapTo) || !gridPanels.TryGetValue(portal.MapTo, out var targetPanel))
                    continue;

                var targetTabPage = (CustomTabPage)targetPanel.Tag;
                var targetPortal = targetTabPage.Portals.FirstOrDefault(p =>
                    p.MapTo == tabPage.Text && p.ToX == portal.FromX && p.ToY == portal.FromY);

                if (targetPortal == null) continue;

                // Calculate connection points in panel coordinates
                Point startPoint = GetPortalPoint(panel, portal);
                Point endPoint = GetPortalPoint(targetPanel, targetPortal);

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
            int deltaX = e.X - lastMousePosition.X;
            int deltaY = e.Y - lastMousePosition.Y;

            viewportPanel.AutoScrollPosition = new Point(
                -viewportPanel.AutoScrollPosition.X - deltaX,
                -viewportPanel.AutoScrollPosition.Y - deltaY
            );

            lastMousePosition = e.Location;
        }
    }
    private void ViewportPanel_MouseWheel(object sender, MouseEventArgs e)
    {
        // Get mouse position relative to viewport
        Point mousePos = viewportPanel.PointToClient(Cursor.Position);

        // Calculate zoom factor
        float zoomDelta = (e.Delta > 0) ? 1.1f : 0.9f;
        float newZoom = currentZoom * zoomDelta;
        newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, newZoom));

        if (newZoom != currentZoom)
        {
            // Calculate zoom center point
            Point scrollPos = new Point(
                -viewportPanel.AutoScrollPosition.X,
                -viewportPanel.AutoScrollPosition.Y
            );

            // Apply zoom
            foreach (var panel in gridPanels.Values)
            {
                // Store original position relative to mouse
                float relativeX = (panel.Left - scrollPos.X - mousePos.X) / currentZoom;
                float relativeY = (panel.Top - scrollPos.Y - mousePos.Y) / currentZoom;

                // Calculate new position
                panel.Left = (int)(mousePos.X + relativeX * newZoom) + scrollPos.X;
                panel.Top = (int)(mousePos.Y + relativeY * newZoom) + scrollPos.Y;

                // Apply scale transform
                panel.Scale(new SizeF(newZoom / currentZoom, newZoom / currentZoom));
            }

            currentZoom = newZoom;
            viewportPanel.Refresh();
        }
    }
    private void ViewportPanel_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        viewportPanel.Cursor = Cursors.Default;
    }
    private class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);
        }
    }

    private void PositionConnectedPanels(string currentMapId, Dictionary<string, Point> coordinates, HashSet<string> positioned)
    {
        var tabPage = (CustomTabPage)gridPanels[currentMapId].Tag;

        foreach (var portal in tabPage.Portals)
        {
            if (string.IsNullOrEmpty(portal.MapTo) || !gridPanels.ContainsKey(portal.MapTo) || positioned.Contains(portal.MapTo))
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

    private void OverviewPanel_PortalClicked(object sender, PortalClickEventArgs e)
    {
        if (!isConnectingPortals) return;

        var clickedPanel = (MapGridOverviewPanel)sender;
        var clickedTabPage = (CustomTabPage)clickedPanel.Tag;

        if (selectedPortal == null)
        {
            selectedPortal = e.Portal;
            selectedTabPage = clickedTabPage;
            clickedPanel.HighlightPortal(selectedPortal);
        }
        else
        {
            if (selectedTabPage != clickedTabPage)
            {
                ConnectPortals(selectedTabPage, selectedPortal, clickedTabPage, e.Portal);
            }

            foreach (var panel in gridPanels.Values)
            {
                panel.ClearPortalHighlight();
            }
            selectedPortal = null;
            selectedTabPage = null;
            Refresh(); // Redraw connections
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
