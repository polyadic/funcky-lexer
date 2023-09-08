using Funcky.Lexer.Extensions;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer.Rules;

internal sealed class SimpleLexerRule<TToken> : ILexerRule
    where TToken : IToken, new()
{
    private readonly string _textSymbol;
    private readonly bool _isOperator;

    public SimpleLexerRule(string textSymbol)
        => (_textSymbol, _isOperator)
            = (textSymbol, !textSymbol.All(char.IsLetter));

    public int Weight
        => _textSymbol.Length;

    public Option<Lexeme> Match(ILexemeBuilder builder)
        => builder.IsSymbolMatching(_textSymbol) && (_isOperator || builder.HasWordBoundary(_textSymbol)) && builder.ConsumeSymbol(_textSymbol)
            ? builder.Build(new TToken())
            : Option<Lexeme>.None;

    public bool IsActive(IReadOnlyList<Lexeme> context)
        => true;
}