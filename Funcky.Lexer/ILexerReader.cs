using Funcky.Lexer.DefaultImplementation;
using Funcky.Monads;

namespace Funcky.Lexer;

public interface ILexerReader
{
    public static Factory DefaultFactory = expression => new LexerReader(expression);

    public delegate ILexerReader Factory(string expression);

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
    /// Returns the current character in the string and advances the position by one.
    /// </summary>
    Option<char> Read();
}