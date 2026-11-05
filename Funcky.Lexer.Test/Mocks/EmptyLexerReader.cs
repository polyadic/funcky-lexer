using Funcky.Monads;
using static Funcky.Lexer.Constants;

namespace Funcky.Lexer.Test.Mocks;

internal sealed class EmptyLexerReader : ILexerReader
{
    public int Position => LineAnchor.DocumentStart.StartOfLine;

    public Option<char> Peek(int lookAhead = NoLookAhead)
        => Option<char>.None;

    public Option<char> Read()
        => Option<char>.None;
}