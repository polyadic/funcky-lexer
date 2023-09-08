using Funcky.Monads;

namespace Funcky.Lexer.Rules;

internal sealed class LexerRule : ILexerRule
{
    private readonly Predicate<char> _symbolPredicate;
    private readonly Lexeme.Factory _createLexeme;

    public LexerRule(Predicate<char> symbolPredicate, Lexeme.Factory createLexeme, int weight)
        => (_symbolPredicate, _createLexeme, Weight)
            = (symbolPredicate, createLexeme, weight);

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => ApplyPredicate(builder).GetOrElse(false)
            ? _createLexeme(builder)
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => true;

    private Option<bool> ApplyPredicate(ILexemeBuilder builder)
        => from nextCharacter in builder.Peek()
           select _symbolPredicate(nextCharacter);
}