using Funcky.Lexer.DefaultImplementation;

namespace Funcky.Lexer;

public interface ILinePositionCalculator
{
    public static Factory DefaultFactory = lexemes => new LinePositionCalculator(lexemes);

    public delegate ILinePositionCalculator Factory(IReadOnlyList<Lexeme> lexemes);

    Position CalculateLinePosition(int absolutePosition, int length);
}