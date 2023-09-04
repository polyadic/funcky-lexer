using Funcky.Lexer.Exceptions;
using Funcky.Lexer.Test.LexerRules;
using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test;

/// <summary>
/// Test to verify the functionality of the lexer.
/// </summary>
public sealed class LexerTest
{
    [Fact]
    public void GivenSymbolsWithOverlappingPrefixesTheLexerGetsTheLongerOne()
    {
        var rules = ExampleRules.GetRules();

        Assert.IsType<EqualToken>(rules.Scan("=").Lexemes.Single().Token);
        Assert.IsType<DoubleEqualToken>(rules.Scan("==").Lexemes.Single().Token);
        Assert.IsType<GreaterToken>(rules.Scan("<").Lexemes.Single().Token);
        Assert.IsType<GreaterEqualToken>(rules.Scan("<=").Lexemes.Single().Token);
        Assert.IsType<GreaterEqualToken>(rules.Scan("<===").Lexemes.First().Token);
        Assert.IsType<DoubleEqualToken>(rules.Scan("<===").Lexemes.Last().Token);
    }

    [Fact]
    public void GivenALexerRuleForIdentifiersDoNotReturKeyTokenInTheMiddle()
    {
        var rules = ExampleRules.GetRules();

        Assert.IsType<IdentifierToken>(rules.Scan("sand").Lexemes.Single().Token);
        Assert.IsType<IdentifierToken>(rules.Scan("andor").Lexemes.Single().Token);
        Assert.IsType<AndToken>(rules.Scan("and").Lexemes.Single().Token);
    }

    [Fact]
    public void GivenLexerRulesTheLexemesHaveTheRightPositions()
    {
        var rules = ExampleRules.GetRules();

        var lexemes = rules.Scan("love and sand and testing").Lexemes;
        Assert.Equal(9, lexemes.Count);

        Assert.Equal(0, lexemes[0].AbsolutePosition.StartPosition);
        Assert.Equal(4, lexemes[0].AbsolutePosition.Length);
        Assert.Equal(4, lexemes[0].AbsolutePosition.EndPosition);

        Assert.IsType<SpaceToken>(lexemes[3].Token);

        Assert.Equal(8, lexemes[3].AbsolutePosition.StartPosition);
        Assert.Equal(1, lexemes[3].AbsolutePosition.Length);
        Assert.Equal(9, lexemes[3].AbsolutePosition.EndPosition);

        Assert.IsType<AndToken>(lexemes[6].Token);

        Assert.Equal(14, lexemes[6].AbsolutePosition.StartPosition);
        Assert.Equal(3, lexemes[6].AbsolutePosition.Length);
        Assert.Equal(17, lexemes[6].AbsolutePosition.EndPosition);
    }

    [Fact]
    public void GivenALexerMissingAProductionForAGivenStringItShouldThrowAnException()
    {
        var rules = LexerRuleBook.Builder.Build();

        Assert.Throws<UnknownTokenException>(() => rules.Scan("You can't tokenize this!"));
    }

    [Fact]
    public void GivenAStringAndAMissingTokenizerThrows()
    {
        var rules = RulesWithContext.GetRules();

        var exception = Assert.Throws<UnknownTokenException>(() => rules.Scan("aa aa cc aa UU cc aa"));

        Assert.Equal("Unknown Token 'U' at Line 1 Column 13", exception.Message);
    }

    [Fact]
    public void GivenALexerAndAContextedLexerRuleGenerateTokenContexted()
    {
        var rules = RulesWithContext.GetRules();

        var lexemes = rules.Scan("aa aa cc aa bb cc aa").Lexemes;

        Assert.Equal(13, lexemes.Count);

        Assert.IsType<AaToken>(lexemes[0].Token);
        Assert.IsType<SpaceToken>(lexemes[1].Token);
        Assert.IsType<AaToken>(lexemes[2].Token);
        Assert.IsType<SpaceToken>(lexemes[3].Token);

        // The first cc produces a CcToken
        Assert.IsType<CcToken>(lexemes[4].Token);
        Assert.IsType<SpaceToken>(lexemes[5].Token);
        Assert.IsType<AaToken>(lexemes[6].Token);
        Assert.IsType<SpaceToken>(lexemes[7].Token);
        Assert.IsType<BbToken>(lexemes[8].Token);
        Assert.IsType<SpaceToken>(lexemes[9].Token);

        // The second cc produces a CcAfterBbToken because there is a BbToken already produced
        Assert.IsType<CcAfterBbToken>(lexemes[10].Token);
        Assert.IsType<SpaceToken>(lexemes[11].Token);
        Assert.IsType<AaToken>(lexemes[12].Token);
    }

    [Fact]
    public void EmptyTokenWalkerDoesNotThrowOnPeek()
    {
        var rules = LexerRuleBook.Builder.WithEpsilonToken<EpsilonToken>().Build();

        Assert.IsType<EpsilonToken>(rules.Scan(string.Empty).Walker.Peek().Token);
    }
}