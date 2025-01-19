public class CustomToolStripRenderer : ToolStripProfessionalRenderer
{
    private Color hoverColor = Color.FromArgb(50, 50, 50); // Set your desired hover color here

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected)
        {
            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
            using (SolidBrush brush = new SolidBrush(hoverColor))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }
        else
        {
            base.OnRenderMenuItemBackground(e);
        }
    }
}
