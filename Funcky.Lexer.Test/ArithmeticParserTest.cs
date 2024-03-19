using Funcky.Lexer.Test.LexerRules;
using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test;

public sealed class ArithmeticParserTest
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
                [
                    new(new NumberToken(6), new Position(0, 1, LineAnchor.DocumentStart)),
                    new(new MultiplicationToken(), new Position(1, 1, LineAnchor.DocumentStart)),
                    new(new OpenParenthesisToken(), new Position(2, 1, LineAnchor.DocumentStart)),
                    new(new NumberToken(10), new Position(3, 2, LineAnchor.DocumentStart)),
                    new(new PlusToken(), new Position(5, 1, LineAnchor.DocumentStart)),
                    new(new NumberToken(20), new Position(6, 2, LineAnchor.DocumentStart)),
                    new(new PlusToken(), new Position(8, 1, LineAnchor.DocumentStart)),
                    new(new NumberToken(30), new Position(9, 2, LineAnchor.DocumentStart)),
                    new(new ClosedParenthesisToken(), new Position(11, 1, LineAnchor.DocumentStart)),
                    new(new EpsilonToken(), new Position(12, 0, LineAnchor.DocumentStart)),
                ]
            },
            {
                "(42.1337)",
                [
                    new(new OpenParenthesisToken(), new Position(0, 1, LineAnchor.DocumentStart)),
                    new(new NumberToken(42.1337), new Position(1, 7, LineAnchor.DocumentStart)),
                    new(new ClosedParenthesisToken(), new Position(8, 1, LineAnchor.DocumentStart)),
                    new(new EpsilonToken(), new Position(9, 0, LineAnchor.DocumentStart)),
                ]
            },
            {
                string.Empty,
                [
                    new(new EpsilonToken(), new Position(0, 0, LineAnchor.DocumentStart)),
                    new(new EpsilonToken(), new Position(0, 0, LineAnchor.DocumentStart)),
                    new(new EpsilonToken(), new Position(0, 0, LineAnchor.DocumentStart)),
                    new(new EpsilonToken(), new Position(0, 0, LineAnchor.DocumentStart)),
                ]
            },
        };

    private static ILexemeWalker GetLexemeWalker(string expression)
        => ArithmeticLexerRules.GetRules()
            .Build()
            .Scan(expression)
            .Walker;
}
