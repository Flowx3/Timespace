using MaterialSkin.Controls;
using MaterialSkin;
using System.Drawing;
using System.Windows.Forms;
using TimeSpace;

public class MonsterAttributeForm : MaterialForm
{
    private MaterialListView attributeList;
    private MaterialButton btnAdd;
    private MaterialButton btnRemove;
    private MaterialButton btnApply;
    private MaterialButton btnDiscard;
    private CustomMaterialStyleComboBox attributeCombo;
    private MaterialTextBox2 valueBox;
    private Dictionary<string, object> selectedAttributes = new Dictionary<string, object>();
    private readonly Dictionary<string, object> initialAttributes;

    private readonly string[] availableAttributes = new[]
    {
        "SpawnAfterMobsKilled",
        "WithCustomLevel",
        "SpawnAfterTaskStart",
        "OnThreeFourthsHp",
        "OnHalfHp",
        "OnQuarterHp",
        "OnDeath"
    };

    private readonly string[] noValueAttributes = new[]
    {
        "SpawnAfterTaskStart",
        "OnThreeFourthsHp",
        "OnHalfHp",
        "OnQuarterHp",
        "OnDeath"
    };

    public Dictionary<string, object> SelectedAttributes => selectedAttributes;

    public MonsterAttributeForm(Dictionary<string, object> existingAttributes = null)
    {
        initialAttributes = existingAttributes ?? new Dictionary<string, object>();
        selectedAttributes = new Dictionary<string, object>(initialAttributes);
        InitializeComponents();
        StyleComponents();
        UpdateListView();
    }

    private void InitializeComponents()
    {
        // Form settings
        Text = "Monster Attributes";
        Size = new Size(1200, 700);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        BackColor = Color.FromArgb(18, 18, 18);

        // Main panel
        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(18, 18, 18),
            AutoScroll = false
        };

        // Attributes List Label
        var lblAttributes = new MaterialLabel
        {
            Text = "Monster Attributes",
            Height = 40,
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 14F),
            Padding = new Padding(0, 10, 0, 10)
        };

        // Attributes ListView
        attributeList = new MaterialListView
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
            GridLines = false,
            Scrollable = false
        };
        attributeList.Columns.Add("Attribute", 250);
        attributeList.Columns.Add("Value", 1500);  // Increased width for Value column
        attributeList.DrawColumnHeader += AttributeList_DrawColumnHeader;
        attributeList.DrawItem += AttributeList_DrawItem;
        attributeList.DrawSubItem += AttributeList_DrawSubItem;

        // Configuration Panel
        var configPanel = new Panel
        {
            Height = 120,
            Dock = DockStyle.Top,
            Padding = new Padding(0, 20, 0, 0),
            BackColor = Color.FromArgb(28, 28, 28)
        };

        // Attribute ComboBox
        attributeCombo = new CustomMaterialStyleComboBox
        {
            Location = new Point(10, 20),
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(38, 38, 38),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F)
        };
        attributeCombo.Items.AddRange(availableAttributes);
        attributeCombo.SelectedIndexChanged += AttributeCombo_SelectedIndexChanged;

        // Value TextBox
        valueBox = new MaterialTextBox2
        {
            Location = new Point(320, 20),
            Width = 300,
            Font = new Font("Segoe UI", 11F)
        };

        // Add Button
        btnAdd = new MaterialButton
        {
            Text = "Add Attribute",
            Location = new Point(630, 20),
            Width = 120,
            Height = 36,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(63, 81, 181),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F)
        };
        btnAdd.Click += BtnAdd_Click;

        // Remove Button
        btnRemove = new MaterialButton
        {
            Text = "Remove",
            Location = new Point(10, 70),
            Width = 120,
            Height = 36,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(220, 53, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F)
        };
        btnRemove.Click += BtnRemove_Click;

        // Button Panel
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            Padding = new Padding(0, 10, 0, 0),
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.FromArgb(18, 18, 18)
        };

        // Apply Button
        btnApply = new MaterialButton
        {
            Text = "Apply Changes",
            Height = 45,
            Width = 150,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(63, 81, 181),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F),
            Margin = new Padding(10, 0, 0, 0),
            DialogResult = DialogResult.OK
        };

        // Discard Button
        btnDiscard = new MaterialButton
        {
            Text = "Discard Changes",
            Height = 45,
            Width = 150,
            Type = MaterialButton.MaterialButtonType.Contained,
            BackColor = Color.FromArgb(220, 53, 69),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F),
            Margin = new Padding(0),
            DialogResult = DialogResult.Cancel
        };
        btnDiscard.Click += BtnDiscard_Click;

        // Add controls to panels
        configPanel.Controls.AddRange(new Control[] { attributeCombo, valueBox, btnAdd, btnRemove });
        buttonPanel.Controls.AddRange(new Control[] { btnApply, btnDiscard });
        mainPanel.Controls.AddRange(new Control[] { buttonPanel, configPanel, attributeList, lblAttributes });

        Controls.Add(mainPanel);
    }

    private void StyleComponents()
    {
        // Material Skin colors
        var materialSkinManager = MaterialSkinManager.Instance;
        materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
        materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey900, Primary.Grey900, Accent.Amber700, TextShade.WHITE);
    }

    private void AttributeList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
    {
        using (var backgroundBrush = new SolidBrush(Color.FromArgb(38, 38, 38)))
        using (var textBrush = new SolidBrush(Color.White))
        using (var headerFont = new Font("Segoe UI", 11F, FontStyle.Bold))
        {
            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            e.Graphics.DrawString(e.Header.Text, headerFont, textBrush,
                new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 4, e.Bounds.Width - 8, e.Bounds.Height - 8),
                new StringFormat { LineAlignment = StringAlignment.Center });
        }
    }

    private void AttributeList_DrawItem(object sender, DrawListViewItemEventArgs e)
    {
        e.DrawDefault = true;
    }

    private void AttributeList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
        e.DrawDefault = true;
    }

    private void AttributeCombo_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedAttribute = attributeCombo.SelectedItem?.ToString();
        valueBox.Enabled = !noValueAttributes.Contains(selectedAttribute);
        valueBox.Text = string.Empty;
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (attributeCombo.SelectedItem == null) return;

        string attr = attributeCombo.SelectedItem.ToString();

        if (noValueAttributes.Contains(attr))
        {
            if (attr == "SpawnAfterTaskStart")
            {
                // For SpawnAfterTaskStart, just add it without value
                selectedAttributes[attr] = "";
                UpdateListView();
            }
            else
            {
                // For event attributes, open EventManager
                using (var eventManager = new EventManagerForm(string.Empty, () => new List<string>(), new List<string>()))
                {
                    if (eventManager.ShowDialog() == DialogResult.OK)
                    {
                        selectedAttributes[attr] = eventManager.Events;
                        UpdateListView();
                    }
                }
            }
        }
        else
        {
            string value = valueBox.Text;
            if (!string.IsNullOrWhiteSpace(value))
            {
                selectedAttributes[attr] = value;
                UpdateListView();
            }
        }
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

    private void BtnDiscard_Click(object sender, EventArgs e)
    {
        selectedAttributes = new Dictionary<string, object>(initialAttributes);
        UpdateListView();
    }

    private void UpdateListView()
    {
        attributeList.Items.Clear();
        foreach (var attr in selectedAttributes)
        {
            var item = new ListViewItem(attr.Key);
            if (attr.Value is List<string> events)
            {
                item.SubItems.Add(string.Join(", ", events));
            }
            else if (attr.Key == "SpawnAfterTaskStart")
            {
                item.SubItems.Add(""); // Empty value for SpawnAfterTaskStart
            }
            else
            {
                item.SubItems.Add(attr.Value.ToString());
            }
            attributeList.Items.Add(item);
        }
    }
}