using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace TimeSpace.ItemSearchSystem
{
    public partial class ItemSearchForm : Form
    {
        private ModernTextBox searchBox;
        private ListView itemListView;
        private ModernButton selectButton;
        private ModernNumericUpDown quantityInput;
        private readonly ItemSearchManager searchManager;
        private List<ItemData> allItems;
        private List<ItemData> filteredItems;
        private const int ICON_SIZE = 32;
        private static ImageList itemImageList = new ImageList { ImageSize = new Size(ICON_SIZE, ICON_SIZE), ColorDepth = ColorDepth.Depth32Bit };
        private System.Windows.Forms.Timer searchTimer;
        private const int SEARCH_DELAY = 500;
        private bool isFirstLoad = true;
        private static List<ListViewItem>? cachedListViewItems;
        private CancellationTokenSource? loadingCancellation;
        private static readonly SemaphoreSlim loadingSemaphore = new SemaphoreSlim(1, 1);
        private static bool isBackgroundLoadingComplete = false;
        private bool isLoading = false;
        public ItemData? SelectedItem { get; private set; }
        public int SelectedQuantity { get; private set; }

        public ItemSearchForm(ItemSearchManager manager)
        {
            searchManager = manager;
            allItems = new List<ItemData>();
            filteredItems = new List<ItemData>();

            InitializeComponent();
            InitializeSearchTimer();
            ApplyDarkTheme();

            Shown += ItemSearchForm_Shown;
            FormClosing += ItemSearchForm_FormClosing;
        }

        private void ApplyDarkTheme()
        {
            // Form  
            BackColor = Color.FromArgb(28, 28, 28);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Search box  
            searchBox.BackColor = Color.FromArgb(45, 45, 48);
            searchBox.ForeColor = Color.White;

            // Quantity input  
            quantityInput.ForeColor = Color.White;

            // ListView  
            itemListView.BackColor = Color.FromArgb(28, 28, 28);
            itemListView.ForeColor = Color.White;
            itemListView.GridLines = true;

            // Select button  
            selectButton.BackColor = Color.FromArgb(45, 45, 48);
            selectButton.ForeColor = Color.White;
        }

        private void InitializeComponent()
        {
            Size = new Size(400, 500);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Item Search";

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            searchBox = new ModernTextBox
            {
                Location = new Point(5, 5),
                Width = 280,
                Height = 23,
                PlaceholderText = "Search by ID or name..."
            };

            quantityInput = new ModernNumericUpDown
            {
                Location = new Point(290, 5),
                Width = 80,
                Minimum = 1,
                Maximum = 999,
                Value = 1
            };

            itemListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };
            itemListView.Columns.Add("Icon", 40);
            itemListView.Columns.Add("ID", 70);
            itemListView.Columns.Add("Name", 250);

            itemListView.SmallImageList = itemImageList;

            selectButton = new ModernButton
            {
                Dock = DockStyle.Bottom,
                Text = "Select",
                Height = 30
            };

            searchBox.TextChanged += SearchBox_TextChanged_Delayed;
            itemListView.DoubleClick += ItemListView_DoubleClick;
            selectButton.Click += SelectButton_Click;

            topPanel.Controls.Add(searchBox);
            topPanel.Controls.Add(quantityInput);
            Controls.Add(itemListView);
            Controls.Add(topPanel);
            Controls.Add(selectButton);
        }

        private void InitializeSearchTimer()
        {
            searchTimer = new System.Windows.Forms.Timer
            {
                Interval = SEARCH_DELAY
            };
            searchTimer.Tick += SearchTimer_Tick;
        }

        private async void ItemSearchForm_Shown(object sender, EventArgs e)
        {
            if (isFirstLoad)
            {
                await LoadItemsAsync();
                isFirstLoad = false;
            }
            else if (cachedListViewItems != null)
            {
                LoadCachedItems();
            }

            if (!isBackgroundLoadingComplete && !isLoading)
            {
                StartBackgroundLoading();
            }
        }

        private void LoadCachedItems()
        {
            if (cachedListViewItems == null) return;

            itemListView.BeginUpdate();
            try
            {
                itemListView.Items.Clear();
                itemListView.Items.AddRange(cachedListViewItems.ToArray());
            }
            finally
            {
                itemListView.EndUpdate();
            }
        }

        private void StartBackgroundLoading()
        {
            loadingCancellation = new CancellationTokenSource();
            var token = loadingCancellation.Token;

            Task.Run(async () =>
            {
                try
                {
                    await LoadIconsInBackgroundAsync(token);
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Background loading cancelled");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in background loading: {ex.Message}");
                }
            }, token);
        }

        private async Task LoadIconsInBackgroundAsync(CancellationToken token)
        {
            if (isBackgroundLoadingComplete) return;

            await loadingSemaphore.WaitAsync(token);
            try
            {
                isLoading = true;
                int batchSize = 50; // Process icons in batches  
                var iconBatch = new Dictionary<string, Image>();

                for (int i = 0; i < allItems.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var item = allItems[i];
                    string imageKey = item.Vnum.ToString();

                    if (!itemImageList.Images.ContainsKey(imageKey))
                    {
                        var icon = searchManager.GetCachedIcon(item.IconIndex);
                        iconBatch[imageKey] = icon;

                        // When batch is full or it's the last item, update UI  
                        if (iconBatch.Count >= batchSize || i == allItems.Count - 1)
                        {
                            await UpdateImageListAsync(iconBatch);
                            iconBatch.Clear();
                        }
                    }
                }

                isBackgroundLoadingComplete = true;
            }
            finally
            {
                isLoading = false;
                loadingSemaphore.Release();
            }
        }

        private async Task UpdateImageListAsync(Dictionary<string, Image> iconBatch)
        {
            if (IsDisposed) return;

            try
            {
                await InvokeAsync(() =>
                {
                    foreach (var kvp in iconBatch)
                    {
                        if (!itemImageList.Images.ContainsKey(kvp.Key))
                        {
                            itemImageList.Images.Add(kvp.Key, kvp.Value);
                        }
                    }
                    // Update ListView to refresh icons  
                    foreach (ListViewItem item in itemListView.Items)
                    {
                        if (iconBatch.ContainsKey(item.ImageKey))
                        {
                            item.ImageIndex = itemImageList.Images.IndexOfKey(item.ImageKey);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating image list: {ex.Message}");
            }
        }

        private async Task LoadItemsAsync()
        {
            try
            {
                UseWaitCursor = true;
                allItems = searchManager.GetAllItems();
                filteredItems = new List<ItemData>(allItems);

                if (cachedListViewItems != null)
                {
                    LoadCachedItems();
                    UseWaitCursor = false;
                    return;
                }

                await Task.Run(() =>
                {
                    var newItems = new List<ListViewItem>();
                    foreach (var item in filteredItems)
                    {
                        string imageKey = item.Vnum.ToString();
                        var lvi = new ListViewItem("") { ImageKey = imageKey };
                        lvi.SubItems.Add(item.Vnum.ToString());
                        lvi.SubItems.Add(item.TranslatedName ?? item.Name);
                        lvi.Tag = item;
                        newItems.Add(lvi);
                    }
                    cachedListViewItems = newItems;

                    InvokeAsync(() =>
                    {
                        itemListView.BeginUpdate();
                        try
                        {
                            itemListView.Items.Clear();
                            itemListView.Items.AddRange(newItems.ToArray());
                        }
                        finally
                        {
                            itemListView.EndUpdate();
                            UseWaitCursor = false;
                        }
                    });
                });

                // Start background loading of icons  
                StartBackgroundLoading();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading items: {ex.Message}");
                MessageBox.Show($"Error loading items: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            loadingCancellation?.Cancel();
            loadingCancellation?.Dispose();
        }

        private void SearchBox_TextChanged_Delayed(object sender, EventArgs e)
        {
            searchTimer.Stop();
            searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            PerformSearch();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchBox.Text.Trim().ToLower();

            itemListView.BeginUpdate();
            try
            {
                itemListView.Items.Clear();

                var searchResults = allItems.Where(item =>
                    item.Vnum.ToString().Contains(searchText) ||
                    (item.Name?.ToLower().Contains(searchText) ?? false) ||
                    (item.TranslatedName?.ToLower().Contains(searchText) ?? false));
                foreach (var item in searchResults)
                {
                    string imageKey = item.Vnum.ToString();
                    var lvi = new ListViewItem("") { ImageKey = imageKey };
                    lvi.SubItems.Add(item.Vnum.ToString());
                    lvi.SubItems.Add(item.TranslatedName ?? item.Name);
                    lvi.Tag = item;
                    itemListView.Items.Add(lvi);
                }
            }
            finally
            {
                itemListView.EndUpdate();
            }
        }

        private void PerformSearch()
        {
            string searchText = searchBox.Text.Trim().ToLower();

            itemListView.BeginUpdate();
            try
            {
                itemListView.Items.Clear();

                if (cachedListViewItems != null)
                {
                    var matchingItems = cachedListViewItems.Where(item =>
                    {
                        var itemData = item.Tag as ItemData;
                        return itemData != null &&
                               (itemData.Vnum.ToString().Contains(searchText) ||
                                (itemData.Name?.ToLower().Contains(searchText) ?? false) ||
                                (itemData.TranslatedName?.ToLower().Contains(searchText) ?? false));
                    }).ToList();

                    itemListView.Items.AddRange(matchingItems.ToArray());
                }
            }
            finally
            {
                itemListView.EndUpdate();
            }
        }

        private void SelectItem()
        {
            if (itemListView.SelectedItems.Count > 0)
            {
                var selectedItem = itemListView.SelectedItems[0];
                SelectedItem = selectedItem.Tag as ItemData;
                SelectedQuantity = (int)quantityInput.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ItemListView_DoubleClick(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            SelectItem();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            itemListView.Items.Clear();
        }

        private async Task InvokeAsync(Action action)
        {
            try
            {
                if (IsDisposed) return;
                if (InvokeRequired)
                {
                    await Task.Factory.FromAsync(
                        BeginInvoke(action),
                        EndInvoke);
                }
                else
                {
                    action();
                }
            }
            catch (ObjectDisposedException)
            {
                // Handle the case where the control is disposed during async operation  
            }
        }
    }
}