using System;
using System.Drawing;
using System.Windows.Forms;

public class ModernTabControl : TabControl
{
    private Color selectedTabColor = Color.FromArgb(45, 45, 45);
    private Color unselectedTabColor = Color.FromArgb(28, 28, 28);
    private Color headerColor = Color.FromArgb(60, 60, 60);

    public ModernTabControl()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true
        );

        ItemSize = new Size(75, 25);
        SizeMode = TabSizeMode.Fixed;

        // Remove white border and set background
        this.Padding = new Point(0, 0);
        this.BackColor = unselectedTabColor;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(unselectedTabColor);

        // Draw each tab
        for (int i = 0; i < TabCount; i++)
        {
            var tabRect = GetTabRect(i);

            // Fill background
            using (var brush = new SolidBrush(i == SelectedIndex ? selectedTabColor : unselectedTabColor))
            {
                g.FillRectangle(brush, tabRect);
            }

            // Draw the header line for selected tab
            if (i == SelectedIndex)
            {
                using (var brush = new SolidBrush(headerColor))
                {
                    g.FillRectangle(brush, tabRect.X, 0, tabRect.Width, 3);
                }
            }

            // Draw tab text
            string tabText = TabPages[i].Text;
            using (var brush = new SolidBrush(Color.White))
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(tabText, Font, brush, tabRect, stringFormat);
            }
        }
    }
}