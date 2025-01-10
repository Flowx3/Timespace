//I need this shit becuase of fucking Materialskin.2 <- shit does not fw customization whatsoever
public static class ControlStyleManager
{
    private static Dictionary<Control, (Color BackColor, Color ForeColor, Font Font)> _originalStyles
        = new Dictionary<Control, (Color, Color, Font)>();

    public static void StoreControlStyle(Control control)
    {
        if (!_originalStyles.ContainsKey(control))
        {
            _originalStyles[control] = (control.BackColor, control.ForeColor, control.Font);
        }
    }

    public static void RestoreControlStyle(Control control)
    {
        if (_originalStyles.ContainsKey(control))
        {
            var style = _originalStyles[control];
            control.BackColor = style.BackColor;
            control.ForeColor = style.ForeColor;
            control.Font = style.Font;
        }
    }

    public static void RestoreAllStyles(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            RestoreControlStyle(control);
            if (control.HasChildren)
            {
                RestoreAllStyles(control.Controls);
            }
        }
    }
}