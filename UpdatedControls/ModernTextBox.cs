public class ModernTextBox : TextBox
{
    private readonly Color defaultBackColor = Color.FromArgb(28, 28, 28);
    private readonly Color defaultForeColor = Color.White;
    private readonly Font defaultFont;
    private bool isCustomFont = false;
    private bool isCustomBackColor = false;

    public ModernTextBox()
    {
        defaultFont = new Font("Segoe UI", 10);
        this.BorderStyle = BorderStyle.FixedSingle;
        this.BackColor = defaultBackColor;
        this.ForeColor = defaultForeColor;
        this.Font = defaultFont;
        this.Padding = new Padding(5);
        this.Margin = new Padding(3);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        if (!isCustomBackColor && this.BackColor != defaultBackColor)
        {
            isCustomBackColor = true;
        }

        if (!isCustomBackColor && this.BackColor != defaultBackColor)
        {
            this.BackColor = defaultBackColor;
            return;
        }
        base.OnBackColorChanged(e);
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        if (this.ForeColor != defaultForeColor)
        {
            this.ForeColor = defaultForeColor;
            return;
        }
        base.OnForeColorChanged(e);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        if (!isCustomFont && this.Font != defaultFont)
        {
            isCustomFont = true;
        }

        if (!isCustomFont && this.Font != defaultFont)
        {
            this.Font = defaultFont;
            return;
        }
        base.OnFontChanged(e);
    }
}