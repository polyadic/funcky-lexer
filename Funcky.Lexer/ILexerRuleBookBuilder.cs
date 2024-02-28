using Funcky.Lexer.Rules;
using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public interface ILexerRuleBookBuilder
{
    /// <summary>
    /// Add your own generic lexer rule: you have to implement your rule by implementing the ILexerRule interface.
    /// </summary>
    ILexerRuleBookBuilder AddRule(ILexerRule rule);

    /// <summary>
    /// Add a generic rule based on a predicate and a Lexeme.Factory.
    /// </summary>
    ILexerRuleBookBuilder AddRule(Predicate<char> symbolPredicate, Lexeme.Factory createLexeme, int weight = 0);

    ILexerRuleBookBuilder AddSimpleRule<TToken>(string textSymbol)
        where TToken : IToken, new();

    ILexerRuleBookBuilder AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createLexeme, int weight);

    ILexerRuleBookBuilder AddSimpleRuleWithContext<TToken>(string textSymbol, Predicate<IReadOnlyList<Lexeme>> contextPredicate, int weight)
        where TToken : IToken, new();

    ILexerRuleBookBuilder WithLexerReader(ILexerReader.Factory newLexerReader);

    ILexerRuleBookBuilder WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker);

    ILexerRuleBookBuilder WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder);

    ILexerRuleBookBuilder WithPostProcess(Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>> postProcess);

    LexerRuleBook Build();
}