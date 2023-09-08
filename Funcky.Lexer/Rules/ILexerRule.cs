using Funcky.Monads;

namespace Funcky.Lexer.Rules;

public interface ILexerRule
{
    /// <summary>
    /// If more than one rule matches, the rule with the higher weight gets selected.
    /// </summary>
    int Weight { get; }

    /// <summary>
    /// For Lexer rules which are not context dependent this function returns always true.
    /// Otherwise the Lexer rule can determine its state with the context which is a list of all lexemes which have been produced till now.
    /// </summary>
    bool IsActive(IReadOnlyList<Lexeme> context);

    /// <summary>
    /// Returns the matching lexeme if the rule matches or None, if the rule does not match.
    ///
    /// Attention: If you return None you should only have used Peek to inspect and not changed the underlying stream by advancing it with Retain or Discard.
    /// </summary>
    Option<Lexeme> Match(ILexemeBuilder builder);
}