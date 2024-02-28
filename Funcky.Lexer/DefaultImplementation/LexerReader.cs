using Funcky.Monads;

namespace Funcky.Lexer.DefaultImplementation;

internal sealed class LexerReader(string expression) : ILexerReader
{
    public int Position { get; private set; }

    public Option<char> Peek(int lookAhead = 0)
        => PeekAt(Position + lookAhead);

    public Option<char> Read()
    {
        var result = Peek();

        Position++;

        return result;
    }

    private Option<char> PeekAt(int position)
        => position >= 0 && position < expression.Length
            ? expression[position]
            : Option<char>.None;
}