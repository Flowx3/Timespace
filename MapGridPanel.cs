using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TimeSpace
{
    public class MapGridPanel : Panel
    {
        private byte[] _grid;
        private int _width;
        private int _height;
        private int _cellSize;
        private string _currentMapId;
        private (int x, int y)? _highlightedPosition;
        private byte _originalHighlightValue;
        private bool _isDragging;
        private (int x, int y)? _dragStart;
        private byte _draggedElementType;
        private CustomTabPage _parentTab;
        private ContextMenuStrip _contextMenu;
        private (int x, int y) _contextMenuPosition;
        private Point _lastMousePosition;
        private readonly Stack<IPanelCommand> _undoStack = new();
        private readonly Stack<IPanelCommand> _redoStack = new();
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
        private Label _positionLabel;
        private const int FixedPanelWidth = 790;
        private const int FixedPanelHeight = 450;
        private const string NORTH = "North";
        private const string SOUTH = "South";
        private const string EAST = "East";
        private const string WEST = "West";
        #region IPanelCommand Interface

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

            public void Execute() => _panel.UpdateElementPosition(_fromX, _fromY, _toX, _toY);
            public void Undo() => _panel.UpdateElementPosition(_toX, _toY, _fromX, _fromY);
        }

        #endregion

        public MapGridPanel()
        {
            Width = FixedPanelWidth;
            Height = FixedPanelHeight;
            Name = "mapGridPanel";
            Size = new Size(FixedPanelWidth, FixedPanelHeight);
            DoubleBuffered = true;
            BorderStyle = BorderStyle.FixedSingle;
            InitializePositionLabel();
            InitializeContextMenu();
            MouseMove += MapGridPanel_PositionUpdate;
            MouseLeave += MapGridPanel_MouseLeave;
        }

        #region Position Label

        private void InitializePositionLabel()
        {
            _positionLabel = new Label
            {
                AutoSize = true,
                BackColor = Color.FromArgb(128, Color.Black),
                ForeColor = Color.White,
                Font = new Font("Arial", 9f, FontStyle.Bold),
                Visible = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            Controls.Add(_positionLabel);
        }

        private void MapGridPanel_PositionUpdate(object sender, MouseEventArgs e)
        {
            if (_cellSize == 0)
            {
                // Handle the error appropriately, e.g., log it or set a default value
                _positionLabel.Visible = false;
                return;
            }

            int cellX = e.X / _cellSize;
            int cellY = e.Y / _cellSize;

            if (IsValidPosition(cellX, cellY))
            {
                _positionLabel.Text = $"X: {cellX}, Y: {cellY}";

                // Calculate position for the label
                int labelX = Math.Clamp(e.X + 10, 0, Width - _positionLabel.Width);
                int labelY = Math.Clamp(e.Y + 10, 0, Height - _positionLabel.Height);

                _positionLabel.Location = new Point(labelX, labelY);
                _positionLabel.Visible = true;
                _positionLabel.BringToFront();
            }
            else
            {
                _positionLabel.Visible = false;
            }
        }

        private void MapGridPanel_MouseLeave(object sender, EventArgs e)
        {
            _positionLabel.Visible = false;
        }

        #endregion
        #region Context Menu
        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Add Portal", null, AddPortal_Click);
            _contextMenu.Items.Add("Add Monster", null, AddMonster_Click);
            _contextMenu.Items.Add("Add NPC", null, AddNpc_Click);
            _contextMenu.Items.Add("Add Objective", null, AddObjective_Click);
            _contextMenu.Items.Add("Remove Element", null, RemoveElement_Click);

            MouseClick += MapGridPanel_MouseClick;
        }
        private void MapGridPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || _parentTab == null)
                return;

            int cellX = e.X / _cellSize;
            int cellY = e.Y / _cellSize;

            if (!IsValidPosition(cellX, cellY))
                return;

            _contextMenuPosition = (cellX, cellY);

            bool isWalkable = IsWalkable(cellX, cellY);
            if (isWalkable)
            {
                _contextMenu.Items[0].Visible = true;
                _contextMenu.Items[1].Visible = true;
                _contextMenu.Items[2].Visible = true;
                _contextMenu.Items[3].Visible = true;
                _contextMenu.Items[4].Visible = false;
            }
            else
            {
                _contextMenu.Items[0].Visible = false;
                _contextMenu.Items[1].Visible = false;
                _contextMenu.Items[2].Visible = false;
                _contextMenu.Items[3].Visible = false;
                _contextMenu.Items[4].Visible = true;
            }

            _contextMenu.Show(this, e.Location);
        }
        private void AddPortal_Click(object sender, EventArgs e)
        {
            if (_parentTab.Portals.Count >= CustomTabPage.MAX_PORTALS)
            {
                MessageBox.Show("You don't need more than 4 Portals.");
                return;
            }

            string orientation = DetermineOrientation(_contextMenuPosition.x, _contextMenuPosition.y);

            var portal = new Portal(
                _parentTab.MapName,
                "DefaultTo",
                "Type1",
                orientation,
                _contextMenuPosition.x,
                _contextMenuPosition.y,
                null,
                null,
                _parentTab.GetMapNames,
                _parentTab
            );

            _parentTab.Portals.Add(portal);
            _parentTab._portalPanel.Controls.Add(portal.CreatePortal());
            _parentTab.SaveAndRefreshPortals(sender, e, false);

            _grid[_contextMenuPosition.y * _width + _contextMenuPosition.x] = 0x40;
            Refresh();
        }

        private string DetermineOrientation(int x, int y)
        {
            // Calculate position relative to map center
            double centerX = _width / 2.0;
            double centerY = _height / 2.0;

            // Calculate angles from center to point
            double angle = Math.Atan2(y - centerY, x - centerX) * (180 / Math.PI);

            // Convert angle to orientation
            return angle switch
            {
                <= 45 and > -45 => EAST,
                <= 135 and > 45 => SOUTH,
                <= -45 and > -135 => NORTH,
                _ => WEST
            };
        }


        private void AddMonster_Click(object sender, EventArgs e)
        {
            var monster = new Monster(_parentTab.MapName);
            _parentTab.MonsterEvents.Add(monster);

            object[] rowData = new object[]
            {
            "",     // Vnum
            _contextMenuPosition.x,    // X
            _contextMenuPosition.y,    // Y
            "",    // Wave
            false,  // AsTarget
            ""      // Attributes
            };

            _parentTab._monsterDataGridView.Rows.Add(rowData);

            _grid[_contextMenuPosition.y * _width + _contextMenuPosition.x] = 0x80;
            Refresh();
        }
        private void AddNpc_Click(object sender, EventArgs e)
        {
            var npc = new Npc(_parentTab.MapName);
            _parentTab.NpcEvents.Add(npc);

            object[] rowData = new object[]
            {
                "",     // Vnum
                _contextMenuPosition.x,    // X
                _contextMenuPosition.y,    // Y
                ""      // Attributes
            };
            _parentTab._npcDataGridView.Rows.Add(rowData);

            _grid[_contextMenuPosition.y * _width + _contextMenuPosition.x] = 0x50;
            Refresh();
        }
        private void AddObjective_Click(object sender, EventArgs e)
        {
            if (_parentTab.Objects.Count >= CustomTabPage.MAX_OBJECTS)
            {
                MessageBox.Show("You don't need more than 4 Objects.");
                return;
            }

            var mapObject = new MapObject(
                _parentTab.MapName,
                "Object",
                _contextMenuPosition.x,
                _contextMenuPosition.y,
                _parentTab._allPortalsList,
                _parentTab
            );

            _parentTab.Objects.Add(mapObject);
            _parentTab._objectivePanel.Controls.Add(mapObject.CreateObject());

            // Update marking and force refresh
            _grid[_contextMenuPosition.y * _width + _contextMenuPosition.x] = 0x30;
            Refresh();
        }

        private void RemoveElement_Click(object sender, EventArgs e)
        {
            byte elementType = GetMarking(_contextMenuPosition.x, _contextMenuPosition.y);

            switch (elementType)
            {
                case 0x40: // Portal
                    RemovePortal(_contextMenuPosition.x, _contextMenuPosition.y);
                    break;
                case 0x30: // Objective
                    RemoveObjective(_contextMenuPosition.x, _contextMenuPosition.y);
                    break;
                case 0x50: // NPC
                    RemoveNpc(_contextMenuPosition.x, _contextMenuPosition.y);
                    break;
                case 0x80: // Monster
                case 0x90: // Target Monster
                    RemoveMonster(_contextMenuPosition.x, _contextMenuPosition.y);
                    break;
            }

            _grid[_contextMenuPosition.y * _width + _contextMenuPosition.x] = 0;
            Refresh();
        }

        private void RemovePortal(int x, int y)
        {
            var portal = _parentTab.Portals.FirstOrDefault(p => p.FromX == x && p.FromY == y);
            if (portal != null)
            {
                _parentTab.Portals.Remove(portal);
                _parentTab._portalPanel.Controls.Remove(portal.Panel);
                _parentTab._portalPanel.Refresh();
                _parentTab.SaveAndRefreshPortals(this, EventArgs.Empty, false);
            }
        }

        private void RemoveObjective(int x, int y)
        {
            var objective = _parentTab.Objects.FirstOrDefault(o => o.GetX() == x && o.GetY() == y);
            if (objective != null)
            {
                _parentTab.Objects.Remove(objective);
                _parentTab._objectivePanel.Controls.Remove(objective.Panel);
                _parentTab._objectivePanel.Refresh();
            }
        }

        private void RemoveMonster(int x, int y)
        {
            for (int i = _parentTab._monsterDataGridView.Rows.Count - 1; i >= 0; i--)
            {
                var row = _parentTab._monsterDataGridView.Rows[i];
                if (!row.IsNewRow &&
                    Convert.ToInt32(row.Cells["X"].Value) == x &&
                    Convert.ToInt32(row.Cells["Y"].Value) == y)
                {
                    _parentTab._monsterDataGridView.Rows.RemoveAt(i);
                    break;
                }
            }
        }
        private void RemoveNpc(int x, int y)
        {
            for (int i = _parentTab._npcDataGridView.Rows.Count - 1; i >= 0; i--)
            {
                var row = _parentTab._npcDataGridView.Rows[i];
                if (!row.IsNewRow &&
                    Convert.ToInt32(row.Cells["X"].Value) == x &&
                    Convert.ToInt32(row.Cells["Y"].Value) == y)
                {
                    _parentTab._npcDataGridView.Rows.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion
        public void InitializeDragAndDrop(CustomTabPage parentTab)
        {
            _parentTab = parentTab;
            MouseDown += MapGridPanel_MouseDown;
            MouseMove += MapGridPanel_MouseMove;
            MouseUp += MapGridPanel_MouseUp;
        }

        public void SetGrid(string mapId, int width, int height, byte[] grid)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException($"Invalid grid dimensions: width={width}, height={height}");

            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _width = width;
            _height = height;
            _currentMapId = mapId;

            int maxCellWidth = Math.Max(1, FixedPanelWidth / _width);
            int maxCellHeight = Math.Max(1, FixedPanelHeight / _height);
            _cellSize = Math.Min(maxCellWidth, maxCellHeight);

            Width = Math.Max(FixedPanelWidth, _width * _cellSize);
            Height = Math.Max(FixedPanelHeight, _height * _cellSize);

            AutoScroll = true;
            Invalidate();
        }

        public void RecalculateCellSize()
        {
            if (_width <= 0 || _height <= 0)
                throw new InvalidOperationException("Cannot recalculate cell size: invalid grid dimensions");

            SetGrid(_currentMapId, _width, _height, _grid);
        }

        public bool IsWalkable(int x, int y) => !IsBlockingZone(x, y, out _);

        public (int x, int y) GenerateWalkablePosition()
        {
            if (_grid == null || _grid.Length == 0)
                return (0, 0);

            Random random = new();
            int maxAttempts = _width * _height;

            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                int x = random.Next(0, _width);
                int y = random.Next(0, _height);

                if (IsWalkable(x, y))
                    return (x, y);
            }

            throw new InvalidOperationException("Could not find a walkable position in the grid after maximum attempts.");
        }

        public bool IsBlockingZone(int x, int y, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsValidPosition(x, y))
            {
                errorMessage = $"Position ({x}, {y}) is out of bounds. Grid size is {_width}x{_height}.";
                return true;
            }

            byte value = _grid[y * _width + x];
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
            e.Graphics.Clear(BackColor);

            if (_grid == null)
                return;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var color = GetColor(_grid[y * _width + x]);
                    using var brush = new SolidBrush(color);
                    e.Graphics.FillRectangle(brush, x * _cellSize, y * _cellSize, _cellSize, _cellSize);

                    using var pen = new Pen(Color.Black);
                    e.Graphics.DrawRectangle(pen, x * _cellSize, y * _cellSize, _cellSize, _cellSize);
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
                    using var brush = new SolidBrush(Color.FromArgb(128, ghostColor));
                    e.Graphics.FillRectangle(brush, cellX * _cellSize, cellY * _cellSize, _cellSize, _cellSize);

                    using var pen = new Pen(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                    e.Graphics.DrawRectangle(pen, cellX * _cellSize, cellY * _cellSize, _cellSize, _cellSize);
                }
            }
        }

        private readonly Dictionary<int, Color> _flagColors = new()
        {
            { 0x1, Color.Gray },         // IsWalkingDisabled
            { 0x2, Color.Blue },         // IsAttackDisabledThrough
            { 0x4, Color.Blue },         // UnknownYet
            { 0x8, Color.MediumVioletRed }, // IsMonsterAggroDisabled
            { 0x10, Color.Purple },      // IsPvpDisabled
            { 0x20, Color.Orange },      // MateDoll
            { 0x30, Color.Gold },        // Objective
            { 0x40, Color.LightBlue },   // Portal
            { 0x50, Color.Green },       // NPC
            { 0x60, Color.Cyan },        // HIGHLIGHT
            { 0x80, Color.Red },         // Monster
            { 0x90, Color.Purple },      // Target Monster
        };

        public void ResetGrid()
        {
            if (_grid != null)
            {
                _grid = new byte[_width * _height];
                Invalidate();
            }
        }

        public void HighlightPosition(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return;

            if (_highlightedPosition.HasValue)
            {
                int prevIdx = _highlightedPosition.Value.y * _width + _highlightedPosition.Value.x;
                _grid[prevIdx] = _originalHighlightValue;
            }

            _highlightedPosition = (x, y);
            int idx = y * _width + x;
            _originalHighlightValue = _grid[idx];

            _grid[idx] = 0x60; // Green highlight flag
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

            ResetGrid();
            Array.Copy(originalGrid, _grid, _grid.Length);

            foreach (var portal in currentMapTab.Portals)
            {
                if (IsValidPosition(portal.FromX, portal.FromY))
                    _grid[portal.FromY.Value * _width + portal.FromX.Value] = 0x40;
            }

            foreach (DataGridViewRow row in currentMapTab.MonsterDataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                int x = Convert.ToInt32(row.Cells["X"].Value);
                int y = Convert.ToInt32(row.Cells["Y"].Value);
                bool isTarget = Convert.ToBoolean(row.Cells["AsTarget"].Value);

                if (IsValidPosition(x, y))
                    _grid[y * _width + x] = isTarget ? (byte)0x90 : (byte)0x80;
            }

            foreach(DataGridViewRow row in currentMapTab.NpcDataGridview.Rows)
            {
                if (row.IsNewRow)
                    continue;

                int x = Convert.ToInt32(row.Cells["X"].Value);
                int y = Convert.ToInt32(row.Cells["Y"].Value);

                if (IsValidPosition(x,y))
                    _grid[y * _width + x] = 0x50;
            }

            foreach (var obj in currentMapTab.Objects)
            {
                int x = obj.GetX();
                int y = obj.GetY();

                if (x != 1500 && y != 1500 && IsValidPosition(x, y))
                    _grid[y * _width + x] = 0x30;
            }

            for (int i = 0; i < _grid.Length; i++)
            {
                if (_grid[i] == 0)
                    _grid[i] = originalGrid[i];
            }

            Refresh();
        }
        private void MarkPortal(int? x, int? y)
        {
            if (x.HasValue && y.HasValue && IsValidPosition(x.Value, y.Value))
            {
                _grid[y.Value * _width + x.Value] = 0x40;
                Refresh();
            }
        }

        private void MarkMonster(int x, int y, bool isTarget)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = isTarget ? (byte)0x90 : (byte)0x80;
                Refresh();
            }
        }
        private void MarkNpc(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0x50;
                Refresh();
            }
        }
        private void MarkObjective(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0x30;
                Refresh();
            }
        }

        private bool IsValidPosition(int? x, int? y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height && _grid != null;
        }

        public void ClearMarking(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[y * _width + x] = 0;
                Refresh();
            }
        }

        public byte GetMarking(int x, int y)
        {
            return IsValidPosition(x, y) ? _grid[y * _width + x] : (byte)0;
        }

        private Color GetColor(byte value)
        {
            return _flagColors.TryGetValue(value, out var color) ? color : Color.White;
        }

        #region MapGridMover

        private void MapGridPanel_MouseDown(object sender, MouseEventArgs e)
        {
            int cellX = e.X / _cellSize;
            int cellY = e.Y / _cellSize;

            if (!IsValidPosition(cellX, cellY))
                return;

            byte currentElement = GetMarking(cellX, cellY);

            if (currentElement is 0x40 or 0x30 or 0x80 or 0x90)
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

            if (!IsValidPosition(targetX, targetY) || (GetMarking(targetX, targetY) != 0 && GetMarking(targetX, targetY) != _draggedElementType))
            {
                ResetDragState();
                return;
            }

            var command = new MoveElementCommand(this, _dragStart.Value.x, _dragStart.Value.y, targetX, targetY, _draggedElementType);
            ExecuteCommand(command);
            ResetDragState();
        }

        public void UpdateElementPosition(int fromX, int fromY, int toX, int toY)
        {
            switch (_draggedElementType)
            {
                case 0x40:
                    UpdatePortalPosition(fromX, fromY, toX, toY);
                    break;
                case 0x30:
                    UpdateObjectivePosition(fromX, fromY, toX, toY);
                    break;
                case 0x80:
                case 0x90:
                    UpdateMonsterPosition(fromX, fromY, toX, toY);
                    break;
            }

            ClearMarking(fromX, fromY);
            _grid[toY * _width + toX] = _draggedElementType;
            Invalidate();
        }

        private void UpdatePortalPosition(int fromX, int fromY, int toX, int toY)
        {
            var portal = _parentTab.Portals.FirstOrDefault(p => p.FromX == fromX && p.FromY == fromY);
            portal?.SetPosition(toX, toY);

            foreach (var tab in _parentTab.Parent.Controls.OfType<CustomTabPage>())
            {
                if (tab.Text == _parentTab.Text)
                    continue;

                var linkedPortal = tab.Portals.FirstOrDefault(p => p.ToX == fromX && p.ToY == fromY && p.MapTo == portal.MapFrom);
                linkedPortal?.SetPosition(toX, toY, true);
            }
        }

        private void UpdateObjectivePosition(int fromX, int fromY, int toX, int toY)
        {
            var objective = _parentTab.Objects.FirstOrDefault(o => o.GetX() == fromX && o.GetY() == fromY);
            objective?.SetPosition(toX, toY);
        }

        private void UpdateMonsterPosition(int fromX, int fromY, int toX, int toY)
        {
            foreach (DataGridViewRow row in _parentTab.MonsterDataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (Convert.ToInt32(row.Cells["X"].Value) == fromX && Convert.ToInt32(row.Cells["Y"].Value) == fromY)
                {
                    row.Cells["X"].Value = toX;
                    row.Cells["Y"].Value = toY;
                    break;
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
            _redoStack.Clear();
        }

        #endregion
    }
}
