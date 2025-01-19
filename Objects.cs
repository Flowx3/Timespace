using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeSpace
{
    public class MapObject
    {
        public string MapName { get; set; }
        public string ObjectType { get; set; }
        public int? X { get; private set; }
        public int Y { get; private set; }
        public Panel Panel { get; private set; }
        private CustomMaterialStyleComboBox cboObjectiveType;
        private TextBox txtX;
        private TextBox txtY;
        private ModernButton btnConfigurePortals;
        public List<string> selectedEvents;
        private List<string> lockedPortalsList;
        private CustomTabPage _tabPage;

        public MapObject(string mapName, string objectType, int x, int y, List<string> lockedPortalsList, CustomTabPage tabpage)
        {
            MapName = mapName;
            ObjectType = objectType;
            X = x;
            Y = y;
            this.lockedPortalsList = lockedPortalsList;
            selectedEvents = new List<string>();
            _tabPage = tabpage;
        }

        public Panel CreateObject()
        {
            var panel = new Panel { Width = 600, Height = 75 };
            var lblObjectiveType = new Label { Text = "Objective Type:", Location = new System.Drawing.Point(0, 5), AutoSize = true };
            cboObjectiveType = new CustomMaterialStyleComboBox { Location = new System.Drawing.Point(125, 0), Width = 150 };
            cboObjectiveType.Items.AddRange(new string[] { "OldBox", "RegularBox", "LuxuriousBox", "Lever" });
            cboObjectiveType.SelectedIndexChanged += CboObjectiveType_SelectedIndexChanged;

            var lblX = new Label { Text = "X:", Location = new System.Drawing.Point(0, 35), AutoSize = true };
            txtX = new ModernTextBox { Location = new System.Drawing.Point(25, 30), Width = 50 };
            txtX.Text = X.ToString();
            var lblY = new Label { Text = "Y:", Location = new System.Drawing.Point(90, 35), AutoSize = true };
            txtY = new ModernTextBox { Location = new System.Drawing.Point(110, 30), Width = 50 };
            txtY.Text = Y.ToString();

            btnConfigurePortals = new ModernButton { Text = "Configure Portals", Location = new System.Drawing.Point(175, 30), Visible = false, Width = 150 };
            btnConfigurePortals.Click += BtnConfigurePortals_Click;

            panel.Controls.Add(lblObjectiveType);
            panel.Controls.Add(cboObjectiveType);
            panel.Controls.Add(lblX);
            panel.Controls.Add(txtX);
            panel.Controls.Add(lblY);
            panel.Controls.Add(txtY);
            panel.Controls.Add(btnConfigurePortals);

            Panel = panel;
            return panel;
        }

        private void CboObjectiveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboObjectiveType.SelectedItem?.ToString() == "Lever")
            {
                btnConfigurePortals.Visible = true;
            }
            else
            {
                btnConfigurePortals.Visible = false;
            }
        }

        private void BtnConfigurePortals_Click(object sender, EventArgs e)
        {
            using (var eventManager = new EventManagerForm(MapName, _tabPage.GetMapNames, lockedPortalsList, selectedEvents))
            {
                if (eventManager.ShowDialog() == DialogResult.OK)
                {
                    selectedEvents = eventManager.Events;
                }
            }
        }

        public void UpdatePortalsList(List<string> newPortalsList)
        {
            lockedPortalsList = newPortalsList;
        }

        public void SetObjectivesAndPortals(string objectiveType, List<string> events)
        {
            cboObjectiveType.SelectedItem = objectiveType;
            if (objectiveType == "Lever")
            {
                selectedEvents = events ?? new List<string>();
            }
        }

        public void SetPosition(int x, int y)
        {
            txtX.Text = x.ToString();
            txtY.Text = y.ToString();
            X = x;
            Y = y;
        }

        public int GetX()
        {
            if (txtX.Text == "") return 1500;
            return int.Parse(txtX?.Text);
        }

        public int GetY()
        {
            if (txtY.Text == "") return 1500;
            return int.Parse(txtY?.Text);
        }

        public string GenerateObjectiveScript()
        {
            var script = new StringBuilder();
            string objectiveType = cboObjectiveType?.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(txtX.Text) || string.IsNullOrEmpty(txtY.Text) || string.IsNullOrEmpty(objectiveType))
            {
                return string.Empty;
            }

            script.Append($"    MapObject.Create{objectiveType}().At({txtX.Text}, {txtY.Text})");

            if (objectiveType == "Lever" && selectedEvents.Any())
            {
                script.Append(".OnSwitch({ \n");
                foreach (var evt in selectedEvents)
                {
                    script.AppendLine($"        {evt}");
                }
                script.Append("    })");
            }

            script.Append(",");
            return script.ToString();
        }
    }
}