using Funcky.Lexer.Extensions;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.Rules;

internal sealed class SimpleLexerRuleWithContext<TToken>(
    string textSymbol,
    Predicate<IReadOnlyList<Lexeme>> contextPredicate,
    int weight)
    : ILexerRule
    where TToken : IToken, new()
{
    private readonly bool _isOperator = !textSymbol.All(char.IsLetter);

    public int Weight { get; } = weight;

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => builder.IsSymbolMatching(textSymbol) && (_isOperator || builder.HasWordBoundary(textSymbol)) && builder.ConsumeSymbol(textSymbol)
           ? builder.Build(new TToken())
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => contextPredicate(context);
}