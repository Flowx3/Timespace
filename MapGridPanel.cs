using System.Windows.Input;

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
        private bool _isDragging = false;
        private (int x, int y)? _dragStart = null;
        private byte _draggedElementType = 0;
        private CustomTabPage _parentTab;
        private Point _lastMousePosition;
        private readonly Stack<IPanelCommand> _undoStack = new Stack<IPanelCommand>();
        private readonly Stack<IPanelCommand> _redoStack = new Stack<IPanelCommand>();

        private const int FixedPanelWidth = 790;
        private const int FixedPanelHeight = 450;


        public interface IPanelCommand
        {
            void Execute();
            void Undo();
        }
        public class MoveElementCommand : IPanelCommand
        {
            private readonly MapGridPanel _panel;
            private readonly int _fromX, _fromY, _toX, _toY;
            private readonly byte _elementType;

            public MoveElementCommand(MapGridPanel panel, int fromX, int fromY, int toX, int toY, byte elementType)
            {
                _panel = panel;
                _fromX = fromX;
                _fromY = fromY;
                _toX = toX;
                _toY = toY;
                _elementType = elementType;
            }

            public void Execute()
            {
                _panel.UpdateElementPosition(_fromX, _fromY, _toX, _toY);
            }

            public void Undo()
            {
                _panel.UpdateElementPosition(_toX, _toY, _fromX, _fromY);
            }

        }
        public MapGridPanel()
        {
            // Set fixed panel size
            Width = FixedPanelWidth;
            Height = FixedPanelHeight;

            // Set the panel's Name property
            Name = "mapGridPanel";

            Size = new Size(FixedPanelWidth, FixedPanelHeight);
            // Enable auto scrolling and double buffering
            //this.AutoScroll = true;
            DoubleBuffered = true;
            BorderStyle = BorderStyle.FixedSingle;

        }
        public void InitializeDragAndDrop(CustomTabPage parentTab)
        {
            _parentTab = parentTab;
            this.MouseDown += MapGridPanel_MouseDown;
            this.MouseMove += MapGridPanel_MouseMove;
            this.MouseUp += MapGridPanel_MouseUp;
        }

        public void SetGrid(string mapId, int width, int height, byte[] grid)
        {
            // Validate input parameters  
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException($"Invalid grid dimensions: width={width}, height={height}");
            }
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            _width = width;
            _height = height;
            _grid = grid;
            _currentMapId = mapId;

            // Calculate cell size based on panel dimensions  
            int maxCellWidth = Math.Max(1, FixedPanelWidth / _width);
            int maxCellHeight = Math.Max(1, FixedPanelHeight / _height);
            _cellSize = Math.Min(maxCellWidth, maxCellHeight);

            // Calculate total dimensions  
            int totalWidth = _width * _cellSize;
            int totalHeight = _height * _cellSize;

            // Set panel size  
            AutoScroll = true;
            Width = Math.Max(FixedPanelWidth, totalWidth);
            Height = Math.Max(FixedPanelHeight, totalHeight);

            Invalidate();
        }

        public void RecalculateCellSize()
        {
            if (_width <= 0 || _height <= 0)
            {
                throw new InvalidOperationException("Cannot recalculate cell size: invalid grid dimensions");
            }
            SetGrid(_currentMapId, _width, _height, _grid);
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
            // Clear the background
            e.Graphics.Clear(this.BackColor);

            // Draw the grid
            if (_grid != null)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var color = GetColor(_grid[y * _width + x]);
                        using (var brush = new SolidBrush(color))
                        {
                            e.Graphics.FillRectangle(brush, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                        }

                        using (var pen = new Pen(Color.Black))
                        {
                            e.Graphics.DrawRectangle(pen, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                        }
                    }
                }

                // Draw ghost image while dragging
                if (_isDragging && _dragStart.HasValue)
                {
                    int cellX = _lastMousePosition.X / _cellSize;
                    int cellY = _lastMousePosition.Y / _cellSize;

                    if (IsValidPosition(cellX, cellY))
                    {
                        Color ghostColor = GetColor(_draggedElementType);
                        using (var brush = new SolidBrush(Color.FromArgb(128, ghostColor)))
                        {
                            e.Graphics.FillRectangle(brush, cellX * _cellSize, cellY * _cellSize, _cellSize, _cellSize);
                        }

                        using (var pen = new Pen(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                        {
                            e.Graphics.DrawRectangle(pen, cellX * _cellSize, cellY * _cellSize, _cellSize, _cellSize);
                        }
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
        { 0x90, Color.Purple }, // Target Monster
    };

        public void ResetGrid()
        {
            if (_grid != null)
            {
                // Create a new grid instead of modifying the existing one  
                _grid = new byte[_width * _height];
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

        public void UpdateMapMarkings(CustomTabPage currentMapTab, string mapId, byte[] originalGrid)
        {
            if (currentMapTab == null || _grid == null || _currentMapId != mapId)
                return;

            // Clear the grid before applying new markings  
            ResetGrid();

            // Use the passed original grid instead of current potentially modified grid  
            Array.Copy(originalGrid, _grid, _grid.Length);

            // Mark portals, monsters, and objectives as before...  
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
                bool isTarget = Convert.ToBoolean(row.Cells["AsTarget"].Value);
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

        private void MarkPortal(int? x, int? y)
        {
            if (x.HasValue && y.HasValue)
            {
                _grid[y.Value * _width + x.Value] = 0x40;
            }
        }

        private void MarkMonster(int x, int y, bool isTarget)
        {
            _grid[y * _width + x] = (byte)(isTarget ? 0x90 : 0x80);
        }

        private void MarkObjective(int x, int y)
        {
            _grid[y * _width + x] = 0x30;
        }

        private bool IsValidPosition(int? x, int? y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height && _grid != null;
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

        #region MapGridMover

        private void MapGridPanel_MouseDown(object sender, MouseEventArgs e)
        {
            int cellX = e.X / _cellSize;
            int cellY = e.Y / _cellSize;

            if (!IsValidPosition(cellX, cellY))
                return;

            byte currentElement = GetMarking(cellX, cellY);

            if (currentElement == 0x40 || // Portal
                currentElement == 0x30 || // Objective
                currentElement == 0x80 || // Monster
                currentElement == 0x90)   // Target Monster
            {
                _isDragging = true;
                _dragStart = (cellX, cellY);
                _draggedElementType = currentElement;
                _lastMousePosition = e.Location;
                Invalidate();
            }
        }

        private void MapGridPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !_dragStart.HasValue)
                return;

            _lastMousePosition = e.Location;
            Invalidate();
        }

        private void MapGridPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !_dragStart.HasValue || _parentTab == null)
                return;

            int targetX = e.X / _cellSize;
            int targetY = e.Y / _cellSize;

            if (!IsValidPosition(targetX, targetY))
            {
                ResetDragState();
                return;
            }

            // Don't allow dropping on other elements
            if (GetMarking(targetX, targetY) != 0 && GetMarking(targetX, targetY) != _draggedElementType)
            {
                ResetDragState();
                return;
            }

            // Create and execute the move command
            var command = new MoveElementCommand(
                this,
                _dragStart.Value.x,
                _dragStart.Value.y,
                targetX,
                targetY,
                _draggedElementType
            );

            ExecuteCommand(command);
            ResetDragState();
        }
        private void MapGridPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_grid == null) return;

            // Draw the normal grid
            DrawGrid(e.Graphics);

            // Draw ghost image while dragging
            if (_isDragging && _dragStart.HasValue)
            {
                int cellX = _lastMousePosition.X / _cellSize;
                int cellY = _lastMousePosition.Y / _cellSize;

                if (IsValidPosition(cellX, cellY))
                {
                    Color ghostColor = GetColor(_draggedElementType);
                    using (var brush = new SolidBrush(Color.FromArgb(128, ghostColor)))
                    {
                        e.Graphics.FillRectangle(brush, cellX * _cellSize, cellY * _cellSize, _cellSize, _cellSize);
                    }

                    // Draw a dashed border around the ghost image
                    using (var pen = new Pen(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                    {
                        e.Graphics.DrawRectangle(pen, cellX * _cellSize, cellY * _cellSize, _cellSize, _cellSize);
                    }
                }
            }
        }

        public void UpdateElementPosition(int fromX, int fromY, int toX, int toY)
        {
            switch (_draggedElementType)
            {
                case 0x40: // Portal
                    UpdatePortalPosition(fromX, fromY, toX, toY);
                    break;
                case 0x30: // Objective
                    UpdateObjectivePosition(fromX, fromY, toX, toY);
                    break;
                case 0x80: // Monster
                case 0x90: // Target Monster
                    UpdateMonsterPosition(fromX, fromY, toX, toY);
                    break;
            }

            // Update the grid marking
            ClearMarking(fromX, fromY);
            _grid[toY * _width + toX] = _draggedElementType;
            Invalidate();
        }

        private void UpdatePortalPosition(int fromX, int fromY, int toX, int toY)
        {
            var portal = _parentTab.Portals.FirstOrDefault(p => p.FromX == fromX && p.FromY == fromY);
            if (portal != null)
            {
                portal.FromX = toX;
                portal.FromY = toY;
            }
        }

        private void UpdateObjectivePosition(int fromX, int fromY, int toX, int toY)
        {
            var objective = _parentTab.Objects.FirstOrDefault(o => o.GetX() == fromX && o.GetY() == fromY);
            if (objective != null)
            {
                objective.SetPosition(toX, toY);
            }
        }

        private void UpdateMonsterPosition(int fromX, int fromY, int toX, int toY)
        {
            foreach (DataGridViewRow row in _parentTab.MonsterDataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                if (Convert.ToInt32(row.Cells["X"].Value) == fromX &&
                    Convert.ToInt32(row.Cells["Y"].Value) == fromY)
                {
                    row.Cells["X"].Value = toX;
                    row.Cells["Y"].Value = toY;
                    break;
                }
            }
        }
        private void DrawGrid(Graphics graphics)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var color = GetColor(_grid[y * _width + x]);
                    using (var brush = new SolidBrush(color))
                    {
                        graphics.FillRectangle(brush, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                    }

                    using (var pen = new Pen(Color.Black))
                    {
                        graphics.DrawRectangle(pen, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
                    }
                }
            }
        }
        private void ResetDragState()
        {
            _isDragging = false;
            _dragStart = null;
            _draggedElementType = 0;
            Invalidate();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                Invalidate();
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                Invalidate();
            }
        }

        private void ExecuteCommand(IPanelCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Clear redo stack when new command is executed
        }

        #endregion
    }
}