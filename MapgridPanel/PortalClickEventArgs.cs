using TimeSpace;
using TimeSpace.MapgridPanel;

public class PortalClickEventArgs : EventArgs
{
    public Portal Portal { get; }
    public MapGridPanel Panel { get; }

    public PortalClickEventArgs(Portal portal, MapGridPanel panel)
    {
        Portal = portal;
        Panel = panel;
    }
}