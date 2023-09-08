using System.Collections.Immutable;

namespace Funcky.Lexer;

public sealed record LexerResult(
    ImmutableList<Lexeme> Lexemes,
    ILexemeWalker Walker);
