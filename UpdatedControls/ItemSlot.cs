using System.Diagnostics;
using TimeSpace;

public class ItemSlot : PictureBox
{
    private readonly ItemSearchManager _searchManager;
    private static ItemSearchManager? _sharedSearchManager;
    private ToolTip tooltip;
    private readonly Config _config;
    public ItemData? Item { get; private set; }
    public int Quantity { get; private set; }
    private Label quantityLabel;
    private const int SLOT_SIZE = 72;

    public ItemSlot(Config config)
    {
        _config = config;

        if (_sharedSearchManager == null)
        {
            _sharedSearchManager = new ItemSearchManager(config);
        }
        _searchManager = _sharedSearchManager;

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.BorderStyle = BorderStyle.FixedSingle;
        this.SizeMode = PictureBoxSizeMode.CenterImage;
        this.Size = new Size(SLOT_SIZE, SLOT_SIZE);
        this.BackColor = Color.FromArgb(20, 20, 20);

        quantityLabel = new Label
        {
            AutoSize = true,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            Font = new Font("Arial", 8f, FontStyle.Bold),
            Parent = this,
            Visible = false
        };

        tooltip = new ToolTip
        {
            InitialDelay = 200,
            ShowAlways = true
        };

        this.Click += ItemSlot_Click;
        this.MouseHover += ItemSlot_MouseHover;
    }

    public void SetItem(ItemData? item, int quantity = 1)
    {
        Item = item;
        Quantity = quantity;

        if (item != null)
        {
            try
            {
                var icon = _searchManager.GetCachedIcon(item.IconIndex);
                this.Image = new Bitmap(icon); // Create a new instance to prevent sharing

                quantityLabel.Text = quantity > 1 ? quantity.ToString() : "";
                quantityLabel.Visible = quantity > 1;

                // Update label position after text is set to ensure correct width calculation
                this.BeginInvoke(new Action(() =>
                {
                    quantityLabel.Location = new Point(
                        this.Width - quantityLabel.Width - 2,
                        this.Height - quantityLabel.Height - 2
                    );
                    quantityLabel.BringToFront();
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting item icon: {ex.Message}");
                this.Image = null;
            }
        }
        else
        {
            this.Image = null;
            quantityLabel.Visible = false;
        }
    }
    private void ItemSlot_Click(object? sender, EventArgs e)
    {
        try
        {
            Debug.WriteLine("ItemSlot_Click started");

            // Since we're already on the UI thread, we don't need to create a new thread
            if (this.IsDisposed || !this.IsHandleCreated)
            {
                Debug.WriteLine("Control is disposed or handle not created");
                return;
            }

            Debug.WriteLine("Creating search form");
            var searchForm = new ItemSearchForm(_searchManager);

            Debug.WriteLine("Showing search form");
            var result = searchForm.ShowDialog(this);
            Debug.WriteLine($"Dialog result: {result}");

            if (result == DialogResult.OK && searchForm.SelectedItem != null)
            {
                SetItem(searchForm.SelectedItem, searchForm.SelectedQuantity);
            }

            searchForm.Dispose();
            Debug.WriteLine("Search form disposed");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in ItemSlot_Click: {ex}");
            MessageBox.Show("Failed to open item search form.", "Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ItemSlot_MouseHover(object? sender, EventArgs e)
    {
        if (Item != null)
        {
            string tooltipText = $"ID: {Item.Vnum}\nName: {Item.TranslatedName ?? Item.Name}";
            tooltip.SetToolTip(this, tooltipText);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            tooltip.Dispose();
            quantityLabel.Dispose();
            if (this.Image != null)
            {
                this.Image.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}