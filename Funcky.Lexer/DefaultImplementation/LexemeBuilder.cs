using System.Text;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.DefaultImplementation;

internal sealed class LexemeBuilder : ILexemeBuilder
{
    private readonly ILinePositionCalculator _linePositionCalculator;
    private readonly int _startPosition;
    private readonly StringBuilder _stringBuilder = new();
    private readonly ILexerReader _reader;

    public LexemeBuilder(ILexerReader reader, ILinePositionCalculator linePositionCalculator)
    {
        _linePositionCalculator = linePositionCalculator;
        _startPosition = reader.Position;
        _reader = reader;
    }

    public string CurrentToken
        => _stringBuilder.ToString();

    public int Position
        => _reader.Position;

    public Lexeme Build(IToken token)
        => new(
            Token: token,
            Position: _linePositionCalculator.CalculateLinePosition(_startPosition, Length()));

    public Option<char> Peek(int lookAhead = 0)
        => _reader.Peek(lookAhead);

    public ILexemeBuilder Retain()
    {
        _ = _reader.Read().AndThen(_stringBuilder.Append);

        return this;
    }

    public ILexemeBuilder Discard()
    {
        _reader.Read();

        return this;
    }

    public ILexemeBuilder Clear()
    {
        _stringBuilder.Clear();

        return this;
    }

    private int Length()
        => _reader.Position - _startPosition;
}
