using System.Text;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.Test.Mocks;

internal sealed class FakeDigitLexemeBuilder(ILexerReader reader, LineAnchor currentLine)
    : ILexemeBuilder
{
    private readonly int _startPosition = reader.Position;
    private readonly StringBuilder _stringBuilder = new();

    public string CurrentToken
        => _stringBuilder.ToString();

    public int Position
        => reader.Position;

    private int Length
        => reader.Position - _startPosition;

    public Lexeme Build(IToken token)
        => new(
            Token: token,
            Position: new Position(_startPosition, Length, currentLine));

    public Option<char> Peek(int lookAhead = 0)
        => reader.Peek(lookAhead);

    public ILexemeBuilder Retain()
    {
        _ = reader.Read().Select(TransformDigit).AndThen(_stringBuilder.Append);

        return this;
    }

    public ILexemeBuilder Discard()
    {
        reader.Read();

        return this;
    }

    public ILexemeBuilder Clear()
    {
        _stringBuilder.Clear();

        return this;
    }

    private static char TransformDigit(char maybeDigit)
        => char.IsDigit(maybeDigit)
            ? '4'
            : maybeDigit;
}