using System.Text;

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
    private List<string> existingEvents;

    public TaskEventManagerForm(string mapName, List<string> lockedPortalsList, List<string> existingEventScripts = null)
    {
        currentMapName = mapName;
        existingEvents = existingEventScripts ?? new List<string>();
        InitializeComponents(lockedPortalsList);
        LoadExistingEvents();
    }

    private void LoadExistingEvents()
    {
        if (existingEvents.Count == 0) return;

        // Load the first event by default
        string eventScript = existingEvents[0];

        // Determine event type
        bool isTaskFinish = eventScript.Contains(".OnTaskFinish");
        eventTypeComboBox.SelectedItem = isTaskFinish ? "TaskFinish" : "TaskFail";

        // Extract portals
        var portalMatches = System.Text.RegularExpressions.Regex.Matches(eventScript, @"Event\.OpenPortal\((portal_[^)]+)\)");
        ComboBox[] activePortals = isTaskFinish ? taskFinishPortals : taskFailPortals;

        for (int i = 0; i < Math.Min(portalMatches.Count, activePortals.Length); i++)
        {
            string portalName = portalMatches[i].Groups[1].Value;
            if (activePortals[i].SelectedItem != portalName)
                activePortals[i].SelectedItem = portalName;
        }

        // Extract time modifications
        var addTimeMatch = System.Text.RegularExpressions.Regex.Match(eventScript, @"Event\.AddTime\((\d+)\)");
        if (addTimeMatch.Success && isTaskFinish)
        {
            addTimeEvent.Value = int.Parse(addTimeMatch.Groups[1].Value);
        }

        var removeTimeMatch = System.Text.RegularExpressions.Regex.Match(eventScript, @"Event\.RemoveTime\((\d+)\)");
        if (removeTimeMatch.Success && !isTaskFinish)
        {
            removeTimeEvent.Value = int.Parse(removeTimeMatch.Groups[1].Value);
        }

        // Check for despawn mobs
        despawnAllMobsInRoom.Checked = eventScript.Contains("Event.DespawnAllMobsInRoom");
    }

    private void InitializeComponents(List<string> lockedPortalsList)
    {
        // Form settings
        Text = "Task Event Manager";
        Size = new Size(600, 450);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(50, 50, 50);

        // Event Type Selection
        var lblEventType = new Label
        {
            Text = "Event Type:",
            Location = new Point(20, 20),
            Width = 80,
            ForeColor = Color.White
        };
        eventTypeComboBox = new ComboBox
        {
            Location = new Point(100, 20),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White
        };
        eventTypeComboBox.Items.AddRange(new string[] { "TaskFinish", "TaskFail" });
        eventTypeComboBox.SelectedIndex = 0;
        eventTypeComboBox.SelectedIndexChanged += EventType_Changed;

        // Initialize portal ComboBoxes
        taskFinishPortals = new ComboBox[4];
        taskFailPortals = new ComboBox[4];

        // Add empty option to portal list
        var portalsList = new List<string> { "" };  // Add empty option
        if (lockedPortalsList != null && lockedPortalsList.Any())
        {
            portalsList.AddRange(lockedPortalsList);
        }

        for (int i = 0; i < 4; i++)
        {
            var lblPortal = new Label
            {
                Text = $"Portal {i + 1}:",
                Location = new Point(20, 60 + (i * 40)),
                Width = 80,
                ForeColor = Color.White
            };

            taskFinishPortals[i] = new ComboBox
            {
                Location = new Point(100, 60 + (i * 40)),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };
            taskFinishPortals[i].Items.AddRange(portalsList.ToArray());
            taskFinishPortals[i].SelectedIndex = 0;  // Will select the empty option

            taskFailPortals[i] = new ComboBox
            {
                Location = new Point(100, 60 + (i * 40)),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Visible = false
            };
            taskFailPortals[i].Items.AddRange(portalsList.ToArray());
            taskFailPortals[i].SelectedIndex = 0;  // Will select the empty option

            Controls.Add(lblPortal);
            Controls.Add(taskFinishPortals[i]);
            Controls.Add(taskFailPortals[i]);
        }

        // Time modifications
        var lblAddTime = new Label
        {
            Text = "Add Time:",
            Location = new Point(20, 220),
            Width = 80,
            ForeColor = Color.White
        };
        addTimeEvent = new NumericUpDown
        {
            Location = new Point(100, 220),
            Width = 100,
            Minimum = 0,
            Maximum = 999,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White
        };
        var lblRemoveTime = new Label
        {
            Text = "Remove Time:",
            Location = new Point(220, 220),
            Width = 80,
            ForeColor = Color.White
        };
        removeTimeEvent = new NumericUpDown
        {
            Location = new Point(300, 220),
            Width = 100,
            Minimum = 0,
            Maximum = 999,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White
        };

        // Despawn mobs checkbox
        despawnAllMobsInRoom = new CheckBox
        {
            Text = "Despawn All Mobs",
            Location = new Point(20, 300),
            AutoSize = true,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 50)
        };

        // Apply button
        applyButton = new Button
        {
            Text = "Apply",
            Location = new Point(20, 340),
            Width = 100,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White
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

    public void UpdatePortalComboboxes(List<string> newPortalsList)
    {
        // Add empty option to portal list
        var portalsList = new List<string> { "" };  // Add empty option
        if (newPortalsList != null && newPortalsList.Any())
        {
            portalsList.AddRange(newPortalsList);
        }

        foreach (var comboBox in taskFinishPortals.Concat(taskFailPortals))
        {
            if (comboBox != null)
            {
                string selectedValue = comboBox.SelectedItem?.ToString();
                comboBox.Items.Clear();
                comboBox.Items.AddRange(portalsList.ToArray());

                if (selectedValue != null && portalsList.Contains(selectedValue))
                {
                    comboBox.SelectedItem = selectedValue;
                }
                else
                {
                    comboBox.SelectedIndex = 0;  // Select empty option
                }
            }
        }
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

        // Handle finish timespace event for TaskFail
        if (!isTaskFinish)
        {
            script.AppendLine($"    Event.FinishTimeSpace(TimeSpaceFinishType.TIME_IS_UP),");
        }

        script.AppendLine("})");
        DialogResult = DialogResult.OK;
        Tag = script.ToString();
        Close();
    }
}
