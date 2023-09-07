namespace Funcky.Lexer.Test.Mocks;

internal sealed class FixLinePositionCalculator : ILinePositionCalculator
{
    private readonly int _lineNumber;

    public FixLinePositionCalculator(int lineNumber)
    {
        _lineNumber = lineNumber;
    }

    public Position CalculateLinePosition(int absolutePosition, int length)
        => new(absolutePosition, _lineNumber, absolutePosition, length);
}