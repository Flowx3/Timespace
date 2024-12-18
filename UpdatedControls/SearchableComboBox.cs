using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.ComponentModel;

public class SearchableComboBox : ComboBox
{
    // Constants for Windows messages  
    private const int WM_CTLCOLOREDIT = 0x0133;

    // Colors  
    private static readonly Color BackgroundColor = Color.FromArgb(28, 28, 28);
    private static readonly Color TextColor = Color.White;
    private static readonly Color BorderColor = Color.FromArgb(90, 90, 90);
    private static readonly Color SelectionColor = Color.FromArgb(45, 45, 45);
    private static readonly Color HighlightColor = Color.FromArgb(200, 200, 200);
    private static readonly Color HighlightTextColor = Color.FromArgb(120, 120, 120);

    // Cache graphics resources
    private readonly BufferedGraphicsContext bufferedGraphicsContext;
    private BufferedGraphics bufferedGraphics;
    private readonly StringFormat stringFormat;

    // Cache commonly used rectangles
    private Rectangle textRect;
    private Point[] arrowPoints;

    // Cache GDI objects
    private readonly SolidBrush backgroundBrush;
    private readonly SolidBrush textBrush;
    private readonly SolidBrush selectionBrush;
    private readonly SolidBrush highlightBrush;
    private readonly SolidBrush highlightTextBrush;
    private readonly Pen borderPen;

    public SearchableComboBox()
    {
        // Enable double buffering and optimize painting
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint, true);

        // Initialize graphics resources
        bufferedGraphicsContext = BufferedGraphicsManager.Current;
        stringFormat = new StringFormat(StringFormat.GenericDefault)
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Near,
            FormatFlags = StringFormatFlags.NoWrap
        };

        // Initialize brushes and pens
        backgroundBrush = new SolidBrush(BackgroundColor);
        textBrush = new SolidBrush(TextColor);
        selectionBrush = new SolidBrush(SelectionColor);
        highlightBrush = new SolidBrush(HighlightColor);
        highlightTextBrush = new SolidBrush(HighlightTextColor);
        borderPen = new Pen(BorderColor);

        // Set control properties
        DrawMode = DrawMode.OwnerDrawFixed;
        BackColor = BackgroundColor;
        ForeColor = TextColor;
        Font = SystemFonts.DefaultFont;
        ItemHeight = 20;
        DropDownStyle = ComboBoxStyle.DropDown;
        AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        AutoCompleteSource = AutoCompleteSource.ListItems;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateBufferedGraphics();
        if (!DesignMode)
        {
            ConfigureTextBox();
        }
    }

    private void ConfigureTextBox()
    {
        IntPtr textBoxHandle = FindWindowEx(Handle, IntPtr.Zero, "EDIT", null);
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

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateBufferedGraphics();
        UpdateCachedGeometry();
    }

    private void UpdateBufferedGraphics()
    {
        if (Width > 0 && Height > 0)
        {
            bufferedGraphics?.Dispose();
            bufferedGraphics = bufferedGraphicsContext.Allocate(CreateGraphics(), ClientRectangle);
            bufferedGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bufferedGraphics.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }
    }

    private void UpdateCachedGeometry()
    {
        // Cache text rectangle
        textRect = new Rectangle(2, 2, Width - 20, Height - 4);

        // Cache arrow points
        int arrowX = Width - 15;
        int arrowY = (Height - 5) / 2;
        arrowPoints = new Point[]
        {
            new Point(arrowX, arrowY),
            new Point(arrowX + 6, arrowY),
            new Point(arrowX + 3, arrowY + 5)
        };
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_CTLCOLOREDIT)
        {
            IntPtr hdc = m.WParam;
            SetBkColor(hdc, ColorTranslator.ToWin32(BackgroundColor));
            SetTextColor(hdc, ColorTranslator.ToWin32(TextColor));
            m.Result = CreateSolidBrush(ColorTranslator.ToWin32(BackgroundColor));
            return;
        }
        base.WndProc(ref m);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = bufferedGraphics.Graphics;
        g.Clear(BackgroundColor);

        // Draw text
        if (!string.IsNullOrEmpty(Text))
        {
            g.DrawString(Text, Font, textBrush, textRect, stringFormat);
        }

        // Draw arrow
        g.FillPolygon(textBrush, arrowPoints);

        // Draw border
        g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

        // Render the buffered graphics
        bufferedGraphics.Render(e.Graphics);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= Items.Count) return;

        var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        var brush = selected ? highlightBrush : backgroundBrush;
        var textBrushToUse = selected ? highlightTextBrush : textBrush;

        e.Graphics.FillRectangle(brush, e.Bounds);

        var itemText = GetItemText(Items[e.Index]);
        var textBounds = new Rectangle(
            e.Bounds.X + 3,
            e.Bounds.Y,
            e.Bounds.Width - 6,
            e.Bounds.Height
        );

        e.Graphics.DrawString(itemText, Font, textBrushToUse, textBounds, stringFormat);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            stringFormat?.Dispose();
            backgroundBrush?.Dispose();
            textBrush?.Dispose();
            selectionBrush?.Dispose();
            highlightBrush?.Dispose();
            highlightTextBrush?.Dispose();
            borderPen?.Dispose();
            bufferedGraphics?.Dispose();
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