using Funcky.Monads;

namespace Funcky.Lexer.Exceptions;

public sealed class UnknownTokenException(Option<char> token, Position position)
    : LexerException($"Unknown Token '{ToName(token)}' at Line {position.Line} Column {position.StartColumn}")
{
    public Option<char> Token { get; } = token;

    public Position Position { get; } = position;

    private static char ToName(Option<char> token)
        => token.GetOrElse('Ɛ');
}