public class ModernGridSelector : AnimatedPanel
{
    private const int CELL_SIZE = 40;
    private const int GRID_WIDTH = 11;
    private const int GRID_HEIGHT = 11;
    private List<Point> takenPositions;
    private Point? selectedPosition;
    private Point? currentPosition;
    private ModernButton[,] gridButtons;
    private ModernButton confirmButton;
    private Label coordinatesLabel;
    private Panel gridPanel;

    public event EventHandler<Point> CoordinatesSelected;

    public ModernGridSelector(List<Point> existingPositions, Point? currentPos = null)
    {
        takenPositions = existingPositions ?? new List<Point>();
        currentPosition = currentPos;
        InitializeGridPanel();
        CreateGridButtons();
        this.Visible = false;
    }

    private void InitializeGridPanel()
    {
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.Size = new Size(CELL_SIZE * (GRID_WIDTH + 1) + 100, CELL_SIZE * (GRID_HEIGHT + 2) + 100);

        // Initialize grid panel with modern styling
        gridPanel = new Panel
        {
            Location = new Point(40, 40),
            Size = new Size(CELL_SIZE * (GRID_WIDTH + 1), CELL_SIZE * (GRID_HEIGHT + 1)),
            BackColor = Color.FromArgb(35, 35, 35)
        };
        this.Controls.Add(gridPanel);

        // Modern styled labels
        for (int i = 0; i <= GRID_HEIGHT; i++)
        {
            Label rowLabel = new Label
            {
                Text = $"{i}",
                Location = new Point(5, 40 + i * CELL_SIZE),
                Size = new Size(30, CELL_SIZE),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 9f)
            };
            this.Controls.Add(rowLabel);
        }

        for (int j = 0; j <= GRID_WIDTH; j++)
        {
            Label colLabel = new Label
            {
                Text = $"{j}",
                Location = new Point(40 + j * CELL_SIZE, 5),
                Size = new Size(CELL_SIZE, 30),
                TextAlign = ContentAlignment.BottomCenter,
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 9f)
            };
            this.Controls.Add(colLabel);
        }

        coordinatesLabel = new Label
        {
            Location = new Point(40, CELL_SIZE * (GRID_HEIGHT + 1) + 50),
            Size = new Size(300, 30),
            Text = "Click to select coordinates",
            ForeColor = Color.FromArgb(200, 200, 200),
            Font = new Font("Segoe UI", 10f)
        };
        this.Controls.Add(coordinatesLabel);

        confirmButton = new ModernButton
        {
            Location = new Point(350, CELL_SIZE * (GRID_HEIGHT + 1) + 50),
            Size = new Size(100, 30),
            Text = "Confirm",
            Enabled = false
        };
        confirmButton.Click += ConfirmButton_Click;
        this.Controls.Add(confirmButton);
    }

    private void CreateGridButtons()
    {
        gridButtons = new ModernButton[GRID_WIDTH + 1, GRID_HEIGHT + 1];
        for (int i = 0; i <= GRID_HEIGHT; i++)
        {
            for (int j = 0; j <= GRID_WIDTH; j++)
            {
                var btn = new ModernButton
                {
                    Location = new Point(j * CELL_SIZE, i * CELL_SIZE),
                    Size = new Size(CELL_SIZE, CELL_SIZE),
                    Tag = new Point(j, i)
                };

                Point currentPoint = new Point(j, i);

                if (takenPositions.Contains(currentPoint))
                {
                    btn.State = ButtonState.Disabled;
                    btn.Text = "×";
                }
                else if (currentPosition.HasValue && currentPoint == currentPosition.Value)
                {
                    btn.State = ButtonState.Current;
                    btn.Click += GridButton_Click;
                }
                else
                {
                    btn.State = ButtonState.Normal;
                    btn.Click += GridButton_Click;
                }

                gridButtons[j, i] = btn;
                gridPanel.Controls.Add(btn);
            }
        }
    }

    private void GridButton_Click(object sender, EventArgs e)
    {
        var clickedButton = (ModernButton)sender;
        var coordinates = (Point)clickedButton.Tag;

        if (selectedPosition.HasValue)
        {
            var prev = selectedPosition.Value;
            if (!takenPositions.Contains(prev))
            {
                gridButtons[prev.X, prev.Y].State =
                    currentPosition.HasValue && prev == currentPosition.Value
                    ? ButtonState.Current
                    : ButtonState.Normal;
            }
        }

        selectedPosition = coordinates;
        clickedButton.State = ButtonState.Selected;
        coordinatesLabel.Text = $"Selected: ({coordinates.X}, {coordinates.Y})";
        confirmButton.Enabled = true;
    }

    private void ConfirmButton_Click(object sender, EventArgs e)
    {
        if (selectedPosition.HasValue)
        {
            CoordinatesSelected?.Invoke(this, selectedPosition.Value);
            HideAnimated();
        }
    }
}