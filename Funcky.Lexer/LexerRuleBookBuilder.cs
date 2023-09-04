using System.Collections.Immutable;

namespace Funcky.Lexer;

public class LexerRuleBookBuilder
{
    private ImmutableList<ILexerRule> _rules = ImmutableList<ILexerRule>.Empty;
    private ILexerReader.Factory _newLexerReader;
    private ILinePositionCalculator.Factory _newLinePositionCalculator;
    private ILexemeBuilder.Factory _newLexemeBuilder;

    public LexerRuleBookBuilder()
    {
        _newLexemeBuilder = (reader, linePositionCalculator) => new LexemeBuilder(reader, linePositionCalculator);
        _newLexerReader = expression => new LexerReader(expression);
        _newLinePositionCalculator = lexemes => new LinePositionCalculator(lexemes);
    }

    public LexerRuleBookBuilder AddRule(Predicate<char> predicate, Func<ILexemeBuilder, Lexeme> createToken,
        int weight = 0)
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



    public LexerRuleBookBuilder AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Func<ILexemeBuilder, Lexeme> createToken, int weight)
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


    public LexerRuleBook Build()
        => new(
            _newLexerReader,
            _newLinePositionCalculator,
            _newLexemeBuilder,
            _rules);

    public LexerRuleBookBuilder WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder)
    {
        _newLexemeBuilder = newLexemeBuilder;

        return this;
    }
}