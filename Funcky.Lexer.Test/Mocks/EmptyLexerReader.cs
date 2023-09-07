using Funcky.Monads;

namespace Funcky.Lexer.Test.Mocks;

internal sealed class EmptyLexerReader : ILexerReader
{
    public int Position => 0;

    public Option<char> Peek(int lookAhead = 0)
        => Option<char>.None;

    public Option<char> Read()
        => Option<char>.None;
}