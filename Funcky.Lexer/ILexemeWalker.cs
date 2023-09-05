using Funcky.Lexer.DefaultImplementation;
using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public interface ILexemeWalker
{
    public static Factory DefaultFactory = (lexemes, newEpsilonToken) => new LexemeWalker(lexemes, newEpsilonToken);

    public delegate ILexemeWalker Factory(IReadOnlyList<Lexeme> lexemes, IEpsilonToken.Factory newEpsilonToken);

    Lexeme Pop();

    Lexeme Peek(int lookAhead = 0);
}