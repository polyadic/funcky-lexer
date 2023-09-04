using System.Collections.Immutable;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Lexer;

internal sealed class LexerRuleWithContext : ILexerRule
{
    public LexerRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Func<ILexemeBuilder, Lexeme> createToken, int weight)
        => (SymbolPredicate, ContextPredicate, CreateToken, Weight)
            = (symbolPredicate, contextPredicate, createToken, weight);

    public Predicate<char> SymbolPredicate { get; }

    public Predicate<IReadOnlyList<Lexeme>> ContextPredicate { get; }

    public Func<ILexemeBuilder, Lexeme> CreateToken { get; }

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).Match(none: false, some: Identity)
            ? CreateToken(builder)
            : Option<Lexeme>.None;

    public bool IsActive(ImmutableList<Lexeme> context)
        => ContextPredicate(context);

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
            select SymbolPredicate(nextCharacter);
}