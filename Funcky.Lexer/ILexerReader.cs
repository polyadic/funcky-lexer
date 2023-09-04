using Funcky.Monads;

namespace Funcky.Lexer;

public interface ILexerReader
{
    public delegate ILexerReader Factory(string expression);

    int Position { get; }

    Option<char> Peek(int lookAhead = 0);

    Option<char> Read();
}