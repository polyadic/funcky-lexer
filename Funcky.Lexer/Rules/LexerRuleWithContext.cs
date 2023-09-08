using Funcky.Monads;

namespace Funcky.Lexer.Rules;

internal sealed class LexerRuleWithContext : ILexerRule
{
    private readonly Predicate<IReadOnlyList<Lexeme>> _contextPredicate;
    private readonly Predicate<char> _symbolPredicate;
    private readonly Lexeme.Factory _createLexeme;

    public LexerRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createLexeme, int weight)
        => (_symbolPredicate, _contextPredicate, _createLexeme, Weight)
            = (symbolPredicate, contextPredicate, createLexeme, weight);

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).GetOrElse(false)
            ? _createLexeme(builder)
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => _contextPredicate(context);

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
           select _symbolPredicate(nextCharacter);
}