namespace Funcky.Lexer;

public record Lexeme(
    IToken Token,
    AbsolutePosition AbsolutePosition,
    bool IsLineBreak,
    LinePosition LinePosition);