﻿using System.Collections.Immutable;
using System.ComponentModel;
#if NET7_0_OR_GREATER
using System.Diagnostics;
#endif
using Funcky.Lexer.Rules;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer;

public record LexerRuleBookBuilder : ILexerRuleBookBuilder
{
    private ILexerReader.Factory NewLexerReader { get; init; } = ILexerReader.DefaultFactory;

    private ILinePositionCalculator.Factory NewLinePositionCalculator { get; init; } = ILinePositionCalculator.DefaultFactory;

    private ILexemeBuilder.Factory NewLexemeBuilder { get; init; } = ILexemeBuilder.DefaultFactory;

    private ILexemeWalker.Factory NewLexemeWalker { get; init; } = ILexemeWalker.DefaultFactory;

    private Option<Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>>> PostProcess { get; init; }

    private Option<IEpsilonToken.Factory> NewEpsilonToken { get; init; }

    private ImmutableList<ILexerRule> Rules { get; init; } = ImmutableList<ILexerRule>.Empty;

    public LexerRuleBookBuilder AddRule(Predicate<char> predicate, Lexeme.Factory createToken, int weight = 0)
        => this with { Rules = Rules.Add(new LexerRule(predicate, createToken, weight)) };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.AddRule(Predicate<char> predicate, Lexeme.Factory createToken, int weight)
       => AddRule(predicate, createToken, weight);

    public LexerRuleBookBuilder AddSimpleRule<TToken>(string textSymbol)
        where TToken : IToken, new()
       => this with { Rules = Rules.Add(new SimpleLexerRule<TToken>(textSymbol)) };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.AddSimpleRule<TToken>(string textSymbol)
        => AddSimpleRule<TToken>(textSymbol);

    public LexerRuleBookBuilder AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createToken, int weight)
        => this with { Rules = Rules.Add(new LexerRuleWithContext(symbolPredicate, contextPredicate, createToken, weight)) };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createToken, int weight)
        => AddRuleWithContext(symbolPredicate, contextPredicate, createToken, weight);

    public LexerRuleBookBuilder WithLexerReader(ILexerReader.Factory newLexerReader)
        => this with { NewLexerReader = newLexerReader };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.WithLexerReader(ILexerReader.Factory newLexerReader)
        => WithLexerReader(newLexerReader);

    public LexerRuleBookBuilder WithLinePositionCalculator(ILinePositionCalculator.Factory newLinePositionCalculator)
        => this with { NewLinePositionCalculator = newLinePositionCalculator };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.WithLinePositionCalculator(ILinePositionCalculator.Factory newLinePositionCalculator)
        => WithLinePositionCalculator(newLinePositionCalculator);

    public LexerRuleBookBuilder WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker)
       => this with { NewLexemeWalker = newLexemeWalker };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker)
        => WithLexemeWalker(newLexemeWalker);

    public LexerRuleBookBuilder WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder)
        => this with { NewLexemeBuilder = newLexemeBuilder };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder)
        => this with { NewLexemeBuilder = newLexemeBuilder };

    public LexerRuleBookBuilder WithPostProcess(Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>> postProcess)
        => this with { PostProcess = postProcess };

    ILexerRuleBookBuilder ILexerRuleBookBuilder.WithPostProcess(Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>> postProcess)
        => this with { PostProcess = postProcess };

    public ILexerRuleBookBuilder WithEpsilonToken<TEpsilonToken>()
        where TEpsilonToken : IEpsilonToken, new()
        => this with { NewEpsilonToken = (IEpsilonToken.Factory)(() => new TEpsilonToken()) };

    LexerRuleBook ILexerRuleBookBuilder.Build()
        => new(
            newLexerReader: NewLexerReader,
            newLinePositionCalculator: NewLinePositionCalculator,
            newLexemeBuilder: NewLexemeBuilder,
            newLexemeWalker: NewLexemeWalker,
            newEpsilonToken: NewEpsilonToken.GetOrElse(ThrowUnreachable<IEpsilonToken.Factory>),
            postProcess: PostProcess,
            rules: Rules);

    [Obsolete(message: "You forgot to define an Epsilon token with WithEpsilon<YourEpsilonToken> on the Builder.", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public LexerRuleBook Build()
        => ThrowUnreachable<LexerRuleBook>();

    private static TReturn ThrowUnreachable<TReturn>()
#if NET7_0_OR_GREATER
        => throw new UnreachableException("protected by type system");
#else
        => throw new Exception("unreachable: protected by type system");
#endif

}
