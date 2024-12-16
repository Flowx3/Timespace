using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class ModernButton : Control
{
    private ButtonState _state = ButtonState.Normal;
    private bool _isHovered = false;
    private BufferedGraphicsContext _bufferedGraphicsContext;
    private BufferedGraphics _bufferedGraphics;

    public ButtonState State
    {
        get => _state;
        set
        {
            _state = value;
            this.Invalidate();
        }
    }

    public ModernButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint, true);
        this.Font = new Font("Segoe UI", 9f);
        _bufferedGraphicsContext = BufferedGraphicsManager.Current;
        this.Size = new Size(100, 30); // Default size for visibility  
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (_bufferedGraphics == null || _bufferedGraphics.Graphics == null)
        {
            _bufferedGraphics = _bufferedGraphicsContext.Allocate(g, this.ClientRectangle);
        }

        var bg = _bufferedGraphics.Graphics;
        bg.Clear(this.BackColor);
        bg.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        Color backgroundColor;
        Color textColor;

        switch (State)
        {
            case ButtonState.Selected:
                backgroundColor = Color.FromArgb(0, 120, 215);
                textColor = Color.White;
                break;
            case ButtonState.Current:
                backgroundColor = Color.FromArgb(0, 150, 136);
                textColor = Color.White;
                break;
            case ButtonState.Disabled:
                backgroundColor = Color.FromArgb(60, 60, 60);
                textColor = Color.Gray;
                break;
            default:
                backgroundColor = _isHovered
                    ? Color.FromArgb(45, 45, 45)
                    : Color.FromArgb(35, 35, 35);
                textColor = Color.FromArgb(200, 200, 200);
                break;
        }

        using (var brush = new SolidBrush(backgroundColor))
        {
            bg.FillRoundedRectangle(brush, rect, 4);
        }

        if (!string.IsNullOrEmpty(Text))
        {
            using (var brush = new SolidBrush(textColor))
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                bg.DrawString(Text, Font, brush, rect, stringFormat);
            }
        }

        _bufferedGraphics.Render(g);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        this.Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        this.Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bufferedGraphics?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public enum ButtonState
{
    Normal,
    Selected,
    Current,
    Disabled
}

public static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
    {
        using (GraphicsPath path = CreateRoundedRectangle(rect, radius))
        {
            g.FillPath(brush, path);
        }
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}