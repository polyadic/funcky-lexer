using Funcky.Lexer.Token;

namespace Funcky.Lexer.Test.Tokens;

public record NumberToken(double Number) : IToken;