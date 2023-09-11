using System.Collections.Immutable;
using Funcky.Lexer.Test.Tokens;
using static Funcky.Functional;

namespace Funcky.Lexer.Test;

public sealed class DefineLexerApi
{
    private const string ClassExpression = """
        class MySuperClass
        {
            function DoSomething
            {
               let class = anything
            }
        }
        """;

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
            lexeme => Assert.Equal(new Lexeme(new NumberToken(40), new Position(0, 1, 1, 2)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new PlusToken(), new Position(2, 1, 3, 1)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(20), new Position(3, 1, 4, 2)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new MultiplicationToken(), new Position(5, 1, 6, 1)), lexeme),
            lexeme => Assert.Equal(new Lexeme(new NumberToken(6), new Position(6, 1, 7, 1)), lexeme));

        ILexemeWalker walker = result.Walker;

        Assert.Equal(new Lexeme(new NumberToken(40), new Position(0, 1, 1, 2)), walker.Pop());
        Assert.Equal(new Lexeme(new PlusToken(), new Position(2, 1, 3, 1)), walker.Pop());
        Assert.Equal(new Lexeme(new NumberToken(20), new Position(3, 1, 4, 2)), walker.Pop());
        Assert.Equal(new Lexeme(new MultiplicationToken(), new Position(5, 1, 6, 1)), walker.Pop());
        Assert.Equal(new Lexeme(new NumberToken(6), new Position(6, 1, 7, 1)), walker.Pop());
        Assert.Equal(new Lexeme(new EpsilonToken(), new Position(7, 1, 8, 0)), walker.Pop());
        Assert.Equal(new Lexeme(new EpsilonToken(), new Position(7, 1, 8, 0)), walker.Pop());
    }

    [Fact]
    public void ASimpleRuleWithContextCanOverrideAnotherRule()
    {
        var rules = LexerRuleBook.Builder
            .AddSimpleRule<OpenParenthesisToken>("{")
            .AddSimpleRule<ClosedParenthesisToken>("}")
            .AddSimpleRule<EqualToken>("=")
            .AddSimpleRule<NewLineToken>("\r\n")
            .AddSimpleRule<NewLineToken>("\n")
            .AddRule(IsWhiteSpaceExceptNewline, ScanWhiteSpace)
            .AddRule(char.IsLetter, ScanIdentifier)
            .AddSimpleRuleWithContext<ClassToken>("class", context => context.All(lexeme => lexeme is not { Token: ClassToken }), 1)
            .WithEpsilonToken<EpsilonToken>()
            .Build();

        var result = rules.Scan(ClassExpression);

        Assert.Collection(
            result.Lexemes,
            lexeme => Assert.Equal(new Lexeme(new ClassToken(), new Position(0, 1, 1, 5)), lexeme),
            NoOperation,
            NoOperation,
            NoOperation,
            lexeme => Assert.Equal(new Lexeme(new OpenParenthesisToken(), new Position(20, 2, 1, 1)), lexeme),
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            lexeme => Assert.Equal(new Lexeme(new IdentifierToken("class"), new Position(67, 5, 12, 5)), lexeme),
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            NoOperation,
            lexeme => Assert.Equal(new Lexeme(new ClosedParenthesisToken(), new Position(92, 7, 1, 1)), lexeme));
    }

    private static Lexeme ScanNumber(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsDigit)
            ? ScanNumber(builder.Retain())
            : builder.Build(new NumberToken(int.Parse(builder.CurrentToken)));

    private static Lexeme ScanIdentifier(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsLetterOrDigit)
            ? ScanIdentifier(builder.Retain())
            : builder.Build(new IdentifierToken(builder.CurrentToken));

    private static Lexeme ScanWhiteSpace(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsWhiteSpace)
            ? ScanWhiteSpace(builder.Discard())
            : builder.Build(new WhiteSpaceToken());

    private static bool IsWhiteSpaceExceptNewline(char c)
        => char.IsWhiteSpace(c) && c is not '\r' and not '\n';
}
