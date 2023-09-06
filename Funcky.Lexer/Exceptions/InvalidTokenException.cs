namespace Funcky.Lexer.Exceptions;

public sealed class InvalidTokenException : LexerException
{
    public InvalidTokenException(string message)
        : base(message)
    {
    }
}