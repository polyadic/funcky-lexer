using Funcky.Extensions;
using Funcky.Lexer.Test.Tokens;

namespace Funcky.Lexer.Test.LexerRules;

public static class ArithmeticLexerRules
{
    public static ILexerRuleBookBuilder GetRules()
        => LexerRuleBook.Builder
            .WithEpsilonToken<EpsilonToken>()
            .AddRule(char.IsWhiteSpace, ScanWhiteSpace)
            .AddRule(c => char.IsDigit(c) || c == '.', ScanNumber)
            .AddRule(char.IsLetter, ScanIdentifier)
            .AddSimpleRule<MinusToken>("-")
            .AddSimpleRule<PlusToken>("+")
            .AddSimpleRule<MultiplicationToken>("*")
            .AddSimpleRule<DivideToken>("/")
            .AddSimpleRule<ModuloToken>("%")
            .AddSimpleRule<PowerToken>("^")
            .AddSimpleRule<OpenParenthesisToken>("(")
            .AddSimpleRule<ClosedParenthesisToken>(")")
            .AddSimpleRule<CommaToken>(",");

    private static Lexeme ScanWhiteSpace(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsWhiteSpace)
            ? ScanWhiteSpace(builder.Discard())
            : builder.Build(new WhiteSpaceToken());

    private static Lexeme ScanNumber(ILexemeBuilder builder)
        => RecursiveScanNumber(builder, false);

    private static Lexeme RecursiveScanNumber(ILexemeBuilder builder, bool decimalAlreadySeen)
        => IsDoubleDigitCharacter(builder)
            ? IsDot(builder) is var isDot && isDot && decimalAlreadySeen
                ? throw new Exception("Multiple dots in decimal number")
                : RecursiveScanNumber(builder.Retain(), decimalAlreadySeen || isDot)
            : builder.Build(new NumberToken(ParsedDouble(builder)));

    private static bool IsDot(ILexemeBuilder builder)
        => builder.Peek()
            .Select(character => character is '.')
            .GetOrElse(false);

    private static bool IsDoubleDigitCharacter(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsDigit)
           || builder.Peek().Match(none: false, some: c => c == '.');

    private static double ParsedDouble(ILexemeBuilder builder)
        => builder.CurrentToken
            .ParseDoubleOrNone()
            .GetOrElse(() => throw new Exception("Could not parse number: " + builder.CurrentToken));

    private static Lexeme ScanIdentifier(ILexemeBuilder builder)
        => builder.Peek().Match(none: false, some: char.IsLetterOrDigit)
            ? ScanIdentifier(builder.Retain())
            : builder.Build(new IdentifierToken(builder.CurrentToken));
}
