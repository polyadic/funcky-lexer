using Funcky.Monads;

namespace Funcky.Lexer;

public interface ILexemeBuilder
{
    delegate ILexemeBuilder Factory(ILexerReader lexerReader, ILinePositionCalculator linePositionCalculator);

    string CurrentToken { get; }

    int Position { get; }

    Option<char> Peek(int lookAhead = 0);

    Lexeme Build(IToken token);

    ILexemeBuilder Retain();

    ILexemeBuilder Discard();
}