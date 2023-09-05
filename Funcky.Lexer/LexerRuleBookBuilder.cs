using System.Collections.Immutable;
using Funcky.Lexer.Exceptions;
using Funcky.Lexer.Rules;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer;

public class LexerRuleBookBuilder
{
    private ImmutableList<ILexerRule> _rules = ImmutableList<ILexerRule>.Empty;
    private ILexerReader.Factory _newLexerReader = ILexerReader.DefaultFactory;
    private ILinePositionCalculator.Factory _newLinePositionCalculator = ILinePositionCalculator.DefaultFactory;
    private ILexemeBuilder.Factory _newLexemeBuilder = ILexemeBuilder.DefaultFactory;
    private ILexemeWalker.Factory _newLexemeWalker = ILexemeWalker.DefaultFactory;
    private Option<IEpsilonToken.Factory> _newEpsilonToken;

    public LexerRuleBookBuilder AddRule(Predicate<char> predicate, Lexeme.Factory createToken, int weight = 0)
    {
        _rules = _rules.Add(new LexerRule(predicate, createToken, weight));

        return this;
    }

    public LexerRuleBookBuilder AddSimpleRule<TToken>(string textSymbol)
        where TToken : IToken, new()
    {
        _rules = _rules.Add(new SimpleLexerRule<TToken>(textSymbol));

        return this;
    }

    public LexerRuleBookBuilder AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createToken, int weight)
    {
        _rules = _rules.Add(new LexerRuleWithContext(symbolPredicate, contextPredicate, createToken, weight));

        return this;
    }

    public LexerRuleBookBuilder WithLexerReader(ILexerReader.Factory newLexerReader)
    {
        _newLexerReader = newLexerReader;

        return this;
    }

    public LexerRuleBookBuilder WithLinePositionCalculator(ILinePositionCalculator.Factory newLinePositionCalculator)
    {
        _newLinePositionCalculator = newLinePositionCalculator;

        return this;
    }

    public LexerRuleBookBuilder WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker)
    {
        _newLexemeWalker = newLexemeWalker;

        return this;
    }

    public LexerRuleBookBuilder WithEpsilonToken<TEpsilonToken>()
        where TEpsilonToken : IEpsilonToken, new()
    {
        _newEpsilonToken = (IEpsilonToken.Factory)(() => new TEpsilonToken());

        return this;
    }

    public LexerRuleBook Build()
        => new(
            newLexerReader: _newLexerReader,
            newLinePositionCalculator: _newLinePositionCalculator,
            newLexemeBuilder: _newLexemeBuilder,
            newLexemeWalker: _newLexemeWalker,
            newEpsilonToken: _newEpsilonToken.GetOrElse(NoEpsilonTokenDefined),
            rules: _rules);

    public LexerRuleBookBuilder WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder)
    {
        _newLexemeBuilder = newLexemeBuilder;

        return this;
    }

    private static IEpsilonToken.Factory NoEpsilonTokenDefined()
        => throw new LexerException("No epsilon token defined");
}