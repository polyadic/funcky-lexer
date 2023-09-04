using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.LexerRules;

internal static class RulesWithContext
{
    public static LexerRuleBook GetRules()
        => LexerRuleBook.Builder
            .AddSimpleRule<SpaceToken>(" ")
            .AddSimpleRule<AaToken>("aa")
            .AddSimpleRule<BbToken>("bb")
            .AddSimpleRule<CcToken>("cc")
            .AddRuleWithContext(IsC, context => context.Any(lexeme => lexeme.Token is BbToken), ScanCcAfterBb, 3)
            .Build();

    private static Lexeme ScanCcAfterBb(ILexemeBuilder builder) =>
        builder.Peek().Match(none: false, some: IsC) && builder.Discard().Peek().Match(none: false, some: IsC)
            ? builder.Discard().Build(new CcAfterBbToken())
            : throw new NotImplementedException();

    private static bool IsC(char c)
        => c is 'c';
}