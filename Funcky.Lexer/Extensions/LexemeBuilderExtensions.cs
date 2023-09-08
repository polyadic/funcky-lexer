using Funcky.Extensions;
using static Funcky.Functional;

namespace Funcky.Lexer.Extensions;

internal static class LexemeBuilderExtensions
{
    public static bool IsSymbolMatching(this ILexemeBuilder builder, string textSymbol)
        => textSymbol
            .WithIndex()
            .All(t => builder.Peek(t.Index).Match(none: false, some: c => c == t.Value));

    // we do not want to extract key words in the middle of a word, so a symbol must have ended.
    // Which means after a textsymbol must come something other than a digit or a letter.
    public static bool HasWordBoundary(this ILexemeBuilder builder, string textSymbol)
        => builder.Peek(textSymbol.Length).Match(none: true, some: Not<char>(char.IsLetterOrDigit));

    public static bool ConsumeSymbol(this ILexemeBuilder builder, string textSymbol)
        => textSymbol.ForEach(_ => builder.Discard()) == Unit.Value;
}
