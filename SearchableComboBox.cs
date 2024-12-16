using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class SearchableComboBox : ComboBox
{
    private Color borderColor = Color.FromArgb(70, 70, 70);
    private Color backgroundColor = Color.FromArgb(28, 28, 28);
    private Color textColor = Color.White;
    private Color selectionColor = Color.FromArgb(64, 64, 64); // Light gray for selection
    private Color dropdownBorderColor = Color.FromArgb(70, 70, 70);
    private Color dropdownBackgroundColor = Color.FromArgb(28, 28, 28);
    private int borderRadius = 5;

    public SearchableComboBox()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.BackColor = backgroundColor;
        this.ForeColor = textColor;
        this.Font = new Font("Segoe UI", 10);
        this.DropDownStyle = ComboBoxStyle.DropDown;
        this.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        this.AutoCompleteSource = AutoCompleteSource.ListItems;

        // Set up custom drawing
        this.DrawMode = DrawMode.OwnerDrawFixed;
        this.DrawItem += SearchableComboBox_DrawItem;
    }

    private void SearchableComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        // Get the bounds for the entire item
        Rectangle bounds = e.Bounds;

        // Draw the background
        using (SolidBrush brush = new SolidBrush(
            (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? selectionColor
                : dropdownBackgroundColor))
        {
            e.Graphics.FillRectangle(brush, bounds);
        }

        // Draw the item text
        using (SolidBrush brush = new SolidBrush(textColor))
        {
            string itemText = this.Items[e.Index].ToString();
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };

            // Add some padding to the text
            Rectangle textBounds = new Rectangle(
                bounds.X + 5,
                bounds.Y,
                bounds.Width - 10,
                bounds.Height
            );

            e.Graphics.DrawString(itemText, this.Font, brush, textBounds, format);
        }

        // Draw focus rectangle if focused
        if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
        {
            using (Pen pen = new Pen(dropdownBorderColor))
            {
                e.Graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }
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

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        // After the dropdown is shown
        if (m.Msg == 0x0118) // WM_SHOWPOPUP
        {
            IntPtr dropdownHandle = FindWindowEx(this.Handle, IntPtr.Zero, "ComboLBox", null);
            if (dropdownHandle != IntPtr.Zero)
            {
                using (Graphics g = Graphics.FromHwnd(dropdownHandle))
                {
                    // Draw border around dropdown
                    Rectangle bounds = new Rectangle(0, 0, this.DropDownWidth, this.DropDownHeight);
                    using (Pen pen = new Pen(dropdownBorderColor))
                    {
                        g.DrawRectangle(pen, 0, 0, bounds.Width - 1, bounds.Height - 1);
                    }
                }
            }
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

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
}
