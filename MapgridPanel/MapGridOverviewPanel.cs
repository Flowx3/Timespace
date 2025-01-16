using TimeSpace;
using TimeSpace.MapgridPanel;

public class MapGridOverviewPanel : Panel
{
    private readonly MapGridPanel originalPanel;
    private readonly float scaleFactor;
    private Portal highlightedPortal;
    private Bitmap cacheBitmap;
    private readonly CustomTabPage _tabPage;
    public event EventHandler<PortalClickEventArgs> PortalClicked;

    public MapGridOverviewPanel(MapGridPanel original, float scale, CustomTabPage tab) : base()
    {
        this.SetStyle(ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        originalPanel = original;
        scaleFactor = scale;
        _tabPage = tab;
        BorderStyle = BorderStyle.FixedSingle;
        BackColor = Color.FromArgb(30, 30, 30);

        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

        MouseClick += MapGridOverviewPanel_MouseClick;

        // Create the cached bitmap
        UpdateCache();
    }
    protected override void OnScroll(ScrollEventArgs se)
    {
        this.Invalidate();

        base.OnScroll(se);
    }
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000; // WS_CLIPCHILDREN
            return cp;
        }
    }
    public void UpdateCache()
    {
        if (Width <= 0 || Height <= 0) return;

        cacheBitmap?.Dispose();
        cacheBitmap = new Bitmap(Width, Height);

        using (var g = Graphics.FromImage(cacheBitmap))
        {
            g.Clear(BackColor);
            if (scaleFactor > 0)
            {
                g.ScaleTransform(scaleFactor, scaleFactor);
            }
            DrawContent(g);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (cacheBitmap != null)
        {
            e.Graphics.DrawImage(cacheBitmap, 0, 0);
        }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateCache();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            cacheBitmap?.Dispose();
        }
        base.Dispose(disposing);
    }
    public void DrawContent(Graphics g)
    {
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
                g.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);

                using var pen = new Pen(Color.Black);
                g.DrawRectangle(pen, x * cellSize, y * cellSize, cellSize, cellSize);
            }
        }

        // Draw portal connections
        DrawPortalConnections(g);

        // Draw highlighted portal
        if (highlightedPortal != null)
        {
            using var highlightBrush = new SolidBrush(Color.FromArgb(128, Color.Yellow));
            g.FillRectangle(highlightBrush,
                highlightedPortal.FromX.Value * cellSize,
                highlightedPortal.FromY.Value * cellSize,
                cellSize, cellSize);
        }
    }
    private void DrawPortalConnections(Graphics g)
    {
        foreach (var portal in _tabPage.Portals)
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
                if (portal.MapTo == _tabPage.MapName)
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
        var parentForm = FindForm() as MapOverviewForm;
        if (parentForm == null) return;

        // Now we can access currentZoom
        float zoom = parentForm.currentZoom;

        // Adjust for zoom level when calculating clicked cell
        int cellSize = (int)(originalPanel._cellSize * zoom);
        int clickedX = e.X / cellSize;
        int clickedY = e.Y / cellSize;

        var clickedPortal = _tabPage.Portals.FirstOrDefault(p =>
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
    private Color GetColor(byte value) =>
    originalPanel._flagColors.TryGetValue(value, out var color) ? color : Color.White;
}