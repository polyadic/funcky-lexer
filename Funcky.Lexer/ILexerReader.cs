using Funcky.Lexer.Default;
using Funcky.Monads;

namespace Funcky.Lexer;

public interface ILexerReader
{
    public delegate ILexerReader Factory(string expression);

    public static Factory DefaultFactory = expression => new LexerReader(expression);

    int Position { get; }

    Option<char> Peek(int lookAhead = 0);

    Option<char> Read();
}