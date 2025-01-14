using TimeSpace;
using TimeSpace.MapgridPanel;

public class MapGridOverviewPanel : Panel
{
    private readonly MapGridPanel originalPanel;
    private readonly float scaleFactor;
    private Portal highlightedPortal;

    public event EventHandler<PortalClickEventArgs> PortalClicked;

    public MapGridOverviewPanel(MapGridPanel original, float scale)
    {
        originalPanel = original;
        scaleFactor = scale;

        Width = (int)(original.Width * scale);
        Height = (int)(original.Height * scale);
        BorderStyle = BorderStyle.FixedSingle;

        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

        MouseClick += MapGridOverviewPanel_MouseClick;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(BackColor);
        e.Graphics.ScaleTransform(scaleFactor, scaleFactor);

        // Draw the grid
        int cellSize = originalPanel._cellSize;
        byte[] grid = originalPanel._grid;
        int width = originalPanel._width;
        int height = originalPanel._height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var color = GetColor(grid[y * width + x]);
                using var brush = new SolidBrush(color);
                e.Graphics.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);

                using var pen = new Pen(Color.Black);
                e.Graphics.DrawRectangle(pen, x * cellSize, y * cellSize, cellSize, cellSize);
            }
        }

        // Draw portal connections
        DrawPortalConnections(e.Graphics);

        // Draw highlighted portal
        if (highlightedPortal != null)
        {
            using var highlightBrush = new SolidBrush(Color.FromArgb(128, Color.Yellow));
            e.Graphics.FillRectangle(highlightBrush,
                highlightedPortal.FromX.Value * cellSize,
                highlightedPortal.FromY.Value * cellSize,
                cellSize, cellSize);
        }
    }

    private void DrawPortalConnections(Graphics g)
    {
        var tabPage = (CustomTabPage)Tag;
        foreach (var portal in tabPage.Portals)
        {
            if (portal.MapTo != null && portal.ToX.HasValue && portal.ToY.HasValue)
            {
                // Draw an arrow or line indicating the connection
                using var pen = new Pen(Color.Yellow, 2);
                int cellSize = originalPanel._cellSize;
                Point start = new Point(
                    portal.FromX.Value * cellSize + cellSize / 2,
                    portal.FromY.Value * cellSize + cellSize / 2
                );

                // If connected to a portal in the same map
                if (portal.MapTo == tabPage.MapName)
                {
                    Point end = new Point(
                        portal.ToX.Value * cellSize + cellSize / 2,
                        portal.ToY.Value * cellSize + cellSize / 2
                    );
                    g.DrawLine(pen, start, end);
                }
                else
                {
                    // Draw an arrow pointing to the edge of the map
                    int arrowLength = cellSize;
                    double angle = portal.MinimapOrientation switch
                    {
                        "North" => -Math.PI / 2,
                        "South" => Math.PI / 2,
                        "East" => 0,
                        "West" => Math.PI,
                        _ => 0
                    };

                    Point end = new Point(
                        start.X + (int)(Math.Cos(angle) * arrowLength),
                        start.Y + (int)(Math.Sin(angle) * arrowLength)
                    );
                    g.DrawLine(pen, start, end);
                }
            }
        }
    }

    private void MapGridOverviewPanel_MouseClick(object sender, MouseEventArgs e)
    {
        var tabPage = (CustomTabPage)Tag;
        int cellSize = (int)(originalPanel._cellSize * scaleFactor);
        int clickedX = e.X / cellSize;
        int clickedY = e.Y / cellSize;

        var clickedPortal = tabPage.Portals.FirstOrDefault(p =>
            p.FromX == clickedX && p.FromY == clickedY);

        if (clickedPortal != null)
        {
            PortalClicked?.Invoke(this, new PortalClickEventArgs(clickedPortal, this));
        }
    }

    public void HighlightPortal(Portal portal)
    {
        highlightedPortal = portal;
        Invalidate();
    }

    public void ClearPortalHighlight()
    {
        highlightedPortal = null;
        Invalidate();
    }

    private Color GetColor(byte value) => originalPanel._flagColors.TryGetValue(value, out var color) ? color : Color.White;
}