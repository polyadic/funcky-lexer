using Funcky.Monads;

namespace Funcky.Lexer.Rules;

internal sealed class LexerRuleWithContext(
    Predicate<char> symbolPredicate,
    Predicate<IReadOnlyList<Lexeme>> contextPredicate,
    Lexeme.Factory createLexeme,
    int weight)
    : ILexerRule
{
    public int Weight { get; } = weight;

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).GetOrElse(false)
            ? createLexeme(builder)
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => contextPredicate(context);

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
           select symbolPredicate(nextCharacter);
}