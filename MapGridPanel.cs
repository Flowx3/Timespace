
namespace TimeSpace
{
    public class MapGridPanel : Panel
    {
        public byte[] _grid;
        public int _width;
        public int _height;
        public int _cellSize;
        private string _currentMapId;
        private (int x, int y)? _highlightedPosition = null;
        private byte _originalHighlightValue = 0;

        private const int FixedPanelWidth = 790;
        private const int FixedPanelHeight = 450;

        public event EventHandler<CellClickedEventArgs> CellClicked;

        public MapGridPanel()
        {
            // Set fixed panel size
            this.Width = FixedPanelWidth;
            this.Height = FixedPanelHeight;

            // Set the panel's Name property
            this.Name = "mapGridPanel";

            this.Size = new Size(FixedPanelWidth, FixedPanelHeight);
            // Enable auto scrolling and double buffering
            //this.AutoScroll = true;
            this.DoubleBuffered = true;
            this.BorderStyle = BorderStyle.FixedSingle;

            this.MouseClick += MapGridPanel_MouseClick; // Subscribe to mouse click event
        }
        public void SetGrid(string mapId, int width, int height, byte[] grid)
        {
            _width = width;
            _height = height;
            _grid = grid;
            _currentMapId = mapId;

            // Calculate the maximum cell size that fits the panel
            int maxCellWidth = FixedPanelWidth / _width;
            int maxCellHeight = FixedPanelHeight / _height;
            _cellSize = Math.Min(maxCellWidth, maxCellHeight);

            // Calculate the total width and height required to display the grid
            int totalWidth = _width * _cellSize;
            int totalHeight = _height * _cellSize;

            // Set the panel's size to fit the grid
            this.AutoScroll = true; // Ensure scrolling is enabled
            this.Width = Math.Max(FixedPanelWidth, totalWidth);
            this.Height = Math.Max(FixedPanelHeight, totalHeight);
            Invalidate(); // Trigger a repaint
        }
        public bool IsWalkable(int x, int y)
        {
            string errorMessage;
            return !IsBlockingZone(x, y, out errorMessage);
        }
        public (int x, int y) GenerateWalkablePosition()
        {
            // If grid is empty or null, return default position
            if (_grid == null || _grid.Length == 0)
            {
                return (0, 0);
            }

            Random random = new Random();
            int maxAttempts = _width * _height; // Maximum attempts to find a walkable position
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                int x = random.Next(0, _width);
                int y = random.Next(0, _height);

                string errorMessage;
                if (!IsBlockingZone(x, y, out errorMessage))
                {
                    return (x, y);
                }

                attempts++;
            }

            // If no walkable position found after max attempts, throw exception
            throw new InvalidOperationException("Could not find a walkable position in the grid after maximum attempts.");
        }
        public bool IsBlockingZone(int x, int y, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check if position is within bounds
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                errorMessage = $"Position ({x}, {y}) is out of bounds. Grid size is {_width}x{_height}.";
                return true;
            }

            // Get the grid value at the position
            int index = y * _width + x;
            byte value = _grid[index];

            // Check if the position has any flags (not white)
            Color positionColor = GetColor(value);
            if (positionColor != Color.White && positionColor != Color.Red && positionColor != Color.Purple)
            {
                errorMessage = $"Position ({x}, {y}) is invalid. It contains a blocking flag (Color: {positionColor}).";
                return true;
            }

            return false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_grid == null) return;

            var graphics = e.Graphics;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var color = GetColor(_grid[y * _width + x]);
                    using (var brush = new SolidBrush(color))
                    {
                        graphics.FillRectangle(brush, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                    }

                    // Draw the cell border
                    using (var pen = new Pen(Color.Black))
                    {
                        graphics.DrawRectangle(pen, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                    }
                }
            }
        }
        private readonly Dictionary<int, Color> _flagColors = new Dictionary<int, Color>
    {
        { 0x1, Color.Gray },     // IsWalkingDisabled
        { 0x2, Color.Blue },      // IsAttackDisabledThrough
        { 0x4, Color.Blue },     // UnknownYet
        { 0x8, Color.MediumVioletRed },    // IsMonsterAggroDisabled
        { 0x10, Color.Purple },  // IsPvpDisabled
        { 0x20, Color.Orange },  // MateDoll
        { 0x30, Color.Gold },    // Objective
        { 0x40, Color.LightBlue }, // Portal
        { 0x50,  Color.Green}, // Monster
        { 0x80, Color.Red },     // Monster
        { 0x100, Color.Purple }, // Target Monster
    };

        // Method to reset the grid to default state
        public void ResetGrid()
        {
            if (_grid != null)
            {
                for (int i = 0; i < _grid.Length; i++)
                {
                    _grid[i] = 0; // Reset to default (white)
                }
                Invalidate();
            }
        }
        public void HighlightPosition(int x, int y)
        {
            if (!IsValidPosition(x, y)) return;

            // Store previous position to restore it
            if (_highlightedPosition.HasValue)
            {
                // Restore the original value at the previous position
                int prevIdx = _highlightedPosition.Value.y * _width + _highlightedPosition.Value.x;
                _grid[prevIdx] = _originalHighlightValue;
            }

            // Store new position and its original value
            _highlightedPosition = (x, y);
            int idx = y * _width + x;
            _originalHighlightValue = _grid[idx];

            // Apply highlight
            _grid[idx] = 0x50; // Green highlight flag

            Invalidate();
        }
        public void ClearHighlight()
        {
            if (_highlightedPosition.HasValue)
            {
                int idx = _highlightedPosition.Value.y * _width + _highlightedPosition.Value.x;
                _grid[idx] = _originalHighlightValue;
                _highlightedPosition = null;
                Invalidate();
            }
        }

        public void UpdateMapMarkings(CustomTabPage currentMapTab, string mapId)
        {
            // Only update if this grid belongs to the current map
            if (currentMapTab == null || _grid == null || _currentMapId != mapId)
                return;
            // Create a temporary copy of the original grid
            byte[] originalGrid = new byte[_grid.Length];
            Array.Copy(_grid, originalGrid, _grid.Length);

            // Mark portals for this specific map
            foreach (var portal in currentMapTab.Portals)
            {
                if (IsValidPosition(portal.FromX, portal.FromY))
                {
                    MarkPortal(portal.FromX, portal.FromY);
                }
            }

            // Mark monsters for this specific map
            foreach (DataGridViewRow row in currentMapTab.MonsterDataGridView.Rows)
            {
                if (row.IsNewRow || row.Cells["X"].Value == null || row.Cells["Y"].Value == null)
                    continue;

                int x = Convert.ToInt32(row.Cells["X"].Value);
                int y = Convert.ToInt32(row.Cells["Y"].Value);
                bool isTarget = DetermineIfMonsterIsTarget(row);

                if (IsValidPosition(x, y))
                {
                    MarkMonster(x, y, isTarget);
                }
            }

            // Mark objectives for this specific map
            foreach (var obj in currentMapTab.Objects)
            {
                int x = obj.GetX();
                int y = obj.GetY();

                if (x != 1500 && y != 1500 && IsValidPosition(x, y))
                {
                    MarkObjective(x, y);
                }
            }

            // Preserve original grid values where we haven't added custom markings
            for (int i = 0; i < _grid.Length; i++)
            {
                if (_grid[i] == 0) // If we haven't marked this cell
                {
                    _grid[i] = originalGrid[i]; // Restore original value
                }
            }

            Invalidate();
        }

        private void MarkPortal(int x, int y)
        {
            _grid[y * _width + x] = 0x40;
        }

        private void MarkMonster(int x, int y, bool isTarget)
        {
            _grid[y * _width + x] = (byte)(isTarget ? 0x100 : 0x80);
        }

        private void MarkObjective(int x, int y)
        {
            _grid[y * _width + x] = 0x30;
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height && _grid != null;
        }
        private bool DetermineIfMonsterIsTarget(DataGridViewRow row)
        {
            bool isTarget = Convert.ToBoolean(row.Cells["AsTarget"].Value);
            return isTarget;
        }

        // Method to clear specific type of marking
        public void ClearMarking(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0; // Reset to default (white)
                Invalidate();
            }
        }

        // Helper method to get existing marking at position
        public byte GetMarking(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _grid[y * _width + x];
            }
            return 0;
        }
        private Color GetColor(byte value)
        {
            // Customize this method based on your grid values
            // Map each flag value to its corresponding color
            if (_flagColors.TryGetValue(value, out var color))
            {
                return color;
            }
            return Color.White; // Default color for undefined flags
        }

        private void MapGridPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // Calculate cell coordinates based on mouse click position
            int cellX = e.X / _cellSize;
            int cellY = e.Y / _cellSize;

            // Ensure the click is within bounds
            if (cellX >= 0 && cellX < _width && cellY >= 0 && cellY < _height)
            {
                OnCellClicked(new CellClickedEventArgs(cellX, cellY));
            }
        }

        protected virtual void OnCellClicked(CellClickedEventArgs e)
        {
            CellClicked?.Invoke(this, e);
        }
    }
}