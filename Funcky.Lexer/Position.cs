namespace Funcky.Lexer;

/// <summary>
/// Represents the position of a Lexeme.
/// </summary>
public readonly record struct Position
{
    public Position(int startPosition, int length, LineAnchor lineBeginnig)
    {
        StartPosition = startPosition;
        EndPosition = startPosition + length;
        Length = length;
        Line = ToHumanIndex(lineBeginnig.Line);
        StartColumn = ToHumanIndex(startPosition - lineBeginnig.StartOfLine);
        EndColumn = ToHumanIndex(startPosition - lineBeginnig.StartOfLine + length);
        LineBeginnig = lineBeginnig;
    }

    /// <summary>
    /// Represents the position of the first character of the lexeme, counted in number of characters.
    /// </summary>
    public int StartPosition { get; }

    /// <summary>
    /// Represents the position of the first character after the lexeme, counted in number of characters.
    /// </summary>
    public int EndPosition { get; }

    /// <summary>
    /// Represents the length of the lexeme, counted in number of characters.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Repesents the one-indexed row of the beginning of this lexeme.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Repesents the one-indexed column of the beginning of this lexeme, counted in number of characters from last line-ending.
    /// </summary>
    public int StartColumn { get; }

    /// <summary>
    /// Represents the position of the first character after the lexeme, counted in number of characters.
    /// </summary>
    public int EndColumn { get; }

    /// <summary>
    /// The line of the current lexeme starts here.
    /// </summary>
    public LineAnchor LineBeginnig { get; }

    private static int ToHumanIndex(int index)
        => index + 1;
}