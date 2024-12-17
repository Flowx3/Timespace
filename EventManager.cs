using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public enum TimeSpaceFinishType
{
    TIME_IS_UP = 1,
    NPC_DIED = 2,
    OUT_OF_LIVES = 3,
    TEAM_MEMBER_OUT_OF_LIVES = 4,
    SUCCESS = 5,
    SUCCESS_HIGH_SCORE = 6
}

public class ModernEventManagerForm : Form
{
    private ComboBox eventTypeComboBox;
    private Panel eventConfigPanel;
    private Button applyButton;
    private string currentMapName;
    private List<string> mapList;
    private List<string> portalList;

    // Event-specific controls
    private SearchableComboBox mapSearchComboBox;
    private SearchableComboBox portalSearchComboBox;
    private SearchableComboBox finishTypeComboBox;
    private ModernNumericUpDown timeNumericUpDown;
    private ModernCheckBox despawnMobsCheckBox;

    public ModernEventManagerForm(string currentMap, List<string> maps, List<string> portals)
    {
        currentMapName = currentMap;
        mapList = maps;
        portalList = portals;
        InitializeComponents();
        StyleComponents();
    }

    private void InitializeComponents()
    {
        // Form settings
        Text = "Event Manager";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(30, 30, 35);

        // Event Type Selection
        var lblEventType = new Label
        {
            Text = "Event Type:",
            Location = new Point(20, 20),
            Size = new Size(100, 25),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };

        eventTypeComboBox = new ComboBox
        {
            Location = new Point(130, 20),
            Size = new Size(200, 30),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };

        eventTypeComboBox.Items.AddRange(new string[] {
            "TryStartTaskForMap",
            "OpenPortal",
            "FinishTimeSpace",
            "SetTime",
            "AddTime",
            "DespawnAllMobsInRoom"
        });

        eventTypeComboBox.SelectedIndexChanged += EventType_Changed;

        // Event Configuration Panel
        eventConfigPanel = new Panel
        {
            Location = new Point(20, 60),
            Size = new Size(540, 330),
            BackColor = Color.FromArgb(40, 40, 45)
        };

        // Initialize all possible controls
        InitializeEventControls();

        // Apply Button
        applyButton = new Button
        {
            Text = "Apply",
            Location = new Point(230, 410),
            Size = new Size(120, 35),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        applyButton.Click += ApplyButton_Click;

        // Add controls to form
        Controls.AddRange(new Control[] {
            lblEventType,
            eventTypeComboBox,
            eventConfigPanel,
            applyButton
        });

        eventTypeComboBox.SelectedIndex = 0;
    }

    private void InitializeEventControls()
    {
        // Map Search ComboBox
        mapSearchComboBox = new SearchableComboBox
        {
            Location = new Point(20, 20),
            Size = new Size(500, 30),
            Items = { mapList }
        };

        // Portal Search ComboBox
        portalSearchComboBox = new SearchableComboBox
        {
            Location = new Point(20, 20),
            Size = new Size(500, 30),
            Items = { portalList }
        };

        // TimeSpace Finish Type ComboBox
        finishTypeComboBox = new SearchableComboBox
        {
            Location = new Point(20, 20),
            Size = new Size(500, 30),
            Items = { Enum.GetNames(typeof(TimeSpaceFinishType)).ToList() }
        };

        // Time NumericUpDown
        timeNumericUpDown = new ModernNumericUpDown
        {
            Location = new Point(20, 20),
            Size = new Size(200, 30),
            Minimum = 0,
            Maximum = 3600,
            Value = 0,
        };

        // Despawn Mobs CheckBox
        despawnMobsCheckBox = new ModernCheckBox
        {
            Location = new Point(20, 20),
            Size = new Size(200, 30),
            Text = "Despawn All Mobs"
        };
    }

    private void EventType_Changed(object sender, EventArgs e)
    {
        eventConfigPanel.Controls.Clear();

        switch (eventTypeComboBox.SelectedItem.ToString())
        {
            case "TryStartTaskForMap":
                eventConfigPanel.Controls.Add(mapSearchComboBox);
                break;

            case "OpenPortal":
                eventConfigPanel.Controls.Add(portalSearchComboBox);
                break;

            case "FinishTimeSpace":
                eventConfigPanel.Controls.Add(finishTypeComboBox);
                break;

            case "SetTime":
            case "AddTime":
                eventConfigPanel.Controls.Add(timeNumericUpDown);
                break;

            case "DespawnAllMobsInRoom":
                eventConfigPanel.Controls.Add(despawnMobsCheckBox);
                break;
        }
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
        string eventScript = GenerateEventScript();
        if (!string.IsNullOrEmpty(eventScript))
        {
            Tag = eventScript;
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private string GenerateEventScript()
    {
        string eventType = eventTypeComboBox.SelectedItem.ToString();
        string value = "";

        switch (eventType)
        {
            case "TryStartTaskForMap":
                value = mapSearchComboBox.SelectedItem?.ToString();
                break;

            case "OpenPortal":
                value = portalSearchComboBox.SelectedItem?.ToString();
                break;

            case "FinishTimeSpace":
                value = $"TimeSpaceFinishType.{finishTypeComboBox.SelectedItem}";
                break;

            case "SetTime":
            case "AddTime":
                value = timeNumericUpDown.Value.ToString();
                break;

            case "DespawnAllMobsInRoom":
                value = currentMapName;
                break;
        }

        if (string.IsNullOrEmpty(value) && eventType != "DespawnAllMobsInRoom")
            return null;

        return $"Event.{eventType}({value})";
    }

    private void InitializeComponent()
    {

    }

    private void StyleComponents()
    {
        // Add modern styling for all controls
        foreach (Control control in Controls)
        {
            if (control is ComboBox combo)
            {
                combo.FlatStyle = FlatStyle.Flat;
                combo.BackColor = Color.FromArgb(45, 45, 50);
                combo.ForeColor = Color.White;
            }
            else if (control is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.BackColor = Color.FromArgb(45, 45, 50);
                txt.ForeColor = Color.White;
            }
            else if (control is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.FromArgb(70, 130, 180);
                btn.BackColor = Color.FromArgb(70, 130, 180);
                btn.ForeColor = Color.White;
            }
        }
    }
}