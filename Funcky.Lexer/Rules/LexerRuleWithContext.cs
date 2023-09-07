using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Lexer.Rules;

internal sealed class LexerRuleWithContext : ILexerRule
{
    public LexerRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createLexeme, int weight)
        => (SymbolPredicate, ContextPredicate, CreateLexeme, Weight)
            = (symbolPredicate, contextPredicate, createLexeme, weight);

    public Predicate<char> SymbolPredicate { get; }

    public Predicate<IReadOnlyList<Lexeme>> ContextPredicate { get; }

    public Lexeme.Factory CreateLexeme { get; }

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).Match(none: false, some: Identity)
            ? CreateLexeme(builder)
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => ContextPredicate(context);

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
           select SymbolPredicate(nextCharacter);
}