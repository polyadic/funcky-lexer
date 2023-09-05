using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.ArithmeticParser;

public sealed class LexerTest
{
    [Theory]
    [MemberData(nameof(SourceAndExpectedLExemes))]
    public void GivenSymbolsWithOverlappingPrefixesTheLexerGetsTheLongerOne(string expression, List<Lexeme> expectedLexemes)
    {
        var walker = GetLexemeWalker(expression);

        foreach (var expectedLexeme in expectedLexemes)
        {
            Assert.Equal(expectedLexeme, walker.Pop());
        }
    }

    public static TheoryData<string, List<Lexeme>> SourceAndExpectedLExemes()
        => new()
        {
            {
                "6*(10+20+30)",
                new List<Lexeme>
                {
                    new(new NumberToken(6), new Position(0, 1, 1, 1), false),
                    new(new MultiplicationToken(), new Position(1, 1, 2, 1), false),
                    new(new OpenParenthesisToken(), new Position(2, 1, 3, 1), false),
                    new(new NumberToken(10), new Position(3, 1, 4, 2), false),
                    new(new PlusToken(), new Position(5, 1, 6, 1), false),
                    new(new NumberToken(20), new Position(6, 1, 7, 2), false),
                    new(new PlusToken(), new Position(8, 1, 9, 1), false),
                    new(new NumberToken(30), new Position(9, 1, 10, 2), false),
                    new(new ClosedParenthesisToken(), new Position(11, 1, 12, 1), false),
                    new(new EpsilonToken(), new Position(12, 1, 13, 0), false),
                }
            },
            {
                "(42.1337)",
                new List<Lexeme>
                {
                    new(new OpenParenthesisToken(), new Position(0, 1, 1, 1), false),
                    new(new NumberToken(42.1337), new Position(1, 1, 2, 7), false),
                    new(new ClosedParenthesisToken(), new Position(8, 1, 9, 1), false),
                    new(new EpsilonToken(), new Position(9, 1, 10, 0), false),
                }
            },
            {
                string.Empty,
                new List<Lexeme>
                {
                    new(new EpsilonToken(), new Position(0, 1, 1, 0), false),
                    new(new EpsilonToken(), new Position(0, 1, 1, 0), false),
                    new(new EpsilonToken(), new Position(0, 1, 1, 0), false),
                    new(new EpsilonToken(), new Position(0, 1, 1, 0), false),
                }
            },
        };

    private static ILexemeWalker GetLexemeWalker(string expression)
        => LexerRules.GetRuleBook()
            .Scan(expression)
            .Walker;
}
