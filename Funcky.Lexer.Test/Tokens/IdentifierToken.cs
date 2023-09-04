using Funcky.Lexer.Token;

namespace Funcky.Lexer.Test.Tokens;

public record IdentifierToken(string Name) : IToken
{
    public override string ToString() => $"Identifier: {Name}";
}