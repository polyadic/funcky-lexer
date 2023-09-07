using Funcky.Lexer.Token;

namespace Funcky.Lexer;

/// <summary>
/// A lexeme is representing a unit of characters called a token and its position.
/// </summary>
/// <param name="Token">A token is an abstract way to represent a specfic arrangment of characters.</param>
/// <param name="Position">The position represents the location of the arrangement of charcters from the beginning to the end.</param>
public record Lexeme(
    IToken Token,
    Position Position)
{
    public delegate Lexeme Factory(ILexemeBuilder builder);
}
