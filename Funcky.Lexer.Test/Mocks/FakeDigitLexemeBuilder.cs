using System.Text;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.Test.Mocks;

internal sealed class FakeDigitLexemeBuilder(ILexerReader reader, ILinePositionCalculator linePositionCalculator)
    : ILexemeBuilder
{
    private readonly int _startPosition = reader.Position;
    private readonly StringBuilder _stringBuilder = new();

    public string CurrentToken
        => _stringBuilder.ToString();

    public int Position
        => reader.Position;

    public Lexeme Build(IToken token)
        => new(
            Token: token,
            Position: linePositionCalculator.CalculateLinePosition(_startPosition, Length()));

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

    private int Length()
        => reader.Position - _startPosition;

    private static char TransformDigit(char maybeDigit)
        => char.IsDigit(maybeDigit)
            ? '4'
            : maybeDigit;
}