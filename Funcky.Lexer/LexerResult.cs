using System.Collections.Immutable;

namespace Funcky.Lexer;

public record LexerResult(
    ImmutableList<Lexeme> Lexemes,
    ILexemeWalker Walker);
