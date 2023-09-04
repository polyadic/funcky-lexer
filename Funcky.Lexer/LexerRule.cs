using System.Collections.Immutable;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Lexer;

internal sealed class LexerRule : ILexerRule
{
    public LexerRule(Predicate<char> predicate, Func<ILexemeBuilder, Lexeme> createToken, int weight)
        => (Predicate, CreateToken, Weight)
            = (predicate, createToken, weight);

    public Predicate<char> Predicate { get; }

    public Func<ILexemeBuilder, Lexeme> CreateToken { get; }

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).Match(none: false, some: Identity)
            ? CreateToken(builder)
            : Option<Lexeme>.None;

    public bool IsActive(ImmutableList<Lexeme> context)
        => true;

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
            select Predicate(nextCharacter);
}