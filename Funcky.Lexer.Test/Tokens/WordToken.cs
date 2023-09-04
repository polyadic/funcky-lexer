using Funcky.Lexer.Token;

namespace Funcky.Lexer.Test.Tokens;

internal sealed record WordToken(string Word) : IToken
{
    public override string ToString()
    {
        return Word;
    }
}