using System.Diagnostics;
using Funcky.Extensions;
using Funcky.Lexer.Token;

namespace Funcky.Lexer.DefaultImplementation;

internal class LexemeWalker : ILexemeWalker
{
    private const int EpsilonLength = 0;

    private readonly IEpsilonToken.Factory _newEpsilonToken;

    private readonly IReadOnlyList<Lexeme> _lexemes;
    private int _currentIndex;

    public LexemeWalker(IReadOnlyList<Lexeme> lexemes, IEpsilonToken.Factory newEpsilonToken)
    {
        _lexemes = lexemes;
        _newEpsilonToken = newEpsilonToken;
    }

    private AbsolutePosition EpsilonAbsolutePosition
        => _lexemes.LastOrNone()
            .Match(
                none: new AbsolutePosition(0, EpsilonLength),
                some: lexem => new AbsolutePosition(lexem.AbsolutePosition.EndPosition, EpsilonLength));

    private LinePosition EpsilonLinePosition
        => _lexemes.LastOrNone()
            .Match(
                none: new LinePosition(1, 1, EpsilonLength),
                some: lexem => lexem.LinePosition with { Column = lexem.LinePosition.Column + lexem.LinePosition.Length, Length = EpsilonLength });

    public Lexeme Pop()
        => ValidToken()
            ? _lexemes[_currentIndex++]
            : CreateEpsilon();

    public Lexeme Peek(int lookAhead = 0)
    {
        Debug.Assert(lookAhead >= 0, "a negative look ahead is not supported");

        return ValidToken(lookAhead)
            ? _lexemes[_currentIndex + lookAhead]
            : CreateEpsilon();
    }

    private bool ValidToken(int lookAhead = 0)
        => _currentIndex + lookAhead < _lexemes.Count;

    private Lexeme CreateEpsilon()
        => new(
            Token: _newEpsilonToken(),
            AbsolutePosition: EpsilonAbsolutePosition,
            IsLineBreak: false,
            LinePosition: EpsilonLinePosition);
}