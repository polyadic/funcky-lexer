using System.Collections.Immutable;
using Funcky.Extensions;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.DefaultImplementation;

internal sealed class LinePositionCalculator : ILinePositionCalculator
{
    private readonly ImmutableList<int> _newLines;

    public LinePositionCalculator(IEnumerable<Lexeme> lexemes)
        => _newLines = lexemes
            .WhereSelect(ToLineBreakPosition)
            .ToImmutableList();

    public Position CalculateLinePosition(int absolutePosition, int length)
        => new(
            StartPosition: absolutePosition,
            Line: ToHumanIndex(LineNumber(absolutePosition)),
            StartColumn: ToHumanIndex(absolutePosition - FindClosestNewLineBefore(absolutePosition)),
            Length: length);

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

    private static Option<int> ToLineBreakPosition(Lexeme lexeme)
        => lexeme.Token is ILineBreakToken
            ? lexeme.Position.EndPosition
            : Option<int>.None;
}
