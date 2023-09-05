using Funcky.Monads;

namespace Funcky.Lexer.Exceptions;

public sealed class UnknownTokenException : LexerException
{
    public UnknownTokenException(Option<char> token, Position position)
        : base($"Unknown Token '{ToName(token)}' at Line {position.Line} Column {position.StartColumn}")
        => (Token, Position) = (token, position);

    public Option<char> Token { get; }

    public Position Position { get; }

    private static char ToName(Option<char> token)
        => token.Match(none: 'Ɛ', some: Functional.Identity);
}