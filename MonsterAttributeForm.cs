public class MonsterAttributeForm : Form
{
    private Dictionary<string, string> selectedAttributes = new Dictionary<string, string>();
    private ListView attributeList;
    private Button btnAdd;
    private Button btnRemove;
    private Button btnOK;
    private ComboBox attributeCombo;
    private TextBox valueBox;

    private readonly string[] availableAttributes = new[]
    {
        "SpawnAfterMobsKilled",
        "WithCustomLevel",
        "SpawnAfterTaskStart",
        "OnThreeFourthsHP",
        "OnHalfHp",
        "OnQuarterHp"
    };

    public Dictionary<string, string> SelectedAttributes => selectedAttributes;

    public MonsterAttributeForm(Dictionary<string, string> existingAttributes = null)
    {
        InitializeComponent();
        if (existingAttributes != null)
        {
            foreach (var attr in existingAttributes)
            {
                selectedAttributes[attr.Key] = attr.Value;
            }
            // Update the ListView to show existing attributes
            UpdateListView();
        }
    }

    private void InitializeComponent()
    {
        this.Size = new Size(400, 500);
        this.Text = "Monster Attributes";

        attributeList = new ListView
        {
            Location = new Point(10, 10),
            Size = new Size(360, 200),
            View = View.Details
        };
        attributeList.Columns.Add("Attribute", 180);
        attributeList.Columns.Add("Value", 160);

        attributeCombo = new ComboBox
        {
            Location = new Point(10, 220),
            Width = 180,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        attributeCombo.Items.AddRange(availableAttributes);

        valueBox = new TextBox
        {
            Location = new Point(200, 220),
            Width = 170
        };

        btnAdd = new Button
        {
            Text = "Add",
            Location = new Point(10, 250),
            Width = 75
        };
        btnAdd.Click += BtnAdd_Click;

        btnRemove = new Button
        {
            Text = "Remove",
            Location = new Point(95, 250),
            Width = 75
        };
        btnRemove.Click += BtnRemove_Click;

        btnOK = new Button
        {
            Text = "OK",
            Location = new Point(295, 250),
            Width = 75,
            DialogResult = DialogResult.OK
        };

        Controls.AddRange(new Control[] {
            attributeList, attributeCombo, valueBox,
            btnAdd, btnRemove, btnOK
        });
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (attributeCombo.SelectedItem == null) return;

        string attr = attributeCombo.SelectedItem.ToString();
        string value = valueBox.Text;

        selectedAttributes[attr] = value;
        UpdateListView();
    }

    private void BtnRemove_Click(object sender, EventArgs e)
    {
        if (attributeList.SelectedItems.Count > 0)
        {
            string attr = attributeList.SelectedItems[0].Text;
            selectedAttributes.Remove(attr);
            UpdateListView();
        }
    }

    private void UpdateListView()
    {
        attributeList.Items.Clear();
        foreach (var attr in selectedAttributes)
        {
            var item = new ListViewItem(attr.Key);
            item.SubItems.Add(attr.Value);
            attributeList.Items.Add(item);
        }
    }
}