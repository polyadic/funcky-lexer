using Funcky.Lexer.Default;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer;

public interface ILexemeBuilder
{
    public static Factory DefaultFactory = (reader, linePositionCalculator) => new LexemeBuilder(reader, linePositionCalculator);

    public delegate ILexemeBuilder Factory(ILexerReader lexerReader, ILinePositionCalculator linePositionCalculator);

    /// <summary>
    /// Returns all characters retained as a string.
    /// </summary>
    string CurrentToken { get; }

    /// <summary>
    /// Current zero based absolute position in number of characters.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Returns the current (or lookahead number of characters in front of us) character in the string.
    /// </summary>
    /// <param name="lookAhead">Number of characters we want to skip.</param>
    Option<char> Peek(int lookAhead = 0);

    /// <summary>
    /// Creates a Lexeme with the position information, you return the token usually based on the string in CurrentToken.
    /// </summary>
    Lexeme Build(IToken token);

    /// <summary>
    /// Adds the current characters to the CurrentToken.
    /// </summary>
    ILexemeBuilder Retain();

    /// <summary>
    /// We read the current character but ignore it and do not add it to the CurrentToken.
    /// </summary>
    ILexemeBuilder Discard();
}