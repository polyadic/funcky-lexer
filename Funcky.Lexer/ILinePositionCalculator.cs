namespace Funcky.Lexer;

public interface ILinePositionCalculator
{
    delegate ILinePositionCalculator Factory(IReadOnlyList<Lexeme> lexemes);

    LinePosition CalculateLinePosition(int absolutePosition, int length);
}