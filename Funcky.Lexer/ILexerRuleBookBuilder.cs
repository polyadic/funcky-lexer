using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public interface ILexerRuleBookBuilder
{
    ILexerRuleBookBuilder AddRule(Predicate<char> predicate, Lexeme.Factory createToken, int weight = 0);

    ILexerRuleBookBuilder AddSimpleRule<TToken>(string textSymbol)
        where TToken : IToken, new();

    ILexerRuleBookBuilder AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createToken, int weight);

    ILexerRuleBookBuilder WithLexerReader(ILexerReader.Factory newLexerReader);

    ILexerRuleBookBuilder WithLinePositionCalculator(ILinePositionCalculator.Factory newLinePositionCalculator);

    ILexerRuleBookBuilder WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker);

    ILexerRuleBookBuilder WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder);

    ILexerRuleBookBuilder WithEpsilonToken<TEpsilonToken>()
        where TEpsilonToken : IEpsilonToken, new();

    LexerRuleBook Build();
}