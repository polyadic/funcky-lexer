namespace Funcky.Lexer;

public sealed record LinePosition(
    int Line,
    int Column,
    int Length);