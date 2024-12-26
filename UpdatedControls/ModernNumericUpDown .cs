using System;
using System.Drawing;
using System.Windows.Forms;

public class ModernNumericUpDown : UserControl
{
    private NumericUpDown numericUpDown;

    public decimal Value
    {
        get => numericUpDown.Value;
        set => numericUpDown.Value = value;
    }
    public decimal Minimum
    {
        get => numericUpDown.Minimum;
        set => numericUpDown.Minimum = value;
    }
    public decimal Maximum
    {
        get => numericUpDown.Maximum;
        set => numericUpDown.Maximum = value;
    }
    public decimal Increment
    {
        get => numericUpDown.Increment;
        set => numericUpDown.Increment = value;
    }

    // Define the custom ValueChanged event  
    public event EventHandler ValueChanged;

    public ModernNumericUpDown()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Set control styles  
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.White;

        // Initialize NumericUpDown  
        numericUpDown = new NumericUpDown
        {
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = BackColor,
            ForeColor = ForeColor,
            Font = new Font("Segoe UI", 9F),
            Increment = 1,
            TextAlign = HorizontalAlignment.Right,
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        // Subscribe to the internal NumericUpDown's ValueChanged event  
        numericUpDown.ValueChanged += NumericUpDown_ValueChanged;

        // Customize up/down buttons  
        numericUpDown.UpDownAlign = LeftRightAlignment.Left;
        numericUpDown.Width = Width;
        numericUpDown.Height = Height;

        // Style the up/down buttons  
        numericUpDown.Controls[0].Paint += UpDownButtons_Paint;
        Controls.Add(numericUpDown);

        // Handle resizing  
        Resize += ModernNumericUpDown_Resize;
    }

    private void NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        // Raise the custom ValueChanged event  
        ValueChanged?.Invoke(this, e);
    }

    private void UpDownButtons_Paint(object sender, PaintEventArgs e)
    {
        var buttons = (Control)sender;
        var rect = buttons.ClientRectangle;

        // Draw custom background for buttons  
        using (var backgroundBrush = new SolidBrush(BackColor))
        {
            e.Graphics.FillRectangle(backgroundBrush, rect);
        }

        // Draw custom up arrow  
        var arrowColor = Color.White;
        var arrowSize = new Size(7, 4);
        var arrowUpPoint = new Point(rect.Width / 2 - arrowSize.Width / 2, rect.Height / 4 - arrowSize.Height / 2);
        using (var arrowBrush = new SolidBrush(arrowColor))
        {
            Point[] upArrow = {
                new Point(arrowUpPoint.X, arrowUpPoint.Y + arrowSize.Height),
                new Point(arrowUpPoint.X + arrowSize.Width / 2, arrowUpPoint.Y),
                new Point(arrowUpPoint.X + arrowSize.Width, arrowUpPoint.Y + arrowSize.Height)
            };
            e.Graphics.FillPolygon(arrowBrush, upArrow);
        }

        // Draw custom down arrow  
        var arrowDownPoint = new Point(rect.Width / 2 - arrowSize.Width / 2, rect.Height * 3 / 4 - arrowSize.Height / 2);
        using (var arrowBrush = new SolidBrush(arrowColor))
        {
            Point[] downArrow = {
                new Point(arrowDownPoint.X, arrowDownPoint.Y),
                new Point(arrowDownPoint.X + arrowSize.Width / 2, arrowDownPoint.Y + arrowSize.Height),
                new Point(arrowDownPoint.X + arrowSize.Width, arrowDownPoint.Y)
            };
            e.Graphics.FillPolygon(arrowBrush, downArrow);
        }
    }

    private void ModernNumericUpDown_Resize(object sender, EventArgs e)
    {
        numericUpDown.Width = Width;
        numericUpDown.Height = Height;
    }
}