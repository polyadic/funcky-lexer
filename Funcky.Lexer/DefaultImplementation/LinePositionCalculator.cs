using System.Collections.Immutable;

namespace Funcky.Lexer.Default;

internal class LinePositionCalculator : ILinePositionCalculator
{
    private readonly ImmutableList<int> _newLines;

    public LinePositionCalculator(IEnumerable<Lexeme> lexemes)
        => _newLines = lexemes
            .Where(lexeme => lexeme.IsLineBreak)
            .Select(lexeme => lexeme.Position.EndPosition)
            .ToImmutableList();

    public Position CalculateLinePosition(int absolutePosition, int length)
        => new(
            absolutePosition,
            ToHumanIndex(LineNumber(absolutePosition)),
            ToHumanIndex(absolutePosition - FindClosestNewLineBefore(absolutePosition)),
            length);

    private int LineNumber(int position)
        => _newLines
            .Count(IsBeforeLineBreak(position));

    private int FindClosestNewLineBefore(int position)
        => _newLines
            .LastOrDefault(IsBeforeLineBreak(position));

    private static int ToHumanIndex(int index)
        => index + 1;

    private static Func<int, bool> IsBeforeLineBreak(int position)
        => newLinePosition
            => newLinePosition <= position;
}
