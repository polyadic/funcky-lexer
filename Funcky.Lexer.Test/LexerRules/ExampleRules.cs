using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.LexerRules;

internal static class ExampleRules
{
    public static LexerRuleBook GetRules()
        => LexerRuleBook.Builder
            .AddSimpleRule<EqualToken>("=")
            .AddSimpleRule<DoubleEqualToken>("==")
            .AddSimpleRule<GreaterToken>("<")
            .AddSimpleRule<GreaterEqualToken>("<=")
            .AddSimpleRule<AndToken>("and")
            .AddSimpleRule<SpaceToken>(" ")
            .AddRule(char.IsLetter, ScanIdentifier)
            .Build();

    private static Lexeme ScanIdentifier(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsLetterOrDigit)
            ? ScanIdentifier(builder.Retain())
            : builder.Build(new IdentifierToken(builder.CurrentToken));
}