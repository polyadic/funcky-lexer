using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Lexer.Rules;

internal sealed class LexerRule : ILexerRule
{
    public LexerRule(Predicate<char> predicate, Lexeme.Factory createLexeme, int weight)
        => (Predicate, CreateLexeme, Weight)
            = (predicate, createLexeme, weight);

    public Predicate<char> Predicate { get; }

    public Lexeme.Factory CreateLexeme { get; }

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).Match(none: false, some: Identity)
            ? CreateLexeme(builder)
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => true;

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
           select Predicate(nextCharacter);
}