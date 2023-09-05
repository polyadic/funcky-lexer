using System.Collections.Immutable;
using Funcky.Extensions;
using Funcky.Lexer.Token;
using Funcky.Monads;
using static Funcky.Functional;

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
        => IsSymbolMatchingReader(builder) && (_isOperator || HasWordBoundary(builder)) && ConsumeSymbol(builder)
            ? builder.Build(new TToken())
            : Option<Lexeme>.None;

    public bool IsActive(ImmutableList<Lexeme> context)
        => true;

    // we do not want to extract key words in the middle of a word, so a symbol must have ended.
    // Which means after a textsymbol must come something other than a digit or a letter.
    private bool HasWordBoundary(ILexemeBuilder builder)
        => builder.Peek(_textSymbol.Length).Match(none: true, some: Not<char>(char.IsLetterOrDigit));

    private bool IsSymbolMatchingReader(ILexemeBuilder builder)
        => _textSymbol
            .WithIndex()
            .All(t => builder.Peek(t.Index).Match(none: false, some: c => c == t.Value));

    private bool ConsumeSymbol(ILexemeBuilder builder)
        => _textSymbol.ForEach(_ => builder.Discard()) == Unit.Value;
}