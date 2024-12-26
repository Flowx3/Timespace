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
        public int X { get; private set; }
        public int Y { get; private set; }
        public Panel Panel { get; private set; }
        private CustomMaterialStyleComboBox cboObjectiveType;
        private TextBox txtX;
        private TextBox txtY;
        private CustomMaterialStyleComboBox cboLeverPortal1;
        private CustomMaterialStyleComboBox cboLeverPortal2;
        private CustomMaterialStyleComboBox cboLeverPortal3;
        private List<string> lockedPortalsList;

        public MapObject(string mapName, string objectType, int x, int y, List<string> lockedPortalsList)
        {
            MapName = mapName;
            ObjectType = objectType;
            X = x;
            Y = y; 
            this.lockedPortalsList = lockedPortalsList;
        }

        public Panel CreateObject()
        {
            var panel = new Panel { Width = 600, Height = 100 };
            var lblObjectiveType = new Label { Text = "Objective Type:", Location = new System.Drawing.Point(0, 5), AutoSize = true };
            cboObjectiveType = new CustomMaterialStyleComboBox { Location = new System.Drawing.Point(100, 0), Width = 150 };
            cboObjectiveType.Items.AddRange(new string[] { "OldBox", "RegularBox", "LuxuriousBox", "Lever" });
            cboObjectiveType.SelectedIndexChanged += CboObjectiveType_SelectedIndexChanged;
            var lblX = new Label { Text = "X:", Location = new System.Drawing.Point(0, 35), AutoSize = true };
            txtX = new ModernTextBox { Location = new System.Drawing.Point(50, 30), Width = 50 };
            var lblY = new Label { Text = "Y:", Location = new System.Drawing.Point(110, 35), AutoSize = true };
            txtY = new ModernTextBox { Location = new System.Drawing.Point(130, 30), Width = 50 };
            var btnSelectPosition = new ModernButton { Text = "Select Position", Location = new System.Drawing.Point(190, 30) };
            panel.Controls.Add(lblObjectiveType);
            panel.Controls.Add(cboObjectiveType);
            panel.Controls.Add(lblX);
            panel.Controls.Add(txtX);
            panel.Controls.Add(lblY);
            panel.Controls.Add(txtY);
            panel.Controls.Add(btnSelectPosition);

            Panel = panel;

            return panel;
        }

        private void CboObjectiveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveLeverPortalControls(Panel);
            if (cboObjectiveType.SelectedItem?.ToString() == "Lever")
            {
                var lblLeverPortal1 = new Label { Text = "Portal 1:", Location = new System.Drawing.Point(0, 70), AutoSize = true };
                cboLeverPortal1 = new CustomMaterialStyleComboBox { Location = new System.Drawing.Point(50, 70), Width = 100 };
                cboLeverPortal1.Click += LeverPortalCombobox_Click; // Add click handler

                var lblLeverPortal2 = new Label { Text = "Portal 2:", Location = new System.Drawing.Point(160, 70), AutoSize = true };
                cboLeverPortal2 = new CustomMaterialStyleComboBox { Location = new System.Drawing.Point(210, 70), Width = 100 };
                cboLeverPortal2.Click += LeverPortalCombobox_Click; // Add click handler

                var lblLeverPortal3 = new Label { Text = "Portal 3:", Location = new System.Drawing.Point(320, 70), AutoSize = true };
                cboLeverPortal3 = new CustomMaterialStyleComboBox { Location = new System.Drawing.Point(370, 70), Width = 100 };
                cboLeverPortal3.Click += LeverPortalCombobox_Click; // Add click handler

                Panel.Controls.Add(lblLeverPortal1);
                Panel.Controls.Add(cboLeverPortal1);
                Panel.Controls.Add(lblLeverPortal2);
                Panel.Controls.Add(cboLeverPortal2);
                Panel.Controls.Add(lblLeverPortal3);
                Panel.Controls.Add(cboLeverPortal3);
            }
        }
        public void UpdatePortalComboboxes(List<string> newPortalsList)
        {
            lockedPortalsList = newPortalsList;

            // Only update if the object is a lever and comboboxes exist
            if (cboObjectiveType?.SelectedItem?.ToString() == "Lever")
            {
                // Store currently selected values
                string selected1 = cboLeverPortal1?.SelectedItem?.ToString();
                string selected2 = cboLeverPortal2?.SelectedItem?.ToString();
                string selected3 = cboLeverPortal3?.SelectedItem?.ToString();

                // Clear and refresh items
                if (cboLeverPortal1 != null)
                {
                    cboLeverPortal1.Items.Clear();
                    cboLeverPortal1.Items.AddRange(lockedPortalsList.ToArray());
                    if (selected1 != null && lockedPortalsList.Contains(selected1))
                        cboLeverPortal1.SelectedItem = selected1;
                }

                if (cboLeverPortal2 != null)
                {
                    cboLeverPortal2.Items.Clear();
                    cboLeverPortal2.Items.AddRange(lockedPortalsList.ToArray());
                    if (selected2 != null && lockedPortalsList.Contains(selected2))
                        cboLeverPortal2.SelectedItem = selected2;
                }

                if (cboLeverPortal3 != null)
                {
                    cboLeverPortal3.Items.Clear();
                    cboLeverPortal3.Items.AddRange(lockedPortalsList.ToArray());
                    if (selected3 != null && lockedPortalsList.Contains(selected3))
                        cboLeverPortal3.SelectedItem = selected3;
                }
            }
        }
        private void LeverPortalCombobox_Click(object sender, EventArgs e)
        {
            var comboBox = sender as CustomMaterialStyleComboBox;
            if (comboBox != null)
            {
                // Store the current selection
                string currentSelection = comboBox.SelectedItem?.ToString();

                // Clear and refresh the items
                comboBox.Items.Clear();

                // Regenerate the portal list from the current state
                var currentPortals = lockedPortalsList?.ToArray() ?? Array.Empty<string>();
                comboBox.Items.AddRange(currentPortals);

                // Restore the previous selection if it still exists
                if (currentSelection != null && comboBox.Items.Contains(currentSelection))
                {
                    comboBox.SelectedItem = currentSelection;
                }
            }
        }
        private void RemoveLeverPortalControls(Panel panel)
        {
            var controlsToRemove = panel.Controls.OfType<CustomMaterialStyleComboBox>().Where(c => c.Location.Y > 60).ToList();
            foreach (var control in controlsToRemove)
            {
                panel.Controls.Remove(control);
            }

            var labelsToRemove = panel.Controls.OfType<Label>().Where(l => l.Location.Y > 60).ToList();
            foreach (var label in labelsToRemove)
            {
                panel.Controls.Remove(label);
            }
        }
        public void SetObjectivesAndPortals(string objectiveType, List<Portal> portals)
        {
            cboObjectiveType.SelectedItem = objectiveType;
            if (objectiveType == "Lever")
            {
                var portal1 = portals.FirstOrDefault(p => p.MapFrom == cboLeverPortal1.SelectedItem?.ToString());
                if (portal1 != null)
                    cboLeverPortal1.SelectedItem = portal1.MapFrom;

                var portal2 = portals.FirstOrDefault(p => p.MapFrom == cboLeverPortal2.SelectedItem?.ToString());
                if (portal2 != null)
                    cboLeverPortal2.SelectedItem = portal2.MapFrom;

                var portal3 = portals.FirstOrDefault(p => p.MapFrom == cboLeverPortal3.SelectedItem?.ToString());
                if (portal3 != null)
                    cboLeverPortal3.SelectedItem = portal3.MapFrom;
            }
        }
        public void SetPosition(int x, int y)
        {
            txtX.Text = x.ToString();
            txtY.Text = y.ToString();
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
            // Here you would generate the script for each object  
            // For now, we'll assume the current object is the only one  
            string objectiveType = cboObjectiveType?.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(txtX.Text) || string.IsNullOrEmpty(txtY.Text) || string.IsNullOrEmpty(objectiveType))
            {
                return string.Empty;
            }
            script.Append($"    MapObject.Create{objectiveType}().At({txtX.Text}, {txtY.Text})");
            if (objectiveType == "Lever")
            {
                script.Append(".OnSwitch({ \n");
                if (cboLeverPortal1.SelectedItem != null)
                    script.AppendLine($"        Event.TogglePortal({cboLeverPortal1.SelectedItem}),");
                if (cboLeverPortal2.SelectedItem != null)
                    script.AppendLine($"        Event.TogglePortal({cboLeverPortal2.SelectedItem}),");
                if (cboLeverPortal3.SelectedItem != null)
                    script.AppendLine($"        Event.TogglePortal({cboLeverPortal3.SelectedItem}),");
                script.Append("    }),");
            }
            script.AppendLine("})");

            return script.ToString();
        }
    }
}
