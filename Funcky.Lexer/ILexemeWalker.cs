using Funcky.Lexer.Token;
using Funcky.Lexer.DefaultImplementation;

namespace Funcky.Lexer;

public interface ILexemeWalker
{
    delegate ILexemeWalker Factory(IReadOnlyList<Lexeme> lexemes, IEpsilonToken.Factory newEpsilonToken);

    static Factory DefaultFactory = (lexemes, newEpsilonToken) => new LexemeWalker(lexemes, newEpsilonToken);

    Lexeme Pop();

    Lexeme Peek(int lookAhead = 0);
}