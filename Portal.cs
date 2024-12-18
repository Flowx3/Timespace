using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using YamlDotNet.Core;

namespace TimeSpace
{
    public class Portal
    {
        public string MapFrom { get; set; }
        public string MapTo { get; set; }
        public string PortalType { get; set; }
        public string MinimapOrientation { get; set; }
        public int? FromX { get; set; }
        public int? FromY { get; set; }
        public int? ToX { get; set; }
        public int? ToY { get; set; }
        public Panel Panel { get; set; }
        public ComboBox cboMapFrom;
        public ComboBox cboMapTo;
        public ComboBox cboPortalType;
        public ComboBox cboMinimapOrientation;
        public TextBox txtFromX;
        public TextBox txtFromY;
        public TextBox txtToX;
        public TextBox txtToY;
        public Func<List<string>> getMapNames;
        private CustomTabPage customTabPage;

        public Portal(string mapFrom, string mapTo, string portalType, string minimapOrientation, int? fromX, int? fromY, int? toX, int? toY, Func<List<string>> GetMapNames, CustomTabPage customTabPage)
        {
            MapFrom = mapFrom;
            MapTo = mapTo;
            PortalType = portalType;
            MinimapOrientation = minimapOrientation;
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
            getMapNames = GetMapNames;
            this.customTabPage = customTabPage;
        }

        public Panel CreatePortal()
        {
            var panel = new Panel { Width = 880, Height = 60, Margin = new Padding(0, 5, 0, 5) };

            var lblMapFrom = new Label { Text = "Map From:", Location = new Point(0, 5), AutoSize = true };
            cboMapFrom = new SearchableComboBox { Location = new Point(80, 0), Width = 150 };
            cboMapFrom.Items.AddRange(getMapNames().ToArray());
            cboMapFrom.SelectedItem = MapFrom;

            var lblMapTo = new Label { Text = "Map To:", Location = new Point(240, 5), AutoSize = true };
            cboMapTo = new SearchableComboBox { Location = new Point(300, 0), Width = 150 };
            cboMapTo.Items.AddRange(getMapNames().Concat(new[] { "UNKNOWN" }).ToArray());
            cboMapTo.SelectedItem = MapTo;

            var lblPortalType = new Label { Text = "Portal Type:", Location = new Point(460, 5), AutoSize = true };
            cboPortalType = new SearchableComboBox { Location = new Point(530, 0), Width = 100 };
            cboPortalType.Items.AddRange(new string[] { "Locked", "TsNormal", "TSEndClosed", "TSEnd" });
            cboPortalType.SelectedItem = PortalType;

            var lblMinimapOrientation = new Label { Text = "Orientation:", Location = new Point(640, 5), AutoSize = true };
            cboMinimapOrientation = new SearchableComboBox { Location = new Point(710, 0), Width = 60 };
            cboMinimapOrientation.Items.AddRange(new string[] { "North", "South", "East", "West" });
            cboMinimapOrientation.SelectedItem = MinimapOrientation;

            var lblFromX = new Label { Text = "From X:", Location = new Point(0, 35), AutoSize = true };
            txtFromX = new ModernTextBox { Location = new Point(60, 35), Width = 50 };
            txtFromX.Text = FromX.ToString();

            var lblFromY = new Label { Text = "From Y:", Location = new Point(120, 35), AutoSize = true };
            txtFromY = new ModernTextBox { Location = new Point(180, 35), Width = 50 };
            txtFromY.Text = FromY.ToString();

            var lblToX = new Label { Text = "To X:", Location = new Point(240, 35), AutoSize = true };
            txtToX = new ModernTextBox { Location = new Point(300, 35), Width = 50 };
            txtToX.Text = ToX.ToString();

            var lblToY = new Label { Text = "To Y:", Location = new Point(360, 35), AutoSize = true };
            txtToY = new ModernTextBox { Location = new Point(420, 35), Width = 50 };
            txtToY.Text = ToY.ToString();

            var btnMirrorPortal = new ModernButton { Text = "Mirror Portal", Location = new Point(480, 30), Width = 100 };
            btnMirrorPortal.Click += (sender, e) => MirrorPortal();

            panel.Controls.Add(lblMapFrom);
            panel.Controls.Add(cboMapFrom);
            panel.Controls.Add(lblMapTo);
            panel.Controls.Add(cboMapTo);
            panel.Controls.Add(lblPortalType);
            panel.Controls.Add(cboPortalType);
            panel.Controls.Add(lblMinimapOrientation);
            panel.Controls.Add(cboMinimapOrientation);
            panel.Controls.Add(lblFromX);
            panel.Controls.Add(txtFromX);
            panel.Controls.Add(lblFromY);
            panel.Controls.Add(txtFromY);
            panel.Controls.Add(lblToX);
            panel.Controls.Add(lblToY);
            panel.Controls.Add(txtToX);
            panel.Controls.Add(txtToY);
            panel.Controls.Add(btnMirrorPortal);

            Panel = panel; // Assign the panel to the portal  

            return panel;
        }

        private void MirrorPortal()
        {
            // Store original values
            string originalMapFrom = cboMapFrom.SelectedItem?.ToString();
            string originalMapTo = cboMapTo.SelectedItem?.ToString();
            int originalFromX = int.Parse(txtFromX.Text);
            int originalFromY = int.Parse(txtFromY.Text);
            int originalToX = int.Parse(txtToX.Text);
            int originalToY = int.Parse(txtToY.Text);
            string originalOrientation = cboMinimapOrientation.SelectedItem.ToString();

            // Determine mirrored orientation
            string mirroredOrientation = originalOrientation switch
            {
                "North" => "South",
                "South" => "North",
                "East" => "West",
                "West" => "East",
                _ => originalOrientation
            };

            // Create a new mirrored portal without modifying the current one
            var mirroredPortal = new Portal(
                originalMapTo,  // Swap MapFrom and MapTo
                originalMapFrom,
                cboPortalType.SelectedItem.ToString(),
                mirroredOrientation,
                originalToX,    // Swap FromX/Y with ToX/Y
                originalToY,
                originalFromX,
                originalFromY,
                getMapNames,
                customTabPage
            );

            // Add the mirrored portal to the destination map
            customTabPage.AddPortalToMap(originalMapTo, mirroredPortal);
        }

        public void RefreshMapComboboxes()
        {
            if (cboMapFrom != null && cboMapTo != null)
            {
                // Store current selections  
                string currentMapFrom = cboMapFrom.SelectedItem?.ToString();
                string currentMapTo = cboMapTo.SelectedItem?.ToString();

                // Clear and refresh MapFrom combobox  
                cboMapFrom.Items.Clear();
                cboMapFrom.Items.AddRange(getMapNames().ToArray());

                // Clear and refresh MapTo combobox  
                cboMapTo.Items.Clear();
                cboMapTo.Items.AddRange(getMapNames().Concat(new[] { "UNKNOWN" }).ToArray());

                // Restore selections if they still exist  
                if (currentMapFrom != null && cboMapFrom.Items.Contains(currentMapFrom))
                    cboMapFrom.SelectedItem = currentMapFrom;
                if (currentMapTo != null && cboMapTo.Items.Contains(currentMapTo))
                    cboMapTo.SelectedItem = currentMapTo;
            }
        }

        public string GenerateLocalPortalScript()
        {
            MapFrom = MapFrom.Replace("map_", "");
            MapTo = MapTo.Replace("map_", "");
            int? toX = MapTo.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase) ? FromX : ToX;
            int? toY = MapTo.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase) ? FromY : ToY;
            string toLine;
            if (MapTo.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase))
            {
                toLine = $".To(map_{MapFrom}, {FromX}, {FromY})";
            }
            else
            {
                toLine = $".To(map_{MapTo}, {ToX}, {ToY})";
            }
            return $"local {GeneratePortalIdentifier()} = Portal.Create(PortalType.{PortalType})" +
                   $".From(map_{MapFrom}, {FromX}, {FromY})" +
                   toLine +
                   $".MinimapOrientation(PortalMinimapOrientation.{MinimapOrientation})";
        }

        public string GenerateAddPortalScript()
        {
            MapFrom = MapFrom.Replace("map_", "");
            return $"map_{MapFrom}.AddPortal({GeneratePortalIdentifier()})";
        }

        public string GeneratePortalIdentifier()
        {
            MapFrom = MapFrom.Replace("map_", "");
            MapTo = MapTo.Replace("map_", "");
            return $"portal_{MapFrom}_to_{MapTo}";
        }
    }
}