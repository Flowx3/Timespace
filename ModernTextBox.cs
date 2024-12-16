using System;
using System.Drawing;
using System.Windows.Forms;

public class ModernTextBox : TextBox
{
    private Color borderColor = Color.FromArgb(70, 70, 70);
    private Color backgroundColor = Color.FromArgb(28, 28, 28);
    private Color textColor = Color.White;
    private int borderRadius = 5;

    public ModernTextBox()
    {
        this.BorderStyle = BorderStyle.None;
        this.BackColor = backgroundColor;
        this.ForeColor = textColor;
        this.Font = new Font("Segoe UI", 10);
        this.Padding = new Padding(10, 7, 10, 7);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Draw background
        using (var brush = new SolidBrush(backgroundColor))
        {
            g.FillRectangle(brush, this.ClientRectangle);
        }

        // Draw border
        using (var pen = new Pen(borderColor, 2))
        {
            var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            var path = GetRoundedRectPath(rect, borderRadius);
            g.DrawPath(pen, path);
        }
    }

    private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        this.Invalidate();
    }
}
