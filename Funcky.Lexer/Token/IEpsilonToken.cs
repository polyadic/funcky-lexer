namespace Funcky.Lexer.Token;

public interface IEpsilonToken : IToken
{
    public delegate IEpsilonToken Factory();
}
