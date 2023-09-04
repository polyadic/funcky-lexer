using Funcky.Lexer.Default;

namespace Funcky.Lexer;

public interface ILinePositionCalculator
{
    delegate ILinePositionCalculator Factory(IReadOnlyList<Lexeme> lexemes);

    public static Factory DefaultFactory = lexemes => new LinePositionCalculator(lexemes);

    LinePosition CalculateLinePosition(int absolutePosition, int length);
}