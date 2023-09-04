namespace Funcky.Lexer.Token;

public interface IEpsilonToken : IToken
{
    delegate IEpsilonToken Factory();

}
