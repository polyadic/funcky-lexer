using System.Collections.Immutable;
using Funcky.Lexer.Rules;
using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public record LexerRuleBookBuilder
{
    protected ILexerReader.Factory NewLexerReader { get; init; } = ILexerReader.DefaultFactory;

    protected ILinePositionCalculator.Factory NewLinePositionCalculator { get; init; } = ILinePositionCalculator.DefaultFactory;

    protected ILexemeBuilder.Factory NewLexemeBuilder { get; init; } = ILexemeBuilder.DefaultFactory;

    protected ILexemeWalker.Factory NewLexemeWalker { get; init; } = ILexemeWalker.DefaultFactory;

    protected ImmutableList<ILexerRule> Rules { get; init; } = ImmutableList<ILexerRule>.Empty;

    public LexerRuleBookBuilder AddRule(Predicate<char> predicate, Lexeme.Factory createToken, int weight = 0)
        => this with { Rules = Rules.Add(new LexerRule(predicate, createToken, weight)) };

    public LexerRuleBookBuilder AddSimpleRule<TToken>(string textSymbol)
        where TToken : IToken, new()
        => this with { Rules = Rules.Add(new SimpleLexerRule<TToken>(textSymbol)) };

    public LexerRuleBookBuilder AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createToken, int weight)
        => this with { Rules = Rules.Add(new LexerRuleWithContext(symbolPredicate, contextPredicate, createToken, weight)) };

    public LexerRuleBookBuilder WithLexerReader(ILexerReader.Factory newLexerReader)
        => this with { NewLexerReader = newLexerReader };

    public LexerRuleBookBuilder WithLinePositionCalculator(ILinePositionCalculator.Factory newLinePositionCalculator)
        => this with { NewLinePositionCalculator = newLinePositionCalculator };

    public LexerRuleBookBuilder WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker)
        => this with { NewLexemeWalker = newLexemeWalker };

    public LexerRuleBookBuilder WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder)
        => this with { NewLexemeBuilder = newLexemeBuilder };

    public LexerRuleBookBuilderWithEpsilon WithEpsilonToken<TEpsilonToken>()
        where TEpsilonToken : IEpsilonToken, new()
    {
        return new LexerRuleBookBuilderWithEpsilon(this, () => new TEpsilonToken());
    }

    public record LexerRuleBookBuilderWithEpsilon : LexerRuleBookBuilder
    {
        internal LexerRuleBookBuilderWithEpsilon(LexerRuleBookBuilder lexerRuleBookBuilder, IEpsilonToken.Factory newEpsilonToken)
        {
            NewLexerReader = lexerRuleBookBuilder.NewLexerReader;
            NewLinePositionCalculator = lexerRuleBookBuilder.NewLinePositionCalculator;
            NewLexemeBuilder = lexerRuleBookBuilder.NewLexemeBuilder;
            NewLexemeWalker = lexerRuleBookBuilder.NewLexemeWalker;
            Rules = lexerRuleBookBuilder.Rules;
            NewEpsilonToken = newEpsilonToken;
        }

        protected IEpsilonToken.Factory NewEpsilonToken { get; init; }

        public LexerRuleBook Build()
            => new(
                newLexerReader: NewLexerReader,
                newLinePositionCalculator: NewLinePositionCalculator,
                newLexemeBuilder: NewLexemeBuilder,
                newLexemeWalker: NewLexemeWalker,
                newEpsilonToken: NewEpsilonToken,
                rules: Rules);
    }
}
