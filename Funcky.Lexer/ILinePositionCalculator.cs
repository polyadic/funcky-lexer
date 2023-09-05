using Funcky.Lexer.Default;

namespace Funcky.Lexer;

public interface ILinePositionCalculator
{
    public static Factory DefaultFactory = lexemes => new LinePositionCalculator(lexemes);

    public delegate ILinePositionCalculator Factory(IReadOnlyList<Lexeme> lexemes);

    LinePosition CalculateLinePosition(int absolutePosition, int length);
}