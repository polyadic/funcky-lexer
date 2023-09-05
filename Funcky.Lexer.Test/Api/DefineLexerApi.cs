using System.Collections.Immutable;
using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.Api;

public class DefineLexerApi
{
    [Fact]
    public void DefineHowTheApiShouldBeUsed()
    {
        LexerResult result = LexerRuleBook.Builder
            .WithEpsilonToken<EpsilonToken>()
            .WithLexemeBuilder(ILexemeBuilder.DefaultFactory)
            .WithLexerReader(ILexerReader.DefaultFactory)
            .WithLinePositionCalculator(ILinePositionCalculator.DefaultFactory)
            .WithLexemeWalker(ILexemeWalker.DefaultFactory)
            .AddRule(char.IsDigit, ScanNumber)
            .AddRule(char.IsLetter, ScanIdentifier)
            .AddSimpleRule<MinusToken>("-")
            .AddSimpleRule<PlusToken>("+")
            .AddSimpleRule<MultiplicationToken>("*")
            .AddSimpleRule<DivisionToken>("/")
            .Build()
            .Scan("40+20*6");

        ImmutableList<Lexeme> lexemes = result.Lexemes;

        Assert.Collection(
            lexemes,
            lexeme => Assert.Equal(new Lexeme(new NumberToken(40), new Position(0, 1, 1, 2), false), lexeme),
            lexeme => Assert.Equal(new Lexeme(new PlusToken(), new Position(2, 1, 3, 1), false), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(20), new Position(3, 1, 4, 2), false), lexeme),
            lexeme => Assert.Equal(new Lexeme(new MultiplicationToken(), new Position(5, 1, 6, 1), false), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(6), new Position(6, 1, 7, 1), false), lexeme));

        ILexemeWalker walker = result.Walker;

        Assert.Equal(new Lexeme(new NumberToken(40), new Position(0, 1, 1, 2), false), walker.Pop());
        Assert.Equal(new Lexeme(new PlusToken(), new Position(2, 1, 3, 1), false), walker.Pop());
        Assert.Equal(new Lexeme(new NumberToken(20), new Position(3, 1, 4, 2), false), walker.Pop());
        Assert.Equal(new Lexeme(new MultiplicationToken(), new Position(5, 1, 6, 1), false), walker.Pop());
        Assert.Equal(new Lexeme(new NumberToken(6), new Position(6, 1, 7, 1), false), walker.Pop());
        Assert.Equal(new Lexeme(new EpsilonToken(), new Position(7, 1, 8, 0), false), walker.Pop());
        Assert.Equal(new Lexeme(new EpsilonToken(), new Position(7, 1, 8, 0), false), walker.Pop());
    }

    [Fact]
    public void WhenGivenAPostProcessFunctionTheLexmesAreChangedAccordingly()
    {
        LexerResult result = LexerRuleBook.Builder
            .WithEpsilonToken<EpsilonToken>()
            .WithLexemeBuilder(ILexemeBuilder.DefaultFactory)
            .WithLexerReader(ILexerReader.DefaultFactory)
            .WithLinePositionCalculator(ILinePositionCalculator.DefaultFactory)
            .WithLexemeWalker(ILexemeWalker.DefaultFactory)
            .AddRule(char.IsDigit, ScanNumber)
            .AddRule(char.IsLetter, ScanIdentifier)
            .AddSimpleRule<MinusToken>("-")
            .AddSimpleRule<PlusToken>("+")
            .AddSimpleRule<MultiplicationToken>("*")
            .AddSimpleRule<DivisionToken>("/")
            .WithPostProcess(_ => Enumerable.Empty<Lexeme>())
            .Build()
            .Scan("84/6+17");

        Assert.Empty(result.Lexemes);
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
