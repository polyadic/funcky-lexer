using System.Collections.Immutable;
using Funcky.Extensions;
using Funcky.Monads;

namespace Funcky.Lexer;

internal sealed class SimpleLexerRule<TToken> : ILexerRule
    where TToken : IToken, new()
{
    private readonly string _textSymbol;
    private readonly bool _isTextSymbol;

    public SimpleLexerRule(string textSymbol)
        => (_textSymbol, _isTextSymbol)
            = (textSymbol, textSymbol.All(char.IsLetter));

    public int Weight
        => _textSymbol.Length;

    public Option<Lexeme> Match(ILexemeBuilder builder)
    {
        if (IsSymbolMatchingReader(builder) && (IsOperator() || HasWordBoundary(builder)))
        {
            _textSymbol.ForEach(_ => builder.Discard());

            return builder.Build(new TToken());
        }
        return Option<Lexeme>.None;
    }

    public bool IsActive(ImmutableList<Lexeme> context)
        => true;

    // we do not want to extract key words in the middle of a word, so a symbol must have ended.
    // Which means after a textsymbol must come something other than a digit or a letter.
    private bool HasWordBoundary(ILexemeBuilder builder)
        => builder.Peek(_textSymbol.Length).Match(none: true, some: NonLetterOrDigit);

    private bool IsOperator()
        => !_isTextSymbol;

    private static bool NonLetterOrDigit(char character)
        => !char.IsLetterOrDigit(character);

    private bool IsSymbolMatchingReader(ILexemeBuilder builder)
        => _textSymbol.Select((character, index) => new { character, index })
            .All(t => builder.Peek(t.index).Match(none: false, some: c => c == t.character));
}