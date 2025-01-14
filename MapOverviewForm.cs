using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TimeSpace
{
    public class MapOverviewForm : Form
    {
        private const int PADDING = 20;
        private const float SCALE_FACTOR = 0.5f;
        private Dictionary<string, MapGridOverviewPanel> gridPanels;
        private Panel mainPanel;
        private Panel controlPanel;
        private Button connectPortalsButton;
        private Label instructionLabel;
        private Portal selectedPortal;
        private bool isConnectingPortals;

        public MapOverviewForm(TabControl mapTabControl)
        {
            InitializeComponent();
            InitializeGridPanels(mapTabControl);
            LayoutGridPanels();
        }

        private void InitializeComponent()
        {
            Text = "Map Overview";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            mainPanel = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };

            controlPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(50, 50, 50)
            };

            connectPortalsButton = new Button
            {
                Text = "Connect Portals",
                Size = new Size(120, 30),
                Location = new Point(PADDING, 15),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            connectPortalsButton.Click += ConnectPortalsButton_Click;

            instructionLabel = new Label
            {
                Text = "Click on two portals to connect them",
                AutoSize = true,
                Location = new Point(160, 22),
                ForeColor = Color.White,
                Visible = false
            };

            controlPanel.Controls.Add(connectPortalsButton);
            controlPanel.Controls.Add(instructionLabel);

            Controls.Add(mainPanel);
            Controls.Add(controlPanel);

            gridPanels = new Dictionary<string, MapGridOverviewPanel>();
        }

        private void InitializeGridPanels(TabControl mapTabControl)
        {
            foreach (CustomTabPage tabPage in mapTabControl.Controls.OfType<CustomTabPage>())
            {
                var overviewPanel = new MapGridOverviewPanel(tabPage.MapGridPanel, SCALE_FACTOR)
                {
                    Tag = tabPage
                };
                overviewPanel.PortalClicked += OverviewPanel_PortalClicked;
                gridPanels.Add(tabPage.Text, overviewPanel);
                mainPanel.Controls.Add(overviewPanel);
            }
        }

        private void LayoutGridPanels()
        {
            int maxX = 0, maxY = 0;
            var positioned = new HashSet<string>();
            var toPosition = new Queue<(string mapId, int x, int y)>();

            // Start with the first map at (0,0)
            if (gridPanels.Count > 0)
            {
                var firstMap = gridPanels.First();
                toPosition.Enqueue((firstMap.Key, 0, 0));
            }

            while (toPosition.Count > 0)
            {
                var (currentMapId, x, y) = toPosition.Dequeue();
                if (positioned.Contains(currentMapId)) continue;

                var currentPanel = gridPanels[currentMapId];
                var tabPage = (CustomTabPage)currentPanel.Tag;

                // Position the current panel
                currentPanel.Location = new Point(
                    x * (currentPanel.Width + PADDING) + PADDING,
                    y * (currentPanel.Height + PADDING) + PADDING
                );
                positioned.Add(currentMapId);

                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);

                // Queue connected maps for positioning
                foreach (var portal in tabPage.Portals)
                {
                    if (portal.MapTo != null && !positioned.Contains(portal.MapTo))
                    {
                        // Determine relative position based on portal orientation
                        int nextX = x, nextY = y;
                        switch (portal.Orientation)
                        {
                            case "North": nextY--; break;
                            case "South": nextY++; break;
                            case "East": nextX++; break;
                            case "West": nextX--; break;
                        }

                        toPosition.Enqueue((portal.MapTo, nextX, nextY));
                    }
                }
            }

            // Set panel size
            mainPanel.AutoScrollMinSize = new Size(
                (maxX + 1) * (gridPanels.First().Value.Width + PADDING) + PADDING,
                (maxY + 1) * (gridPanels.First().Value.Height + PADDING) + PADDING
            );
        }

        private void ConnectPortalsButton_Click(object sender, EventArgs e)
        {
            isConnectingPortals = !isConnectingPortals;
            connectPortalsButton.BackColor = isConnectingPortals ?
                Color.FromArgb(0, 150, 136) : Color.FromArgb(30, 30, 30);
            instructionLabel.Visible = isConnectingPortals;
            selectedPortal = null;
        }

        private void OverviewPanel_PortalClicked(object sender, PortalClickEventArgs e)
        {
            if (!isConnectingPortals) return;

            if (selectedPortal == null)
            {
                selectedPortal = e.Portal;
                e.Panel.HighlightPortal(selectedPortal);
            }
            else
            {
                // Connect the portals
                ConnectPortals(selectedPortal, e.Portal);

                // Reset selection
                foreach (var panel in gridPanels.Values)
                {
                    panel.ClearPortalHighlight();
                }
                selectedPortal = null;
            }
        }

        private void ConnectPortals(Portal source, Portal target)
        {
            source.MapTo = ((CustomTabPage)target.Panel.Parent).MapName;
            source.ToX = target.FromX;
            source.ToY = target.FromY;

            target.MapTo = ((CustomTabPage)source.Panel.Parent).MapName;
            target.ToX = source.FromX;
            target.ToY = source.FromY;

            // Update the portal panels in both tabs
            var sourceTab = (CustomTabPage)source.Panel.Parent;
            var targetTab = (CustomTabPage)target.Panel.Parent;

            sourceTab.SaveAndRefreshPortals(this, EventArgs.Empty, false);
            targetTab.SaveAndRefreshPortals(this, EventArgs.Empty, false);
        }
    }