using TimeSpace;

public class PortalClickEventArgs : EventArgs
{
    public Portal Portal { get; }
    public MapGridOverviewPanel Panel { get; }

    public PortalClickEventArgs(Portal portal, MapGridOverviewPanel panel)
    {
        Portal = portal;
        Panel = panel;
    }
}