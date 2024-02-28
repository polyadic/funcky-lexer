﻿using System.Diagnostics;
using Funcky.Extensions;
using Funcky.Lexer.Token;

namespace Funcky.Lexer.DefaultImplementation;

internal sealed class LexemeWalker(IReadOnlyList<Lexeme> lexemes, IEpsilonToken.Factory newEpsilonToken)
    : ILexemeWalker
{
    private const int EpsilonLength = 0;

    private int _currentIndex;

    private Position EpsilonAbsolutePosition
        => lexemes.LastOrNone()
            .Match(
                none: new Position(0, EpsilonLength, LineAnchor.DocumentStart),
                some: lexem => new Position(lexem.Position.EndPosition, EpsilonLength, lexem.Position.LineBeginnig));

    public Lexeme Pop()
        => ValidToken()
            ? lexemes[_currentIndex++]
            : CreateEpsilon();

    public Lexeme Peek(int lookAhead = 0)
    {
        Debug.Assert(lookAhead >= 0, "a negative look ahead is not supported");

        return ValidToken(lookAhead)
            ? lexemes[_currentIndex + lookAhead]
            : CreateEpsilon();
    }

    private bool ValidToken(int lookAhead = 0)
        => _currentIndex + lookAhead < lexemes.Count;

    private Lexeme CreateEpsilon()
        => new(
            Token: newEpsilonToken(),
            Position: EpsilonAbsolutePosition);
}