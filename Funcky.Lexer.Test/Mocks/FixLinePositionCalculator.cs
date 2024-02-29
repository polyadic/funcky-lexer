namespace Funcky.Lexer.Test.Mocks;

internal sealed class FixLinePositionCalculator(int lineNumber) : ILinePositionCalculator
{
    public Position CalculateLinePosition(int absolutePosition, int length)
        => new(absolutePosition, lineNumber, absolutePosition, length);
}