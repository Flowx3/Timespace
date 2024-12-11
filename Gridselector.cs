public class GridSelectorForm : Form
{
    private const int CELL_SIZE = 40;
    private const int GRID_WIDTH = 11;
    private const int GRID_HEIGHT = 11;
    private List<Point> takenPositions;
    private Point? selectedPosition;
    private Point? currentPosition;
    private Button[,] gridButtons;
    private Button confirmButton;
    private Label coordinatesLabel;
    private Panel gridPanel;

    public Point? SelectedCoordinates { get; private set; }

    public GridSelectorForm(List<Point> existingPositions, Point? currentPos = null)
    {
        takenPositions = existingPositions ?? new List<Point>();
        currentPosition = currentPos;
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
        this.BackColor = Color.FromArgb(50, 50, 50);

        // Initialize grid panel
        gridPanel = new Panel
        {
            Location = new Point(40, 40),
            Size = new Size(CELL_SIZE * (GRID_WIDTH + 1), CELL_SIZE * (GRID_HEIGHT + 1)),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(50, 50, 50)
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
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50)
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
                TextAlign = ContentAlignment.BottomCenter,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50)
            };
            this.Controls.Add(colLabel);
        }

        // Create coordinates label
        coordinatesLabel = new Label
        {
            Location = new Point(40, CELL_SIZE * (GRID_HEIGHT + 1) + 50),
            Size = new Size(300, 30),
            Text = "Selected: None",
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 50)
        };
        this.Controls.Add(coordinatesLabel);

        // Create confirm button
        confirmButton = new Button
        {
            Location = new Point(350, CELL_SIZE * (GRID_HEIGHT + 1) + 50),
            Size = new Size(100, 30),
            Text = "Confirm",
            Enabled = false,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White
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
                    Tag = new Point(j, i),
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderColor = Color.Gray;

                Point currentPoint = new Point(j, i);

                if (takenPositions.Contains(currentPoint))
                {
                    // Mark taken positions
                    btn.Enabled = false;
                    btn.BackColor = Color.FromArgb(70, 70, 70); // Darker gray
                    btn.Text = "X";
                }
                else if (currentPosition.HasValue && currentPoint == currentPosition.Value)
                {
                    // Highlight current position
                    btn.BackColor = Color.FromArgb(0, 150, 136); // Teal color
                    btn.Text = "";
                    btn.Click += GridButton_Click;
                }
                else
                {
                    btn.Text = "";
                    btn.Click += GridButton_Click;
                }

                gridButtons[j, i] = btn;
                gridPanel.Controls.Add(btn);
            }
        }

        // Update coordinates label if current position is set
        if (currentPosition.HasValue)
        {
            coordinatesLabel.Text = $"Current Position: ({currentPosition.Value.X}, {currentPosition.Value.Y})";
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
                // Reset to default or current position color
                if (currentPosition.HasValue && prev == currentPosition.Value)
                {
                    gridButtons[prev.X, prev.Y].BackColor = Color.FromArgb(0, 150, 136); // Teal color
                }
                else
                {
                    gridButtons[prev.X, prev.Y].BackColor = Color.FromArgb(30, 30, 30);
                }
            }
        }

        // Update new selection
        selectedPosition = coordinates;
        clickedButton.BackColor = Color.FromArgb(0, 120, 215); // Highlight color
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
