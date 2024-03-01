using Funcky.Monads;
using static Funcky.Lexer.Constants;

namespace Funcky.Lexer.DefaultImplementation;

internal sealed class LexerReader(string expression) : ILexerReader
{
    public int Position { get; private set; }

    public Option<char> Peek(int lookAhead = NoLookAhead)
        => PeekAt(Position + lookAhead);

    public Option<char> Read()
    {
        var result = Peek();

        Position++;

        return result;
    }

    private Option<char> PeekAt(int position)
        => position >= NoLookAhead && position < expression.Length
            ? expression[position]
            : Option<char>.None;
}