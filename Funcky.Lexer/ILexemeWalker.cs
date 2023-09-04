namespace Funcky.Lexer;

public interface ILexemeWalker
{
    Lexeme Pop();

    Lexeme Peek(int lookAhead = 0);

}