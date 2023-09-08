using Funcky.Lexer.Extensions;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.Rules;

internal sealed class SimpleLexerRuleWithContext<TToken> : ILexerRule
    where TToken : IToken, new()
{
    private readonly Predicate<IReadOnlyList<Lexeme>> _contextPredicate;
    private readonly string _textSymbol;
    private readonly bool _isOperator;

    public SimpleLexerRuleWithContext(string textSymbol, Predicate<IReadOnlyList<Lexeme>> contextPredicate, int weight)
    {
        Weight = weight;
        _contextPredicate = contextPredicate;
        _textSymbol = textSymbol;
        _isOperator = !textSymbol.All(char.IsLetter);
    }

    public int Weight { get; }

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => builder.IsSymbolMatching(_textSymbol) && (_isOperator || builder.HasWordBoundary(_textSymbol)) && builder.ConsumeSymbol(_textSymbol)
           ? builder.Build(new TToken())
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => _contextPredicate(context);
}