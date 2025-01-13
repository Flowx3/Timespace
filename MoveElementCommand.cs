using System.Windows.Input;
using TimeSpace;

public class MoveElementCommand : ICommand
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
