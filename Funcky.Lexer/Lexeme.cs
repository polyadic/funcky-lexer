using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public record Lexeme(
    IToken Token,
    Position Position,
    bool IsLineBreak)
{
    public delegate Lexeme Factory(ILexemeBuilder builder);
}
