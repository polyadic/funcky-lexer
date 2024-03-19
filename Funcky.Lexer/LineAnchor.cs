namespace Funcky.Lexer;

public sealed record LineAnchor(int Line, int StartOfLine)
{
    public static LineAnchor DocumentStart { get; } = new(0, 0);
}