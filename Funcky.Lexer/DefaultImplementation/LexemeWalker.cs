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

    private Position EpsilonAbsolutePosition
        => _lexemes.LastOrNone()
            .Match(
                none: new Position(0, 1, 1, EpsilonLength),
                some: lexem => new Position(lexem.Position.EndPosition, lexem.Position.Line, lexem.Position.EndColumn, EpsilonLength));

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
            Position: EpsilonAbsolutePosition,
            IsLineBreak: false);
}