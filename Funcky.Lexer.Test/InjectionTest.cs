﻿using Funcky.Lexer.Test.LexerRules;
using Funcky.Lexer.Test.Mocks;
using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test;

public sealed class InjectionTest
{
    [Fact]
    public void UseDifferentLexerReader()
    {
        var result = ArithmeticLexerRules.GetRules()
            .WithLexerReader(_ => new EmptyLexerReader())
            .Build()
            .Scan("40+20*6");

        // The EmptyLexerReader discards the input.
        Assert.Empty(result.Lexemes);
    }

    [Fact]
    public void UseDifferentLexemeWalker()
    {
        var result = ArithmeticLexerRules.GetRules()
            .WithLexemeWalker((_, _) => new EpsilonLexemeWalker(() => new DifferentEpsilonToken()))
            .Build()
            .Scan("41*20/6");

        // The EpsilonLexemeWalker always returns the DifferentEpsilonToken.
        Assert.IsType<DifferentEpsilonToken>(result.Walker.Pop().Token);
    }

    [Fact]
    public void UseDifferentLexemeBuilder()
    {
        var result = ArithmeticLexerRules.GetRules()
            .WithLexemeBuilder((reader, linePositionCalculator) => new FakeDigitLexemeBuilder(reader, linePositionCalculator))
            .Build()
            .Scan("40+20*6");

        // The FakeDigitLexemeBuilder's Retain() call transforms any digit to a '4'
        Assert.Collection(
            result.Lexemes,
            lexeme => Assert.Equal(new Lexeme(new NumberToken(44), new Position(0, 2, LineAnchor.DocumentStart)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new PlusToken(), new Position(2, 1, LineAnchor.DocumentStart)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(44), new Position(3, 2, LineAnchor.DocumentStart)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new MultiplicationToken(), new Position(5, 1, LineAnchor.DocumentStart)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(4), new Position(6, 1, LineAnchor.DocumentStart)), lexeme));
    }

    [Fact]
    public void WhenGivenAPostProcessFunctionTheLexemesAreChangedAccordingly()
    {
        var sequence = Sequence.Return(new Lexeme(new NumberToken(1337.42), new Position(0, 7, LineAnchor.DocumentStart)));

        var result = ArithmeticLexerRules.GetRules()
            .WithPostProcess(_ => sequence)
            .Build()
            .Scan("84/6+17");

        Assert.Equal(sequence, result.Lexemes);
    }
}
