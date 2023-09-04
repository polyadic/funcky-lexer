using System.Collections.Immutable;
using Funcky.Monads;

namespace Funcky.Lexer.Rules;

public interface ILexerRule
{
    /// <summary>
    /// If more than one rule matches, the rule with the higher weights gets selected.
    /// </summary>
    int Weight { get; }

    /// <summary>
    /// Returns the matching lexeme if the rule matches or None, if the rule does not match.
    /// </summary>
    Option<Lexeme> Match(ILexemeBuilder builder);

    /// <summary>
    /// For Lexer rules which are not context dependent this function returns always true.
    /// Otherwise the Lexer rule can determine its state with the context which is a list of all lexemes which have been produced till now.
    /// </summary>
    bool IsActive(ImmutableList<Lexeme> context);
}