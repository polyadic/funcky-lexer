using System.Collections.Immutable;

namespace Funcky.Lexer;

public class LexemeWalker : ILexemeWalker
{
    private readonly ImmutableList<Lexeme> _lexmes;

    public LexemeWalker(ImmutableList<Lexeme> lexmes)
    {
        _lexmes = lexmes;
    }

    public Lexeme Pop()
    {
        throw new NotImplementedException();
    }

    public Lexeme Peek(int lookAhead = 0)
    {
        throw new NotImplementedException();
    }
}