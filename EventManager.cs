using MaterialSkin.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using TimeSpace;
using Font = System.Drawing.Font;
using MaterialSkin;

public class EventManagerForm : MaterialForm
{
    private TreeView eventTreeView;
    private MaterialListView selectedEventsListView;
    private Panel eventConfigPanel;
    private MaterialButton applyButton;
    private string currentMapName;
    private Func<List<string>> mapList;
    private List<string> portalList;
    private List<string> initialEvents;
    private List<string> selectedEvents;
    private string formTitle;

    // Event-specific controls
    private CustomMaterialStyleComboBox mapSearchComboBox;
    private CustomMaterialStyleComboBox portalSearchComboBox;
    private CustomMaterialStyleComboBox finishTypeComboBox;
    private ModernNumericUpDown timeNumericUpDown;
    private Control currentConfigControl;

    public List<string> Events => selectedEvents;

    public EventManagerForm(string currentMap, Func<List<string>> maps, List<string> portals, List<string> existingEvents = null)
    {
        currentMapName = currentMap;
        mapList = maps;
        portalList = portals;
        selectedEvents = existingEvents ?? new List<string>();
        initialEvents = new List<string>(selectedEvents);
        formTitle = "Event Manager";
        InitializeComponents();
        LoadExistingEvents();
    }

    private void InitializeComponents()
    {
        // Form settings
        Text = formTitle;
        Size = new Size(1200, 700);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        // Disable the close button
        ControlBox = false;

        // Toolbox Panel - Event Types
        var toolboxPanel = CreateToolboxPanel();
        toolboxPanel.Location = new Point(0, 0);
        toolboxPanel.Size = new Size(250, ClientSize.Height);
        toolboxPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

        // Main Panel - Selected Events and Configuration
        var mainPanel = CreateMainPanel();
        mainPanel.Location = new Point(toolboxPanel.Width, 0);
        mainPanel.Size = new Size(ClientSize.Width - toolboxPanel.Width, ClientSize.Height);
        mainPanel.AutoScroll = false;
        eventConfigPanel.AutoScroll = false;
        mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        // Add panels to form
        Controls.Add(toolboxPanel);
        Controls.Add(mainPanel);

        // Apply modern styling
        StyleComponents();
    }

    private Panel CreateToolboxPanel()
    {
        var toolboxPanel = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(150, ClientSize.Height),
            Padding = new Padding(15),
            BackColor = Color.FromArgb(28, 28, 28),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
        };

        var lblToolbox = new MaterialLabel
        {
            Text = "Event Toolbox",
            Dock = DockStyle.Top,
            Height = 40,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 14F),
            Padding = new Padding(0, 10, 0, 10)
        };

        eventTreeView = new TreeView
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(38, 38, 38),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.None,
            ShowLines = false,
            ShowPlusMinus = false,
            ShowRootLines = false,
            ItemHeight = 30,
            Indent = 20,
        };

        // Populate TreeView with event types
        var rootNode = new TreeNode("Events");
        rootNode.Nodes.Add("TryStartTaskForMap");
        rootNode.Nodes.Add("OpenPortal");
        rootNode.Nodes.Add("FinishTimeSpace");
        rootNode.Nodes.Add("SetTime");
        rootNode.Nodes.Add("AddTime");
        rootNode.Nodes.Add("DespawnAllMobsInRoom");
        eventTreeView.Nodes.Add(rootNode);
        eventTreeView.ExpandAll();
        eventTreeView.NodeMouseDoubleClick += EventType_NodeDoubleClick;

        toolboxPanel.Controls.Add(eventTreeView);
        toolboxPanel.Controls.Add(lblToolbox);

        return toolboxPanel;
    }

    private Panel CreateMainPanel()
    {
        var mainPanel = new Panel
        {
            Location = new Point(150, 0),
            Size = new Size(ClientSize.Width - 150, ClientSize.Height),
            Padding = new Padding(20),
            BackColor = Color.FromArgb(18, 18, 18),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoScroll = false
        };

        // Selected Events Label
        var lblSelectedEvents = new MaterialLabel
        {
            Text = "Selected Events",
            Height = 40,
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 14F),
            Padding = new Padding(0, 10, 0, 10)
        };

        // Selected Events ListView
        selectedEventsListView = new MaterialListView
        {
            Dock = DockStyle.Top,
            Height = 250,
            View = View.Details,
            FullRowSelect = true,
            BackColor = Color.FromArgb(38, 38, 38),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.None,
            OwnerDraw = true,
            GridLines = false
        };
        selectedEventsListView.Columns.Add("Events", -2);
        selectedEventsListView.DoubleClick += SelectedEvents_DoubleClick;

        // Adjust column width to fill the ListView
        selectedEventsListView.SizeChanged += SelectedEventsListView_SizeChanged;

        // Button Panel
        var buttonPanel = new Panel
        {
            Height = 50,
            Dock = DockStyle.Top,
            Padding = new Padding(0, 10, 0, 10),
            BackColor = Color.FromArgb(18, 18, 18)
        };

        var removeButton = new MaterialButton
        {
            Text = "Remove Selected",
            Height = 36,
            Width = 150,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(220, 53, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F),
            Left = 0
        };
        removeButton.Click += RemoveEvent_Click;

        var editButton = new MaterialButton
        {
            Text = "Edit Selected",
            Height = 36,
            Width = 150,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F),
            Left = 160
        };
        editButton.Click += EditEvent_Click;

        buttonPanel.Controls.Add(removeButton);
        buttonPanel.Controls.Add(editButton);

        // Configuration Label
        var lblConfiguration = new MaterialLabel
        {
            Text = "Event Configuration",
            Height = 40,
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 14F),
            Padding = new Padding(0, 10, 0, 10)
        };

        // Configuration Panel
        eventConfigPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(28, 28, 28),
            Padding = new Padding(15),
            AutoScroll = false
        };

        InitializeEventControls();

        // Create a FlowLayoutPanel for the buttons
        var buttonFlowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            Padding = new Padding(0, 10, 0, 0),
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.FromArgb(18, 18, 18)
        };

        // Apply Changes Button
        applyButton = new MaterialButton
        {
            Text = "Apply Changes",
            Height = 45,
            Width = 150,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(63, 81, 181),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F),
            Margin = new Padding(10, 0, 0, 0)
        };
        applyButton.Click += ApplyButton_Click;

        // Discard Changes Button
        var discardButton = new MaterialButton
        {
            Text = "Discard Changes",
            Height = 45,
            Width = 150,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(220, 53, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F),
            Margin = new Padding(0)
        };
        discardButton.Click += DiscardButton_Click;

        // Add buttons to the FlowLayoutPanel
        buttonFlowPanel.Controls.Add(applyButton);
        buttonFlowPanel.Controls.Add(discardButton);

        // Add controls to the mainPanel
        mainPanel.Controls.Add(eventConfigPanel);
        mainPanel.Controls.Add(lblConfiguration);
        mainPanel.Controls.Add(buttonPanel);
        mainPanel.Controls.Add(selectedEventsListView);
        mainPanel.Controls.Add(lblSelectedEvents);
        mainPanel.Controls.Add(buttonFlowPanel);

        return mainPanel;
    }
    private void InitializeEventControls()
    {
        mapSearchComboBox = new CustomMaterialStyleComboBox
        {
            Size = new Size(500, 30)
        };
        mapSearchComboBox.Items.AddRange(mapList().ToArray());

        portalSearchComboBox = new CustomMaterialStyleComboBox
        {
            Size = new Size(500, 30)
        };
        portalSearchComboBox.Items.AddRange(portalList.ToArray());

        finishTypeComboBox = new CustomMaterialStyleComboBox
        {
            Size = new Size(500, 30)
        };
        finishTypeComboBox.Items.AddRange(Enum.GetNames(typeof(TimeSpaceFinishType)));

        timeNumericUpDown = new ModernNumericUpDown
        {
            Size = new Size(200, 30),
            Minimum = 0,
            Maximum = 3600,
            Value = 0
        };
    }

    private void EventType_NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node == null || e.Node.Parent == null) return;
        var selectedType = e.Node.Text;
        ShowEventConfiguration(selectedType);
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

            var selectedIndex = selectedEventsListView.SelectedIndices[0];

            selectedEvents.RemoveAt(selectedIndex);
            selectedEventsListView.Items.RemoveAt(selectedIndex);

            ShowEventConfiguration(eventType, value);
        }
    }

    private void ShowEventConfiguration(string eventType, string currentValue = null)
    {
        eventConfigPanel.Controls.Clear();

        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(40, 40, 45)
        };
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        var label = new Label
        {
            Text = $"Configure {eventType}:",
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F)
        };

        Control configControl = null;

        switch (eventType)
        {
            case "TryStartTaskForMap":
                configControl = mapSearchComboBox;
                mapSearchComboBox.SelectedIndex = -1;
                if (currentValue != null)
                    mapSearchComboBox.SelectedItem = currentValue;
                break;

            case "OpenPortal":
                configControl = portalSearchComboBox;
                portalSearchComboBox.SelectedIndex = -1;
                if (currentValue != null)
                    portalSearchComboBox.SelectedItem = currentValue;
                break;

            case "FinishTimeSpace":
                configControl = finishTypeComboBox;
                finishTypeComboBox.SelectedIndex = -1;
                if (currentValue != null)
                    finishTypeComboBox.SelectedItem = currentValue.Replace("TimeSpaceFinishType.", "");
                break;

            case "SetTime":
            case "AddTime":
                configControl = timeNumericUpDown;
                timeNumericUpDown.Value = 0;
                if (currentValue != null && int.TryParse(currentValue, out int time))
                    timeNumericUpDown.Value = time;
                break;

            case "DespawnAllMobsInRoom":
                // No configuration needed; add the event directly
                var script = $"Event.DespawnAllMobsInRoom({currentMapName})";
                AddEventToList(script);
                return;
        }

        if (configControl != null)
        {
            // Store the current configuration control
            currentConfigControl = configControl;

            // Add controls to the layout
            layoutPanel.Controls.Add(label, 0, 0);
            layoutPanel.Controls.Add(configControl, 0, 1);

            // Add the "Add Event" button
            var addButton = new MaterialButton
            {
                Text = "Add Event",
                Height = 36,
                Width = 150,
                Type = MaterialButton.MaterialButtonType.Contained,
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F),
                Margin = new Padding(0, 10, 0, 0),
                Dock = DockStyle.Top
            };
            addButton.Click += (s, e) => AddConfiguredEvent(eventType);

            layoutPanel.Controls.Add(addButton, 0, 2);

            eventConfigPanel.Controls.Add(layoutPanel);
        }
    }

    private string GetConfigurationValue()
    {
        if (currentConfigControl is CustomMaterialStyleComboBox combo && combo.SelectedItem != null)
        {
            if (combo == finishTypeComboBox)
                return $"TimeSpaceFinishType.{combo.SelectedItem}";
            return combo.SelectedItem.ToString();
        }
        else if (currentConfigControl is ModernNumericUpDown numeric)
        {
            return numeric.Value.ToString();
        }
        return null;
    }

    private void AddEventToList(string script)
    {
        selectedEvents.Add(script);
        var item = new ListViewItem(script);
        selectedEventsListView.Items.Add(item);
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
        selectedEventsListView.Items.Clear();
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
        // Material Skin colors
        var materialSkinManager = MaterialSkinManager.Instance;
        materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
        materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey900, Primary.Grey900, Accent.Amber700, TextShade.WHITE);

        foreach (Control control in Controls)
        {
            StyleControl(control);
        }
    }

    private void StyleControl(Control control)
    {
        if (control is CustomMaterialStyleComboBox combo)
        {
            combo.BackColor = Color.FromArgb(38, 38, 38);
            combo.ForeColor = Color.White;
            combo.Font = new Font("Segoe UI", 11F);
            combo.FlatStyle = FlatStyle.Flat;
        }
        else if (control is TextBox txt)
        {
            txt.BackColor = Color.FromArgb(38, 38, 38);
            txt.ForeColor = Color.White;
            txt.Font = new Font("Segoe UI", 11F);
            txt.BorderStyle = BorderStyle.FixedSingle;
        }
        else if (control is Panel panel)
        {
            panel.BackColor = Color.FromArgb(28, 28, 28);
        }

        foreach (Control child in control.Controls)
        {
            StyleControl(child);
        }
    }

    private void SelectedEventsListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
    {
        using (var headerFont = new Font("Segoe UI", 10F, FontStyle.Bold))
        using (var backgroundBrush = new SolidBrush(Color.FromArgb(40, 40, 45)))
        using (var textBrush = new SolidBrush(Color.White))
        {
            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            e.Graphics.DrawString(e.Header.Text, headerFont, textBrush, e.Bounds);
        }
    }

    private void SelectedEventsListView_DrawItem(object sender, DrawListViewItemEventArgs e)
    {
        e.DrawDefault = true;
    }

    private void SelectedEventsListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
        e.DrawDefault = true;
    }
    private void DiscardButton_Click(object sender, EventArgs e)
    {
        // Revert to the initial events list  
        selectedEvents = new List<string>(initialEvents);
        LoadExistingEvents();
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
    private void AddConfiguredEvent(string eventType)
    {
        string value = GetConfigurationValue();

        if (!string.IsNullOrEmpty(value))
        {
            var script = $"Event.{eventType}({value})";
            AddEventToList(script);
            eventConfigPanel.Controls.Clear();
        }
        else
        {
            MessageBox.Show("Please complete the configuration before adding the event.", "Incomplete Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // Adjust the column width to fill the ListView
    private void SelectedEventsListView_SizeChanged(object sender, EventArgs e)
    {
        if (selectedEventsListView.Columns.Count > 0)
        {
            selectedEventsListView.Columns[0].Width = selectedEventsListView.ClientSize.Width;
        }
    }
}
