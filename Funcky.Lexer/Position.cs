namespace Funcky.Lexer;

/// <summary>
/// Represents the position of a Lexeme.
/// </summary>
public readonly record struct Position(int StartPosition, int Line, int StartColumn, int Length)
{
    /// <summary>
    /// Represents the position of the first character after the lexeme, countent in number of characters.
    /// </summary>
    public int EndPosition { get; } = StartPosition + Length;

    /// <summary>
    /// Represents the position of the first character after the lexeme, countent in number of characters.
    /// </summary>
    public int EndColumn { get; } = StartColumn + Length;
}