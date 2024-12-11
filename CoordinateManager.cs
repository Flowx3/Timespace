using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CoordinateManager
{
    private static readonly object _positionsLock = new();
    private static readonly List<Point> _sharedTakenPositions = new();
    private Point? _currentPosition;
    private readonly TextBox _coordinatesTextBox;

    public CoordinateManager(TextBox coordinatesTextBox)
    {
        _coordinatesTextBox = coordinatesTextBox;
    }

    public Point? SelectNewPosition()
    {
        lock (_positionsLock)
        {
            var availablePositions = new List<Point>(_sharedTakenPositions);
            if (_currentPosition.HasValue)
            {
                availablePositions.Remove(_currentPosition.Value);
            }

            using var gridSelector = new GridSelectorForm(availablePositions, _currentPosition);
            if (gridSelector.ShowDialog() == DialogResult.OK && gridSelector.SelectedCoordinates.HasValue)
            {
                UpdatePosition(gridSelector.SelectedCoordinates.Value);
                return gridSelector.SelectedCoordinates;
            }
            return null;
        }
    }


    private void UpdatePosition(Point newPosition)
    {
        lock (_positionsLock)
        {
            if (_currentPosition.HasValue)
            {
                _sharedTakenPositions.Remove(_currentPosition.Value);
            }
            _currentPosition = newPosition;
            _sharedTakenPositions.Add(newPosition);
            _coordinatesTextBox.Text = $"{newPosition.X}_{newPosition.Y}";
        }
    }

    public void Cleanup()
    {
        lock (_positionsLock)
        {
            if (_currentPosition.HasValue)
            {
                _sharedTakenPositions.Remove(_currentPosition.Value);
                _currentPosition = null;
                _coordinatesTextBox.Text = string.Empty;
            }
        }
    }
}
