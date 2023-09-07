using Funcky.Lexer.Token;

namespace Funcky.Lexer.Test.Mocks;

internal sealed class EpsilonLexemeWalker : ILexemeWalker
{
    private readonly IEpsilonToken.Factory _newEpsilonToken;

    public EpsilonLexemeWalker(IEpsilonToken.Factory newEpsilonToken)
    {
        _newEpsilonToken = newEpsilonToken;
    }

    public Lexeme Pop()
    {
        return new Lexeme(_newEpsilonToken(), new Position(0, 0, 0, 0), false);
    }

    public Lexeme Peek(int lookAhead = 0)
    {
        return new Lexeme(_newEpsilonToken(), new Position(0, 0, 0, 0), false);
    }
}