using System.Collections.Immutable;
using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.Api;

public class DefineLexerApi
{
    [Fact]
    public void DefineHowTheApiShouldBeUsed()
    {
        LexerResult result = LexerRuleBook.Builder
            .WithLexemeBuilder(ILexemeBuilder.DefaultFactory)
            .WithLexerReader(ILexerReader.DefaultFactory)
            .WithLinePositionCalculator(ILinePositionCalculator.DefaultFactory)
            .WithLexemeWalker(ILexemeWalker.DefaultFactory)
            .WithEpsilonToken<EpsilonToken>()
            .AddRule(char.IsDigit, ScanNumber)
            .AddRule(char.IsLetter, ScanIdentifier)
            .AddSimpleRule<MinusToken>("-")
            .AddSimpleRule<PlusToken>("+")
            .AddSimpleRule<MultiplicationToken>("*")
            .AddSimpleRule<DivisionToken>("/")
            .Build()
            .Scan("40+20*6");

        ImmutableList<Lexeme> lexemes = result.Lexemes;

        Assert.Collection(lexemes,
            lexeme => Assert.Equal(new Lexeme(new NumberToken(40), new AbsolutePosition(0, 2), false, new LinePosition(1, 1, 2)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new PlusToken(), new AbsolutePosition(2, 1), false, new LinePosition(1, 3, 1)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(20), new AbsolutePosition(3, 2), false, new LinePosition(1, 4, 2)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new MultiplicationToken(), new AbsolutePosition(5, 1), false, new LinePosition(1, 6, 1)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(6), new AbsolutePosition(6, 1), false, new LinePosition(1, 7, 1)), lexeme));


        ILexemeWalker walker = result.Walker;

        Assert.Equal(new Lexeme(new NumberToken(40), new AbsolutePosition(0, 2), false, new LinePosition(1, 1, 2)), walker.Pop());
        Assert.Equal(new Lexeme(new PlusToken(), new AbsolutePosition(2, 1), false, new LinePosition(1, 3, 1)), walker.Pop());
        Assert.Equal(new Lexeme(new NumberToken(20), new AbsolutePosition(3, 2), false, new LinePosition(1, 4, 2)), walker.Pop());
        Assert.Equal(new Lexeme(new MultiplicationToken(), new AbsolutePosition(5, 1), false, new LinePosition(1, 6, 1)), walker.Pop());
        Assert.Equal(new Lexeme(new NumberToken(6), new AbsolutePosition(6, 1), false, new LinePosition(1, 7, 1)), walker.Pop());
        Assert.Equal(new Lexeme(new EpsilonToken(), new AbsolutePosition(7, 0), false, new LinePosition(1, 8, 0)), walker.Pop());
        Assert.Equal(new Lexeme(new EpsilonToken(), new AbsolutePosition(7, 0), false, new LinePosition(1, 8, 0)), walker.Pop());
    }

    private static Lexeme ScanNumber(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsDigit)
            ? ScanNumber(builder.Retain())
            : builder.Build(new NumberToken(int.Parse(builder.CurrentToken)));

    private static Lexeme ScanIdentifier(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsLetterOrDigit)
            ? ScanIdentifier(builder.Retain())
            : builder.Build(new IdentifierToken(builder.CurrentToken));
}

