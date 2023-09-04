namespace Funcky.Lexer;

/// <summary>
/// Represents the position of a Lexeme.
/// </summary>
public readonly record struct AbsolutePosition(int StartPosition, int Length)
{
    /// <summary>
    /// Represents the position of the first character of the lexeme, countent in number of characters.
    /// </summary>
    public int StartPosition { get; } = StartPosition;

    /// <summary>
    /// Represents the position of the first character after the lexeme, countent in number of characters.
    /// </summary>
    public int EndPosition { get; } = StartPosition + Length;

    /// <summary>
    /// Represents the length of the lexeme.
    /// </summary>
    public int Length { get; } = Length;
}