using System;
using System.Drawing;
using System.Windows.Forms;

public class ModernTextBox : TextBox
{
    public ModernTextBox()
    {
        this.BorderStyle = BorderStyle.FixedSingle;
        this.BackColor = Color.FromArgb(28, 28, 28);
        this.ForeColor = Color.White;
        this.Font = new Font("Segoe UI", 10);

        // Add some padding
        this.Padding = new Padding(5);
        this.Margin = new Padding(3);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        // Ensure colors are set after control creation
        this.BackColor = Color.FromArgb(28, 28, 28);
        this.ForeColor = Color.White;
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        // Maintain colors when focused
        this.BackColor = Color.FromArgb(28, 28, 28);
        this.ForeColor = Color.White;
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        // Maintain colors when focus is lost
        this.BackColor = Color.FromArgb(28, 28, 28);
        this.ForeColor = Color.White;
    }
}