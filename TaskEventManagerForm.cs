//public class TaskEventManagerForm : Form
//{
//    private ListView eventListView;
//    private Button addEventButton;
//    private Button removeEventButton;
//    private Button saveButton;
//    private string mapName;
//    private string eventType;
//    private List<string> currentEvents;

//    public List<string> Events => currentEvents;

//    public TaskEventManagerForm(string mapName, string eventType, List<string> existingEvents)
//    {
//        this.mapName = mapName;
//        this.eventType = eventType;
//        this.currentEvents = new List<string>(existingEvents);
//        InitializeComponents();
//        LoadExistingEvents();
//    }

//    private void InitializeComponents()
//    {
//        Text = $"{eventType} Event Manager";
//        Size = new Size(800, 500);
//        StartPosition = FormStartPosition.CenterScreen;
//        FormBorderStyle = FormBorderStyle.FixedDialog;
//        MaximizeBox = false;
//        BackColor = Color.FromArgb(30, 30, 35);

//        eventListView = new ListView
//        {
//            Location = new Point(20, 20),
//            Size = new Size(740, 350),
//            View = View.Details,
//            FullRowSelect = true,
//            BackColor = Color.FromArgb(45, 45, 50),
//            ForeColor = Color.White
//        };
//        eventListView.Columns.Add("Events", -2);

//        addEventButton = new Button
//        {
//            Text = "Add Event",
//            Location = new Point(20, 390),
//            Size = new Size(120, 35),
//            BackColor = Color.FromArgb(70, 130, 180),
//            ForeColor = Color.White,
//            FlatStyle = FlatStyle.Flat
//        };
//        addEventButton.Click += AddEvent_Click;

//        removeEventButton = new Button
//        {
//            Text = "Remove Event",
//            Location = new Point(160, 390),
//            Size = new Size(120, 35),
//            BackColor = Color.FromArgb(70, 130, 180),
//            ForeColor = Color.White,
//            FlatStyle = FlatStyle.Flat
//        };
//        removeEventButton.Click += RemoveEvent_Click;

//        saveButton = new Button
//        {
//            Text = "Save",
//            Location = new Point(640, 390),
//            Size = new Size(120, 35),
//            BackColor = Color.FromArgb(70, 130, 180),
//            ForeColor = Color.White,
//            FlatStyle = FlatStyle.Flat
//        };
//        saveButton.Click += SaveButton_Click;

//        Controls.AddRange(new Control[] { eventListView, addEventButton, removeEventButton, saveButton });
//    }

//    private void LoadExistingEvents()
//    {
//        foreach (var evt in currentEvents)
//        {
//            eventListView.Items.Add(evt);
//        }
//    }

//    private void AddEvent_Click(object sender, EventArgs e)
//    {
//        using (var eventManager = new EventManagerForm(mapName, new List<string>(), new List<string>()))
//        {
//            if (eventManager.ShowDialog() == DialogResult.OK && eventManager.Tag is string eventScript)
//            {
//                eventListView.Items.Add(eventScript);
//                currentEvents.Add(eventScript);
//            }
//        }
//    }

//    private void RemoveEvent_Click(object sender, EventArgs e)
//    {
//        if (eventListView.SelectedItems.Count > 0)
//        {
//            var selectedIndex = eventListView.SelectedIndices[0];
//            currentEvents.RemoveAt(selectedIndex);
//            eventListView.Items.RemoveAt(selectedIndex);
//        }
//    }

//    private void SaveButton_Click(object sender, EventArgs e)
//    {
//        DialogResult = DialogResult.OK;
//        Close();
//    }
//}