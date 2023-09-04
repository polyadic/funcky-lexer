using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.ArithmeticParser;
public sealed class LexerTest
{
    [Theory]
    [MemberData(nameof(SourceAndExpectedLExemes))]
    public void GivenSymbolsWithOverlappingPrefixesTheLexerGetsTheLongerOne(string expression, List<Lexeme> expectedLexemes)
    {
        var result = Funcky.Lexer.Test.ArithmeticParser.LexerRules
            .GetRuleBook()
            .Scan(expression);

        foreach (var expectedLexeme in expectedLexemes)
        {
            Assert.Equal(expectedLexeme, result.Walker.Pop());
        }
    }

    public static TheoryData<string, List<Lexeme>> SourceAndExpectedLExemes()
        => new()
        {
            {
                "6*(10+20+30)",
                new List<Lexeme>
                {
                    new(new NumberToken(6), new AbsolutePosition(0, 1), false, new LinePosition(1,1,1)),
                    new(new MultiplicationToken(), new AbsolutePosition(1, 1), false, new LinePosition(1,2,1)),
                    new(new OpenParenthesisToken(), new AbsolutePosition(2, 1), false, new LinePosition(1,3,1)),
                    new(new NumberToken(10), new AbsolutePosition(3, 2), false, new LinePosition(1,4,2)),
                    new(new PlusToken(), new AbsolutePosition(5, 1), false, new LinePosition(1,6,1)),
                    new(new NumberToken(20), new AbsolutePosition(6, 2), false, new LinePosition(1,7,2)),
                    new(new PlusToken(), new AbsolutePosition(8, 1), false, new LinePosition(1,9,1)),
                    new(new NumberToken(30), new AbsolutePosition(9, 2), false, new LinePosition(1,10,2)),
                    new(new ClosedParenthesisToken(), new AbsolutePosition(11, 1), false, new LinePosition(1,12,1)),
                    new(new EpsilonToken(), new AbsolutePosition(12, 0), false, new LinePosition(1,13,0)),
                }
            },
            {
                "(42.1337)",
                new List<Lexeme>
                {
                    new(new OpenParenthesisToken(), new AbsolutePosition(0, 1), false, new LinePosition(1,1,1)),
                    new(new NumberToken(42.1337), new AbsolutePosition(1, 7), false, new LinePosition(1,2,7)),
                    new(new ClosedParenthesisToken(), new AbsolutePosition(8, 1), false, new LinePosition(1,9,1)),
                    new(new EpsilonToken(), new AbsolutePosition(9, 0), false, new LinePosition(1,10,0)),
                }
            },
            {
                string.Empty,
                new List<Lexeme>
                {
                    new(new EpsilonToken(), new AbsolutePosition(0, 0), false, new LinePosition(1,1,0)),
                    new(new EpsilonToken(), new AbsolutePosition(0, 0), false, new LinePosition(1,1,0)),
                    new(new EpsilonToken(), new AbsolutePosition(0, 0), false, new LinePosition(1,1,0)),
                    new(new EpsilonToken(), new AbsolutePosition(0, 0), false, new LinePosition(1,1,0)),
                }
            },
        };
}
