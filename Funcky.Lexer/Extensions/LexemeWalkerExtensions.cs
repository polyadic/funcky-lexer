using Funcky.Lexer.Exceptions;
using Funcky.Lexer.Token;

namespace Funcky.Lexer.Extensions;

public static class LexemeWalkerExtensions
{
    public static Lexeme Consume<TToken>(this ILexemeWalker walker)
        where TToken : IToken
        => ConsumeLexeme<TToken>(walker.Pop());

    public static bool NextIs<TType>(this ILexemeWalker walker, int lookAhead = 0)
        => walker.Peek(lookAhead).Token is TType;

    private static Lexeme ConsumeLexeme<TToken>(Lexeme lexeme)
        where TToken : IToken
        => lexeme is { Token: TToken }
            ? lexeme
            : HandleMissingLexeme<TToken>(lexeme);

    private static Lexeme HandleMissingLexeme<TToken>(Lexeme lexeme)
        => ThrowExpectingTokenException<TToken>(lexeme, lexeme.Position);

    private static Lexeme ThrowExpectingTokenException<TToken>(Lexeme lexeme, Position position)
        => throw new InvalidTokenException($"Expecting {typeof(TToken).FullName} but got {lexeme.Token} at Line {position.Line} Column {position.StartColumn}.");
}
