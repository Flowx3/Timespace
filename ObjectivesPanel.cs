using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TimeSpace;

public class CollapsibleObjectivesPanel : UserControl
{
    private Button expandButton;
    private Panel contentPanel;
    private CustomMaterialStyleComboBox objectiveTypeComboBox;
    private Panel objectiveValuesPanel;
    private ListBox selectedObjectivesList;
    private Button addObjectiveButton;
    private Button removeObjectiveButton;
    private Dictionary<string, (TextBox, TextBox)> objectiveValueControls;
    private bool isExpanded = false;
    private int expandedHeight = 400;
    private int collapsedHeight = 40;
    private const int PANEL_WIDTH = 400;

    public CollapsibleObjectivesPanel()
    {
        InitializeComponent();
        objectiveValueControls = new Dictionary<string, (TextBox, TextBox)>();
    }

    private void InitializeComponent()
    {
        // Main container setup
        BackColor = Color.FromArgb(45, 45, 48);
        Size = new Size(PANEL_WIDTH, collapsedHeight);
        MinimumSize = new Size(PANEL_WIDTH, collapsedHeight);
        MaximumSize = new Size(PANEL_WIDTH, expandedHeight);
        Margin = new Padding(0);
        Padding = new Padding(0);
        Location = new Point(1100, 16);
        // Expand button
        expandButton = new Button
        {
            Text = "▼ TimeSpace Objectives",
            TextAlign = ContentAlignment.MiddleLeft,
            Height = 40,
            Width = PANEL_WIDTH,
            BackColor = Color.FromArgb(45, 45, 48),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        expandButton.FlatAppearance.BorderSize = 0;
        expandButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 30, 30);
        expandButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);
        expandButton.Click += ExpandButton_Click;

        // Content panel
        contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Padding = new Padding(8),
            Width = PANEL_WIDTH,
            BackColor = Color.FromArgb(45, 45, 48)
        };

        // Objective type combo box
        objectiveTypeComboBox = new CustomMaterialStyleComboBox
        {
            Dock = DockStyle.Top,
            Width = PANEL_WIDTH - 16,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        objectiveTypeComboBox.Items.AddRange(new object[]
        {
            "KillAllMonsters",
            "GoToExit",
            "KillMonsterVnum",
            "CollectItem",
            "Conversation",
            "InteractObjects",
            "ProtectNPC"
        });
        objectiveTypeComboBox.SelectedIndexChanged += ObjectiveTypeComboBox_SelectedIndexChanged;

        // Values panel
        objectiveValuesPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            Padding = new Padding(0, 8, 0, 8),
            Width = PANEL_WIDTH - 16,
            BackColor = Color.FromArgb(45, 45, 48)
        };

        // Selected objectives list
        selectedObjectivesList = new ListBox
        {
            Dock = DockStyle.Fill,
            Width = PANEL_WIDTH - 16,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Buttons panel
        var buttonsPanel = new Panel
        {
            Height = 40,
            Dock = DockStyle.Bottom,
            Padding = new Padding(0, 8, 0, 0),
            Width = PANEL_WIDTH - 16,
            BackColor = Color.FromArgb(45, 45, 48)
        };

        // Add Objective Button
        addObjectiveButton = new Button
        {
            Text = "Add",
            Width = (PANEL_WIDTH - 32) / 2,
            Dock = DockStyle.Left,
            BackColor = Color.FromArgb(45, 45, 48),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        addObjectiveButton.FlatAppearance.BorderSize = 0;
        addObjectiveButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 30, 30);
        addObjectiveButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);
        addObjectiveButton.Click += AddObjectiveButton_Click;

        // Remove Objective Button
        removeObjectiveButton = new Button
        {
            Text = "Remove",
            Width = (PANEL_WIDTH - 32) / 2,
            Dock = DockStyle.Right,
            BackColor = Color.FromArgb(45, 45, 48),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        removeObjectiveButton.FlatAppearance.BorderSize = 0;
        removeObjectiveButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 30, 30);
        removeObjectiveButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);
        removeObjectiveButton.Click += RemoveObjectiveButton_Click;

        // Add controls to buttons panel
        buttonsPanel.Controls.AddRange(new Control[] { addObjectiveButton, removeObjectiveButton });

        // Add controls to content panel
        contentPanel.Controls.AddRange(new Control[]
        {
            buttonsPanel,
            selectedObjectivesList,
            objectiveValuesPanel,
            objectiveTypeComboBox
        });

        // Add main controls
        Controls.AddRange(new Control[] { contentPanel, expandButton });
    }

    private void CreateValueInputs(string label1, string label2)
    {
        var spacing = 12;
        var inputWidth = PANEL_WIDTH - 32;
        var inputHeight = 30;

        var txt1 = new TextBox
        {
            PlaceholderText = label1,
            Location = new Point(0, 8),
            Size = new Size(inputWidth, inputHeight),
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        objectiveValuesPanel.Controls.Add(txt1);

        TextBox txt2 = null;
        if (label2 != null)
        {
            txt2 = new TextBox
            {
                PlaceholderText = label2,
                Location = new Point(0, inputHeight + spacing),
                Size = new Size(inputWidth, inputHeight),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            objectiveValuesPanel.Controls.Add(txt2);
        }

        objectiveValueControls[objectiveTypeComboBox.SelectedItem.ToString()] = (txt1, txt2);
    }

    private void ExpandButton_Click(object sender, EventArgs e)
    {
        isExpanded = !isExpanded;
        expandButton.Text = isExpanded ? "▲ TimeSpace Objectives" : "▼ TimeSpace Objectives";
        Height = isExpanded ? expandedHeight : collapsedHeight;
        contentPanel.Visible = isExpanded;
    }

    private void ObjectiveTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedType = objectiveTypeComboBox.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedType)) return;

        objectiveValuesPanel.Controls.Clear();

        switch (selectedType)
        {
            case "KillMonsterVnum":
                CreateValueInputs("Monster Vnum", "Amount");
                break;
            case "CollectItem":
                CreateValueInputs("Item Vnum", "Amount");
                break;
            case "InteractObjects":
                CreateValueInputs("Object Vnum", "Amount");
                break;
            case "Conversation":
                CreateValueInputs("Conversation ID", null);
                break;
            default:
                objectiveValueControls.Remove(selectedType);
                break;
        }
    }

    private void AddObjectiveButton_Click(object sender, EventArgs e)
    {
        string selectedType = objectiveTypeComboBox.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedType)) return;

        string objective = "";
        switch (selectedType)
        {
            case "KillMonsterVnum":
            case "CollectItem":
            case "InteractObjects":
                if (objectiveValueControls.TryGetValue(selectedType, out var controls))
                {
                    objective = $".With{selectedType}({controls.Item1.Text}, {controls.Item2.Text})";
                }
                break;
            case "Conversation":
                if (objectiveValueControls.TryGetValue(selectedType, out var convControl))
                {
                    objective = $".WithConversations({convControl.Item1.Text})";
                }
                break;
            case "KillAllMonsters":
                objective = ".WithKillAllMonsters()";
                break;
            case "GoToExit":
                objective = ".WithGoToExit()";
                break;
            case "ProtectNPC":
                objective = ".WithProtectNPC()";
                break;
        }

        if (!string.IsNullOrEmpty(objective))
        {
            selectedObjectivesList.Items.Add(objective);
        }
    }

    private void RemoveObjectiveButton_Click(object sender, EventArgs e)
    {
        if (selectedObjectivesList.SelectedIndex != -1)
        {
            selectedObjectivesList.Items.RemoveAt(selectedObjectivesList.SelectedIndex);
        }
    }

    public string GetObjectivesScript()
    {
        if (selectedObjectivesList.Items.Count == 0) return "";

        StringBuilder script = new StringBuilder();
        script.AppendLine("local objectives = TimeSpaceObjective.Create()");

        foreach (string objective in selectedObjectivesList.Items)
        {
            script.AppendLine($"    {objective}");
        }

        return script.ToString();
    }
}
