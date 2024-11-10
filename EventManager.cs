using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

public class TaskEventManagerForm : Form
{
    private ComboBox[] taskFinishPortals;
    private ComboBox[] taskFailPortals;
    private NumericUpDown addTimeEvent;
    private NumericUpDown removeTimeEvent;
    private CheckBox despawnAllMobsInRoom;
    private ComboBox eventTypeComboBox;
    private string currentMapName;
    private Button applyButton;

    public TaskEventManagerForm(string mapName, List<string> allPortalsList)
    {
        currentMapName = mapName;
        InitializeComponents(allPortalsList);
    }

    private void InitializeComponents(List<string> allPortalsList)
    {
        // Form settings  
        Text = "Task Event Manager";
        Size = new Size(600, 450);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        // Event Type Selection  
        var lblEventType = new Label
        {
            Text = "Event Type:",
            Location = new Point(20, 20),
            Width = 80
        };
        eventTypeComboBox = new ComboBox
        {
            Location = new Point(100, 20),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        eventTypeComboBox.Items.AddRange(new string[] { "TaskFinish", "TaskFail" });
        eventTypeComboBox.SelectedIndex = 0;
        eventTypeComboBox.SelectedIndexChanged += EventType_Changed;

        // Initialize portal ComboBoxes  
        taskFinishPortals = new ComboBox[4];
        taskFailPortals = new ComboBox[4];
        for (int i = 0; i < 4; i++)
        {
            var lblPortal = new Label
            {
                Text = $"Portal {i + 1}:",
                Location = new Point(20, 60 + (i * 40)),
                Width = 80
            };
            taskFinishPortals[i] = new ComboBox
            {
                Location = new Point(100, 60 + (i * 40)),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            taskFinishPortals[i].Items.Add(""); // Add empty option  
            taskFinishPortals[i].Items.AddRange(allPortalsList.ToArray());
            taskFinishPortals[i].SelectedIndex = 0;

            taskFailPortals[i] = new ComboBox
            {
                Location = new Point(100, 60 + (i * 40)),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };
            taskFailPortals[i].Items.Add(""); // Add empty option  
            taskFailPortals[i].Items.AddRange(allPortalsList.ToArray());
            taskFailPortals[i].SelectedIndex = 0;

            Controls.Add(lblPortal);
            Controls.Add(taskFinishPortals[i]);
            Controls.Add(taskFailPortals[i]);
        }

        // Time modifications  
        var lblAddTime = new Label
        {
            Text = "Add Time:",
            Location = new Point(20, 220),
            Width = 80
        };
        addTimeEvent = new NumericUpDown
        {
            Location = new Point(100, 220),
            Width = 100,
            Minimum = 0,
            Maximum = 999
        };
        var lblRemoveTime = new Label
        {
            Text = "Remove Time:",
            Location = new Point(220, 220),
            Width = 80
        };
        removeTimeEvent = new NumericUpDown
        {
            Location = new Point(300, 220),
            Width = 100,
            Minimum = 0,
            Maximum = 999
        };

        // Despawn mobs checkbox  
        despawnAllMobsInRoom = new CheckBox
        {
            Text = "Despawn All Mobs",
            Location = new Point(20, 300),
            AutoSize = true
        };

        // Apply button  
        applyButton = new Button
        {
            Text = "Apply",
            Location = new Point(20, 340),
            Width = 100
        };
        applyButton.Click += ApplyButton_Click;

        // Add controls to form  
        Controls.AddRange(new Control[] {
            lblEventType, eventTypeComboBox,
            lblAddTime, addTimeEvent,
            lblRemoveTime, removeTimeEvent,
            despawnAllMobsInRoom,
            applyButton
        });
    }

    private void EventType_Changed(object sender, EventArgs e)
    {
        // Ensure eventTypeComboBox.SelectedItem is not null  
        if (eventTypeComboBox.SelectedItem == null)
        {
            return;
        }

        bool isTaskFinish = eventTypeComboBox.SelectedItem.ToString() == "TaskFinish";
        for (int i = 0; i < 4; i++)
        {
            // Ensure taskFinishPortals and taskFailPortals elements are not null  
            if (taskFinishPortals != null && taskFinishPortals[i] != null)
            {
                taskFinishPortals[i].Visible = isTaskFinish;
            }
            if (taskFailPortals != null && taskFailPortals[i] != null)
            {
                taskFailPortals[i].Visible = !isTaskFinish;
            }
        }

        // Ensure addTimeEvent and removeTimeEvent are not null  
        if (addTimeEvent != null)
        {
            addTimeEvent.Enabled = isTaskFinish;
        }
        if (removeTimeEvent != null)
        {
            removeTimeEvent.Enabled = !isTaskFinish;
        }
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
        StringBuilder script = new StringBuilder();
        bool isTaskFinish = eventTypeComboBox.SelectedItem?.ToString() == "TaskFinish";
        ComboBox[] activePortals = isTaskFinish ? taskFinishPortals : taskFailPortals;

        // Check if any attributes are set  
        bool hasAttributes = activePortals.Any(p => p.SelectedItem?.ToString() != "") ||
                             (isTaskFinish && addTimeEvent.Value != 0) ||
                             (!isTaskFinish && removeTimeEvent.Value != 0) ||
                             despawnAllMobsInRoom.Checked;

        if (!hasAttributes) return;

        // Start building the script  
        script.AppendLine($"{currentMapName}.On{(isTaskFinish ? "TaskFinish" : "TaskFail")}({{");

        // Add portal events  
        foreach (var portal in activePortals)
        {
            if (portal.SelectedItem != null && portal.SelectedItem.ToString() != "")
            {
                script.AppendLine($"    Event.OpenPortal({portal.SelectedItem}),");
            }
        }

        // Add time modification  
        if (isTaskFinish && addTimeEvent.Value != 0)
        {
            script.AppendLine($"    Event.AddTime({addTimeEvent.Value}),");
        }
        else if (!isTaskFinish && removeTimeEvent.Value != 0)
        {
            script.AppendLine($"    Event.RemoveTime({removeTimeEvent.Value}),");
        }
        // Add despawn mobs  
        if (despawnAllMobsInRoom.Checked)
        {
            script.AppendLine($"    Event.DespawnAllMobsInRoom({currentMapName}),");
        }

        script.AppendLine("})");
        DialogResult = DialogResult.OK;
        Tag = script.ToString();
        Close();
    }
}