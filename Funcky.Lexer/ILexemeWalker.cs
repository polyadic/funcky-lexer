using Funcky.Lexer.DefaultImplementation;
using Funcky.Lexer.Token;

namespace Funcky.Lexer;

public interface ILexemeWalker
{
    public static Factory DefaultFactory = (lexemes, newEpsilonToken) => new LexemeWalker(lexemes, newEpsilonToken);

    public delegate ILexemeWalker Factory(IReadOnlyList<Lexeme> lexemes, IEpsilonToken.Factory newEpsilonToken);

    /// <summary>
    /// Returns the current lexeme and advances the position by one.
    /// </summary>
    Lexeme Pop();

    /// <summary>
    /// Returns the current (or lookahead number of lexemes in front of us) lexeme.
    /// </summary>
    /// <param name="lookAhead">Number of lexemes we want to skip.</param>
    Lexeme Peek(int lookAhead = 0);
}