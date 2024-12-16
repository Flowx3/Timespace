public class ModernDataGridView : DataGridView
{
    private readonly Color _selectionColor = Color.FromArgb(60, 60, 60);
    private readonly Color _backgroundColor = Color.FromArgb(28, 28, 28);
    private readonly Color _gridColor = Color.FromArgb(70, 70, 70);
    private readonly Color _headerColor = Color.FromArgb(35, 35, 35);
    private readonly Color _hoverColor = Color.FromArgb(45, 45, 45);

    public ModernDataGridView()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

        // Basic properties
        BackgroundColor = _backgroundColor;
        ForeColor = Color.White;
        GridColor = _gridColor;
        BorderStyle = BorderStyle.Fixed3D; // Changed to prevent border glitching
        CellBorderStyle = DataGridViewCellBorderStyle.Single;
        EnableHeadersVisualStyles = false;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        RowHeadersVisible = false; // Disable row headers to prevent glitching
        AutoGenerateColumns = false;

        // Default styles
        DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = _backgroundColor,
            ForeColor = Color.White,
            SelectionBackColor = _selectionColor,
            SelectionForeColor = Color.White,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(5),
            Alignment = DataGridViewContentAlignment.MiddleLeft
        };

        // Column header styles
        ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = _headerColor,
            ForeColor = Color.White,
            SelectionBackColor = _headerColor,
            SelectionForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F),
            Padding = new Padding(5),
            Alignment = DataGridViewContentAlignment.MiddleLeft
        };

        // Events
        CellMouseEnter += ModernDataGridView_CellMouseEnter;
        CellMouseLeave += ModernDataGridView_CellMouseLeave;
        RowsAdded += ModernDataGridView_RowsAdded;
        CellFormatting += ModernDataGridView_CellFormatting;
    }

    private void ModernDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            var row = Rows[e.RowIndex];
            if (row.DefaultCellStyle.BackColor != _backgroundColor)
            {
                row.DefaultCellStyle = DefaultCellStyle.Clone();
            }
        }
    }

    private void ModernDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
        BeginInvoke(new Action(() =>
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                if (i >= 0 && i < Rows.Count)
                {
                    var row = Rows[i];
                    row.DefaultCellStyle = DefaultCellStyle.Clone();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style = DefaultCellStyle.Clone();
                    }
                }
            }
            Refresh();
        }));
    }

    private void ModernDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < Rows.Count)
        {
            var row = Rows[e.RowIndex];
            row.DefaultCellStyle.BackColor = _hoverColor;
            row.DefaultCellStyle.SelectionBackColor = _selectionColor;
        }
    }

    private void ModernDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < Rows.Count)
        {
            var row = Rows[e.RowIndex];
            row.DefaultCellStyle.BackColor = _backgroundColor;
            row.DefaultCellStyle.SelectionBackColor = _selectionColor;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Force refresh of styles
        foreach (DataGridViewRow row in Rows)
        {
            if (row.DefaultCellStyle.BackColor != _backgroundColor && !row.Selected)
            {
                row.DefaultCellStyle = DefaultCellStyle.Clone();
            }
        }
    }
}