namespace Funcky.Lexer.Exceptions;

public sealed class InvalidTokenException(string message) : LexerException(message);