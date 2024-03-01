using System.Text;
using Funcky.Lexer.Token;
using Funcky.Monads;
using static Funcky.Lexer.Constants;

namespace Funcky.Lexer.DefaultImplementation;

internal sealed class LexemeBuilder(ILexerReader reader, LineAnchor currentLine)
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

    public Option<char> Peek(int lookAhead = NoLookAhead)
        => reader.Peek(lookAhead);

    public ILexemeBuilder Retain()
    {
        _ = reader.Read().AndThen(_stringBuilder.Append);

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
}
