﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TimeSpace
{
    public class Portal
    {
        public string MapFrom { get; set; }
        public string MapTo { get; set; }
        public string PortalType { get; set; }
        public string MinimapOrientation { get; set; }
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
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

        public Portal(string mapFrom, string mapTo, string portalType, string minimapOrientation, int fromX, int fromY, int toX, int toY, Func<List<string>> GetMapNames)
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
        }

        public Panel CreatePortal()
        {
            var panel = new Panel { Width = 780, Height = 50, Margin = new Padding(0, 5, 0, 5) };

            var lblMapFrom = new Label { Text = "Map From:", Location = new Point(0, 5), AutoSize = true };
            cboMapFrom = new ComboBox { Location = new Point(80, 0), Width = 150 };
            cboMapFrom.Items.AddRange(getMapNames().ToArray());
            cboMapFrom.SelectedItem = MapFrom;

            var lblMapTo = new Label { Text = "Map To:", Location = new Point(240, 5), AutoSize = true };
            cboMapTo = new ComboBox { Location = new Point(300, 0), Width = 150 };
            cboMapTo.Items.AddRange(getMapNames().Concat(new[] { "UNKNOWN" }).ToArray());
            cboMapTo.SelectedItem = MapTo;

            var lblPortalType = new Label { Text = "Portal Type:", Location = new Point(460, 5), AutoSize = true };
            cboPortalType = new ComboBox { Location = new Point(530, 0), Width = 100 };
            cboPortalType.Items.AddRange(new string[] { "Locked", "TsNormal", "TSEndClosed", "TSEnd" });
            cboPortalType.SelectedItem = PortalType;

            var lblMinimapOrientation = new Label { Text = "Orientation:", Location = new Point(640, 5), AutoSize = true };
            cboMinimapOrientation = new ComboBox { Location = new Point(710, 0), Width = 60 };
            cboMinimapOrientation.Items.AddRange(new string[] { "North", "South", "East", "West" });
            cboMinimapOrientation.SelectedItem = MinimapOrientation;

            var lblFromX = new Label { Text = "From X:", Location = new Point(0, 35), AutoSize = true };
            txtFromX = new TextBox { Location = new Point(60, 30), Width = 50 };
            txtFromX.Text = FromX.ToString();

            var lblFromY = new Label { Text = "From Y:", Location = new Point(120, 35), AutoSize = true };
            txtFromY = new TextBox { Location = new Point(180, 30), Width = 50 };
            txtFromY.Text = FromY.ToString();

            var lblToX = new Label { Text = "To X:", Location = new Point(240, 35), AutoSize = true };
            txtToX = new TextBox { Location = new Point(300, 30), Width = 50 };
            txtToX.Text = ToX.ToString();

            var lblToY = new Label { Text = "To Y:", Location = new Point(360, 35), AutoSize = true };
            txtToY = new TextBox { Location = new Point(420, 30), Width = 50 };
            txtToX.Text = ToX.ToString();

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

            Panel = panel; // Assign the panel to the portal  
            return panel;
        }

        public string GenerateLocalPortalScript()
        {
            MapFrom = MapFrom.Replace("map_", "");
            MapTo = MapTo.Replace("map_", "");
            int toX = MapTo.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase) ? FromX : ToX;
            int toY = MapTo.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase) ? FromY : ToY;
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
