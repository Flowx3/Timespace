using TimeSpace;

public class EventManagerForm : Form
{
    private ListBox eventTypeListBox;
    private ListView selectedEventsListView;
    private Panel eventConfigPanel;
    private Button applyButton;
    private string currentMapName;
    private Func<List<string>> mapList;
    private List<string> portalList;
    private List<string> selectedEvents;
    private string formTitle;

    // Event-specific controls
    private SearchableComboBox mapSearchComboBox;
    private SearchableComboBox portalSearchComboBox;
    private SearchableComboBox finishTypeComboBox;
    private ModernNumericUpDown timeNumericUpDown;

    public List<string> Events => selectedEvents;

    public EventManagerForm(string currentMap, Func<List<string>> maps, List<string> portals, List<string> existingEvents = null)
    {
        currentMapName = currentMap;
        mapList = maps;
        portalList = portals;
        selectedEvents = existingEvents ?? new List<string>();
        formTitle = "Event Manager";
        InitializeComponents();
        StyleComponents();
        LoadExistingEvents();
    }

    private void InitializeComponents()
    {
        // Form settings
        Text = formTitle;
        Size = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(30, 30, 35);

        // Left Panel - Event Types
        var leftPanel = CreateLeftPanel();

        // Right Panel - Selected Events and Configuration
        var rightPanel = CreateRightPanel();

        // Add panels to form
        Controls.Add(rightPanel);
        Controls.Add(leftPanel);
    }

    private Panel CreateLeftPanel()
    {
        var leftPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 200,
            BackColor = Color.FromArgb(40, 40, 45),
            Padding = new Padding(10)
        };

        var lblAvailableEvents = new Label
        {
            Text = "Available Events",
            Dock = DockStyle.Top,
            Height = 30,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        eventTypeListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.None
        };

        eventTypeListBox.Items.AddRange(new string[] {
            "TryStartTaskForMap",
            "OpenPortal",
            "FinishTimeSpace",
            "SetTime",
            "AddTime",
            "DespawnAllMobsInRoom"
        });

        eventTypeListBox.DoubleClick += EventType_DoubleClick;

        leftPanel.Controls.Add(eventTypeListBox);
        leftPanel.Controls.Add(lblAvailableEvents);

        return leftPanel;
    }

    private Panel CreateRightPanel()
    {
        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        var lblSelectedEvents = new Label
        {
            Text = "Selected Events",
            Height = 30,
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        selectedEventsListView = new ListView
        {
            Dock = DockStyle.Top,
            Height = 200,
            View = View.Details,
            FullRowSelect = true,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        selectedEventsListView.Columns.Add("Events", -2);
        selectedEventsListView.DoubleClick += SelectedEvents_DoubleClick;

        var buttonPanel = new FlowLayoutPanel
        {
            Height = 40,
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.FromArgb(30, 30, 35)
        };

        var removeButton = new Button
        {
            Text = "Remove Selected",
            Height = 30,
            Width = 120,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White
        };
        removeButton.Click += RemoveEvent_Click;

        var editButton = new Button
        {
            Text = "Edit Selected",
            Height = 30,
            Width = 120,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White
        };
        editButton.Click += EditEvent_Click;

        buttonPanel.Controls.AddRange(new Control[] { removeButton, editButton });

        var lblConfiguration = new Label
        {
            Text = "Event Configuration",
            Height = 30,
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        eventConfigPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(40, 40, 45)
        };

        InitializeEventControls();

        applyButton = new Button
        {
            Text = "Apply",
            Height = 35,
            Dock = DockStyle.Bottom,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        applyButton.Click += ApplyButton_Click;

        rightPanel.Controls.Add(eventConfigPanel);
        rightPanel.Controls.Add(lblConfiguration);
        rightPanel.Controls.Add(buttonPanel);
        rightPanel.Controls.Add(selectedEventsListView);
        rightPanel.Controls.Add(lblSelectedEvents);
        rightPanel.Controls.Add(applyButton);

        return rightPanel;
    }

    private void InitializeEventControls()
    {
        mapSearchComboBox = new SearchableComboBox
        {
            Size = new Size(500, 30),
            Items = { mapList }
        };
        mapSearchComboBox.SelectedIndexChanged += ConfigControl_ValueChanged;

        portalSearchComboBox = new SearchableComboBox
        {
            Size = new Size(500, 30),
            Items = { portalList }
        };
        portalSearchComboBox.SelectedIndexChanged += ConfigControl_ValueChanged;

        finishTypeComboBox = new SearchableComboBox
        {
            Size = new Size(500, 30),
            Items = { Enum.GetNames(typeof(TimeSpaceFinishType)).ToList() }
        };
        finishTypeComboBox.SelectedIndexChanged += ConfigControl_ValueChanged;

        timeNumericUpDown = new ModernNumericUpDown
        {
            Size = new Size(200, 30),
            Minimum = 0,
            Maximum = 3600,
            Value = 0
        };
        timeNumericUpDown.ValueChanged += ConfigControl_ValueChanged;
    }

    private void ShowEventConfiguration(string eventType, string currentValue = null)
    {
        eventConfigPanel.Controls.Clear();
        eventTypeListBox.SelectedItem = eventType;

        Control configControl = null;
        Label label = new Label
        {
            Text = $"Configure {eventType}:",
            Location = new Point(10, 10),
            Size = new Size(200, 25),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };

        switch (eventType)
        {
            case "TryStartTaskForMap":
                configControl = mapSearchComboBox;
                if (currentValue != null)
                    mapSearchComboBox.SelectedItem = currentValue;
                break;

            case "OpenPortal":
                configControl = portalSearchComboBox;
                if (currentValue != null)
                    portalSearchComboBox.SelectedItem = currentValue;
                break;

            case "FinishTimeSpace":
                configControl = finishTypeComboBox;
                if (currentValue != null)
                    finishTypeComboBox.SelectedItem = currentValue.Replace("TimeSpaceFinishType.", "");
                break;

            case "SetTime":
            case "AddTime":
                configControl = timeNumericUpDown;
                if (currentValue != null && int.TryParse(currentValue, out int time))
                    timeNumericUpDown.Value = time;
                break;

            case "DespawnAllMobsInRoom":
                var script = $"Event.DespawnAllMobsInRoom({currentMapName})";
                AddEventToList(script);
                return;
        }

        if (configControl != null)
        {
            configControl.Location = new Point(10, 40);
            eventConfigPanel.Controls.Add(label);
            eventConfigPanel.Controls.Add(configControl);
        }
    }

    private void SelectedEvents_DoubleClick(object sender, EventArgs e)
    {
        EditSelectedEvent();
    }

    private void EditEvent_Click(object sender, EventArgs e)
    {
        EditSelectedEvent();
    }

    private void EditSelectedEvent()
    {
        if (selectedEventsListView.SelectedItems.Count == 0) return;

        var selectedEvent = selectedEventsListView.SelectedItems[0].Text;
        var match = System.Text.RegularExpressions.Regex.Match(selectedEvent, @"Event\.(\w+)\((.*)\)");

        if (match.Success)
        {
            var eventType = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            // Store the index for later removal
            var selectedIndex = selectedEventsListView.SelectedIndices[0];

            ShowEventConfiguration(eventType, value);

            // Remove the old event when showing configuration for editing
            selectedEvents.RemoveAt(selectedIndex);
            selectedEventsListView.Items.RemoveAt(selectedIndex);
        }
    }
    private void EventType_DoubleClick(object sender, EventArgs e)
    {
        if (eventTypeListBox.SelectedItem == null) return;

        var selectedType = eventTypeListBox.SelectedItem.ToString();
        eventConfigPanel.Controls.Clear();

        Control configControl = null;
        string defaultScript = null;

        switch (selectedType)
        {
            case "TryStartTaskForMap":
                configControl = mapSearchComboBox;
                configControl.Location = new Point(10, 10);
                break;

            case "OpenPortal":
                configControl = portalSearchComboBox;
                configControl.Location = new Point(10, 10);
                break;

            case "FinishTimeSpace":
                configControl = finishTypeComboBox;
                configControl.Location = new Point(10, 10);
                break;

            case "SetTime":
            case "AddTime":
                configControl = timeNumericUpDown;
                configControl.Location = new Point(10, 10);
                break;

            case "DespawnAllMobsInRoom":
                defaultScript = $"Event.DespawnAllMobsInRoom({currentMapName})";
                AddEventToList(defaultScript);
                return;
        }

        if (configControl != null)
        {
            var label = new Label
            {
                Text = $"Configure {selectedType}:",
                Location = new Point(10, 10),
                Size = new Size(200, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            configControl.Location = new Point(10, 40);
            eventConfigPanel.Controls.Add(label);
            eventConfigPanel.Controls.Add(configControl);
        }
    }

    private void ConfigControl_ValueChanged(object sender, EventArgs e)
    {
        if (eventTypeListBox.SelectedItem == null) return;

        var selectedType = eventTypeListBox.SelectedItem.ToString();
        string value = GetConfigurationValue(sender);

        if (!string.IsNullOrEmpty(value))
        {
            var script = $"Event.{selectedType}({value})";
            AddEventToList(script);
            eventConfigPanel.Controls.Clear();
        }
    }

    private string GetConfigurationValue(object sender)
    {
        if (sender is SearchableComboBox combo && combo.SelectedItem != null)
        {
            if (combo == finishTypeComboBox)
                return $"TimeSpaceFinishType.{combo.SelectedItem}";
            return combo.SelectedItem.ToString();
        }
        else if (sender is ModernNumericUpDown numeric)
        {
            return numeric.Value.ToString();
        }
        return null;
    }

    private void AddEventToList(string script)
    {
        selectedEvents.Add(script);
        selectedEventsListView.Items.Add(script);
    }

    private void RemoveEvent_Click(object sender, EventArgs e)
    {
        if (selectedEventsListView.SelectedItems.Count > 0)
        {
            var index = selectedEventsListView.SelectedIndices[0];
            selectedEvents.RemoveAt(index);
            selectedEventsListView.Items.RemoveAt(index);
        }
    }

    private void LoadExistingEvents()
    {
        if (selectedEvents != null)
        {
            foreach (var evt in selectedEvents)
            {
                selectedEventsListView.Items.Add(evt);
            }
        }
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void StyleComponents()
    {
        foreach (Control control in Controls)
        {
            StyleControl(control);
        }
    }

    private void StyleControl(Control control)
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

        foreach (Control child in control.Controls)
        {
            StyleControl(child);
        }
    }
}