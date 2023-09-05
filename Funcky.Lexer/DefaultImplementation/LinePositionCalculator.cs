using System.Collections.Immutable;

namespace Funcky.Lexer.Default;

internal class LinePositionCalculator : ILinePositionCalculator
{
    private readonly ImmutableList<AbsolutePosition> _newLines;

    public LinePositionCalculator(IEnumerable<Lexeme> lexemes)
        => _newLines = lexemes
            .Where(lexeme => lexeme.IsLineBreak)
            .Select(lexeme => lexeme.AbsolutePosition)
            .ToImmutableList();

    public LinePosition CalculateLinePosition(int absolutePosition, int length)
        => CalculateRelativePosition(
            LineNumber(absolutePosition),
            absolutePosition,
            length,
            FindClosestNewLineBefore(absolutePosition));

    private int LineNumber(int absolutePosition)
        => _newLines
            .Count(position => position.StartPosition < absolutePosition);

    private static LinePosition CalculateRelativePosition(int lineNumber, int absolutePosition, int length, AbsolutePosition newLinePosition)
        => new(
            ToHumanIndex(lineNumber),
            ToHumanIndex(absolutePosition - newLinePosition.EndPosition),
            length);

    private static int ToHumanIndex(int index)
        => index + 1;

    private AbsolutePosition FindClosestNewLineBefore(int position)
        => _newLines
            .LastOrDefault(newLinePosition => newLinePosition.StartPosition < position);
}
