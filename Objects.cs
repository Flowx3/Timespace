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
        public string ObjectType { get; set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public Panel Panel { get; private set; }
        private ComboBox cboObjectiveType;
        private TextBox txtX;
        private TextBox txtY;
        private ComboBox cboLeverPortal1;
        private ComboBox cboLeverPortal2;
        private ComboBox cboLeverPortal3;
        private List<string> allPortalsList;

        public MapObject(string objectType, int x, int y, List<string> allPortalsList)
        {
            ObjectType = objectType;
            X = x;
            Y = y;
            this.allPortalsList = allPortalsList;
        }

        public Panel CreateObject()
        {
            var panel = new Panel { Width = 600, Height = 100 };
            var lblObjectiveType = new Label { Text = "Objective Type:", Location = new System.Drawing.Point(0, 5), AutoSize = true };
            cboObjectiveType = new ComboBox { Location = new System.Drawing.Point(100, 0), Width = 150 };
            cboObjectiveType.Items.AddRange(new string[] { "OldBox", "RegularBox", "LuxuriousBox", "Lever" });
            cboObjectiveType.SelectedIndexChanged += CboObjectiveType_SelectedIndexChanged;
            var lblX = new Label { Text = "X:", Location = new System.Drawing.Point(0, 35), AutoSize = true };
            txtX = new TextBox { Location = new System.Drawing.Point(50, 30), Width = 50 };
            var lblY = new Label { Text = "Y:", Location = new System.Drawing.Point(110, 35), AutoSize = true };
            txtY = new TextBox { Location = new System.Drawing.Point(130, 30), Width = 50 };
            var btnSelectPosition = new Button { Text = "Select Position", Location = new System.Drawing.Point(190, 30) };
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
                cboLeverPortal1 = new ComboBox { Location = new System.Drawing.Point(50, 70), Width = 100 };
                cboLeverPortal1.Items.AddRange(allPortalsList?.ToArray());
                var lblLeverPortal2 = new Label { Text = "Portal 2:", Location = new System.Drawing.Point(160, 70), AutoSize = true };
                cboLeverPortal2 = new ComboBox { Location = new System.Drawing.Point(210, 70), Width = 100 };
                cboLeverPortal2.Items.AddRange(allPortalsList?.ToArray());
                var lblLeverPortal3 = new Label { Text = "Portal 3:", Location = new System.Drawing.Point(320, 70), AutoSize = true };
                cboLeverPortal3 = new ComboBox { Location = new System.Drawing.Point(370, 70), Width = 100 };
                cboLeverPortal3.Items.AddRange(allPortalsList?.ToArray());
                Panel.Controls.Add(lblLeverPortal1);
                Panel.Controls.Add(cboLeverPortal1);
                Panel.Controls.Add(lblLeverPortal2);
                Panel.Controls.Add(cboLeverPortal2);
                Panel.Controls.Add(lblLeverPortal3);
                Panel.Controls.Add(cboLeverPortal3);
            }
        }

        private void RemoveLeverPortalControls(Panel panel)
        {
            var controlsToRemove = panel.Controls.OfType<ComboBox>().Where(c => c.Location.Y > 60).ToList();
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
            string objectiveType = cboObjectiveType.SelectedItem.ToString();
            if (string.IsNullOrEmpty(txtX.Text) || string.IsNullOrEmpty(txtY.Text) || string.IsNullOrEmpty(objectiveType))
            {
                return string.Empty;
            }
            script.Append($"MapObject.Create{objectiveType}().At({txtX.Text}, {txtY.Text})");
            if (objectiveType == "Lever")
            {
                script.Append(".OnSwitch({ \n");
                if (cboLeverPortal1.SelectedItem != null)
                    script.AppendLine($"    Event.TogglePortal({cboLeverPortal1.SelectedItem}),");
                if (cboLeverPortal2.SelectedItem != null)
                    script.AppendLine($"    Event.TogglePortal({cboLeverPortal2.SelectedItem}),");
                if (cboLeverPortal3.SelectedItem != null)
                    script.AppendLine($"    Event.TogglePortal({cboLeverPortal3.SelectedItem}),");
                script.Append("})");
            }
            return script.ToString();
        }
    }
}
