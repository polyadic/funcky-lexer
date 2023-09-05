using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.LexerRules;

internal static class WordTokenizerWithLines
{
    public static LexerRuleBook GetRules()
        => LexerRuleBook.Builder
            .WithEpsilonToken<EpsilonToken>()
            .AddRule(char.IsLetter, ScanWord)
            .AddSimpleRule<SpaceToken>(" ")
            .AddSimpleRule<NewLineToken>("\r\n")
            .AddSimpleRule<NewLineToken>("\n")
            .AddSimpleRule<NewLineToken>("\r")
            .Build();

    private static Lexeme ScanWord(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsLetter)
            ? ScanWord(builder.Retain())
            : builder.Build(new WordToken(builder.CurrentToken));
}