using System;
using System.Drawing;
using System.Windows.Forms;

public class SearchableComboBox : ComboBox
{
    public SearchableComboBox()
    {
        // Configure the ComboBox properties
        DropDownStyle = ComboBoxStyle.DropDown;
        AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        AutoCompleteSource = AutoCompleteSource.ListItems;

        // Set appearance
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        // Enable owner draw to customize appearance
        DrawMode = DrawMode.OwnerDrawFixed;
        DrawItem += SearchableComboBox_DrawItem;
    }

    private void SearchableComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        // Determine the background color
        Color backgroundColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
            ? Color.FromArgb(60, 60, 60) // Background when selected
            : Color.FromArgb(30, 30, 30); // Background when not selected

        // Fill the background
        using (var backgroundBrush = new SolidBrush(backgroundColor))
        {
            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
        }

        // Draw the text
        using (var textBrush = new SolidBrush(ForeColor))
        {
            string text = Items[e.Index]?.ToString() ?? string.Empty;
            e.Graphics.DrawString(
                text,
                Font,
                textBrush,
                e.Bounds.X,
                e.Bounds.Y
            );
        }

        // Draw focus rectangle
        e.DrawFocusRectangle();
    }
}
