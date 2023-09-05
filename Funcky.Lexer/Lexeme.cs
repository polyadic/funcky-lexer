using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public record Lexeme(
    IToken Token,
    AbsolutePosition AbsolutePosition,
    bool IsLineBreak,
    LinePosition LinePosition)
{
    public delegate Lexeme Factory(ILexemeBuilder builder);
}
