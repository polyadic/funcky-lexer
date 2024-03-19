using Funcky.Lexer.Token;
using static Funcky.Lexer.Constants;

namespace Funcky.Lexer.Test.Mocks;

internal sealed class EpsilonLexemeWalker(IEpsilonToken.Factory newEpsilonToken) : ILexemeWalker
{
    public Lexeme Pop()
    {
        return new Lexeme(newEpsilonToken(), new Position(0, 0, LineAnchor.DocumentStart));
    }

    public Lexeme Peek(int lookAhead = NoLookAhead)
    {
        return new Lexeme(newEpsilonToken(), new Position(0, 0, LineAnchor.DocumentStart));
    }
}