using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class SearchableComboBox : ComboBox
{
    // Constants for Windows messages  
    private const int WM_CTLCOLOREDIT = 0x0133;

    // Colors  
    private static readonly Color BackgroundColor = Color.FromArgb(28, 28, 28);
    private static readonly Color TextColor = Color.White;
    private static readonly Color BorderColor = Color.FromArgb(90, 90, 90);
    private static readonly Color SelectionColor = Color.FromArgb(45, 45, 45);
    private static readonly Color HighlightColor = Color.FromArgb(200, 200, 200); // Dimmed highlight color  
    private static readonly Color HighlightTextColor = Color.FromArgb(120, 120, 120); // Dimmed highlight text color  

    // Cache brushes and pens for better performance  
    private static readonly SolidBrush BackgroundBrush;
    private static readonly SolidBrush TextBrush;
    private static readonly SolidBrush SelectionBrush;
    private static readonly SolidBrush HighlightBrush;
    private static readonly SolidBrush HighlightTextBrush;
    private static readonly Pen BorderPen;

    // Static constructor to initialize brushes  
    static SearchableComboBox()
    {
        BackgroundBrush = new SolidBrush(BackgroundColor);
        TextBrush = new SolidBrush(TextColor);
        SelectionBrush = new SolidBrush(SelectionColor);
        HighlightBrush = new SolidBrush(HighlightColor);
        HighlightTextBrush = new SolidBrush(HighlightTextColor);
        BorderPen = new Pen(BorderColor);
    }

    public SearchableComboBox()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint, true);

        DrawMode = DrawMode.OwnerDrawFixed;
        BackColor = BackgroundColor;
        ForeColor = TextColor;
        Font = new Font("Segoe UI", 9F);
        ItemHeight = 20;
        DropDownStyle = ComboBoxStyle.DropDown;
        AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        AutoCompleteSource = AutoCompleteSource.ListItems;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (!DesignMode)
        {
            // Subclass the textbox to prevent default Windows selection behavior  
            IntPtr textBoxHandle = FindTextBoxHandle();
            if (textBoxHandle != IntPtr.Zero)
            {
                var textbox = (TextBox)TextBox.FromHandle(textBoxHandle);
                if (textbox != null)
                {
                    textbox.BackColor = BackgroundColor;
                    textbox.ForeColor = TextColor;
                    textbox.BorderStyle = BorderStyle.None;
                }
            }
        }
    }

    private IntPtr FindTextBoxHandle()
    {
        return FindWindowEx(Handle, IntPtr.Zero, "EDIT", null);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_CTLCOLOREDIT)
        {
            // Set colors for the textbox  
            IntPtr hdc = m.WParam;
            SetBkColor(hdc, ColorTranslator.ToWin32(BackgroundColor));
            SetTextColor(hdc, ColorTranslator.ToWin32(TextColor));

            // Return background brush handle  
            m.Result = CreateSolidBrush(ColorTranslator.ToWin32(BackgroundColor));
            return;
        }
        base.WndProc(ref m);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var rect = ClientRectangle;

        // Draw background  
        g.FillRectangle(BackgroundBrush, rect);

        // Draw text  
        if (!string.IsNullOrEmpty(Text))
        {
            var textRect = new Rectangle(2, 2, Width - 20, Height - 4);
            TextRenderer.DrawText(g, Text, Font, textRect, TextColor, BackgroundColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        // Draw arrow  
        var arrowX = Width - 15;
        var arrowY = (Height - 5) / 2;
        var arrowPoints = new Point[]
        {
            new Point(arrowX, arrowY),
            new Point(arrowX + 6, arrowY),
            new Point(arrowX + 3, arrowY + 5)
        };
        g.FillPolygon(TextBrush, arrowPoints);

        // Draw border  
        g.DrawRectangle(BorderPen, 0, 0, Width - 1, Height - 1);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        // Draw the background  
        var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        e.Graphics.FillRectangle(selected ? HighlightBrush : BackgroundBrush, e.Bounds);

        // Draw the item text  
        if (e.Index < Items.Count)
        {
            var textRect = new Rectangle(
                e.Bounds.X + 3,
                e.Bounds.Y + 2,
                e.Bounds.Width - 6,
                e.Bounds.Height - 4
            );
            TextRenderer.DrawText(e.Graphics,
                GetItemText(Items[e.Index]),
                Font,
                textRect,
                selected ? HighlightTextColor : TextColor,
                selected ? HighlightColor : BackgroundColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // No need to dispose static brushes as they're shared  
            // They'll be disposed when the application exits  
        }
        base.Dispose(disposing);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateSolidBrush(int color);

    [DllImport("gdi32.dll")]
    private static extern int SetBkColor(IntPtr hdc, int crColor);

    [DllImport("gdi32.dll")]
    private static extern int SetTextColor(IntPtr hdc, int crColor);
}