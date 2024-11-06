public class GridSelectorForm : Form
{
    private const int CELL_SIZE = 40;
    private const int GRID_WIDTH = 11;
    private const int GRID_HEIGHT = 11;
    private List<Point> takenPositions;
    private Point? selectedPosition;
    private Button[,] gridButtons;
    private Button confirmButton;
    private Label coordinatesLabel;
    private Panel gridPanel;

    public Point? SelectedCoordinates { get; private set; }

    public GridSelectorForm(List<Point> existingPositions)
    {
        takenPositions = existingPositions ?? new List<Point>();
        InitializeGridForm();
        CreateGridButtons();
    }

    private void InitializeGridForm()
    {
        this.Text = "Grid Position Selector";
        this.Size = new Size(CELL_SIZE * (GRID_WIDTH + 1) + 100, CELL_SIZE * (GRID_HEIGHT + 2) + 100);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Initialize grid panel
        gridPanel = new Panel
        {
            Location = new Point(40, 40),
            Size = new Size(CELL_SIZE * (GRID_WIDTH + 1), CELL_SIZE * (GRID_HEIGHT + 1)),
            BorderStyle = BorderStyle.FixedSingle
        };
        this.Controls.Add(gridPanel);

        // Create row headers
        for (int i = 0; i <= GRID_HEIGHT; i++)
        {
            Label rowLabel = new Label
            {
                Text = $"[{i}]",
                Location = new Point(5, 40 + i * CELL_SIZE),
                Size = new Size(30, CELL_SIZE),
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(rowLabel);
        }

        // Create column headers
        for (int j = 0; j <= GRID_WIDTH; j++)
        {
            Label colLabel = new Label
            {
                Text = $"[{j}]",
                Location = new Point(40 + j * CELL_SIZE, 5),
                Size = new Size(CELL_SIZE, 30),
                TextAlign = ContentAlignment.BottomCenter
            };
            this.Controls.Add(colLabel);
        }

        // Create coordinates label
        coordinatesLabel = new Label
        {
            Location = new Point(40, CELL_SIZE * (GRID_HEIGHT + 1) + 50),
            Size = new Size(200, 30),
            Text = "Selected: None"
        };
        this.Controls.Add(coordinatesLabel);

        // Create confirm button
        confirmButton = new Button
        {
            Location = new Point(250, CELL_SIZE * (GRID_HEIGHT + 1) + 50),
            Size = new Size(100, 30),
            Text = "Confirm",
            Enabled = false
        };
        confirmButton.Click += ConfirmButton_Click;
        this.Controls.Add(confirmButton);
    }

    private void CreateGridButtons()
    {
        gridButtons = new Button[GRID_WIDTH + 1, GRID_HEIGHT + 1];
        for (int i = 0; i <= GRID_HEIGHT; i++)
        {
            for (int j = 0; j <= GRID_WIDTH; j++)
            {
                Button btn = new Button
                {
                    Location = new Point(j * CELL_SIZE, i * CELL_SIZE),
                    Size = new Size(CELL_SIZE, CELL_SIZE),
                    Text = "  ",
                    Tag = new Point(j, i)
                };

                if (takenPositions.Contains(new Point(j, i)))
                {
                    btn.Text = "OO";
                    btn.Enabled = false;
                }

                btn.Click += GridButton_Click;
                gridButtons[j, i] = btn;
                gridPanel.Controls.Add(btn);
            }
        }
    }

    private void GridButton_Click(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;
        Point coordinates = (Point)clickedButton.Tag;

        // Clear previous selection
        if (selectedPosition.HasValue)
        {
            Point prev = selectedPosition.Value;
            if (!takenPositions.Contains(prev))
            {
                gridButtons[prev.X, prev.Y].Text = "  ";
            }
        }

        // Update new selection
        selectedPosition = coordinates;
        clickedButton.Text = "XX";
        coordinatesLabel.Text = $"Selected: ({coordinates.X}, {coordinates.Y})";
        confirmButton.Enabled = true;
    }

    private void ConfirmButton_Click(object sender, EventArgs e)
    {
        if (selectedPosition.HasValue)
        {
            SelectedCoordinates = selectedPosition.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}