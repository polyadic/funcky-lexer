using Funcky.Lexer.Default;
using Funcky.Lexer.Token;
using Funcky.Monads;

namespace Funcky.Lexer;

public interface ILexemeBuilder
{
    delegate ILexemeBuilder Factory(ILexerReader lexerReader, ILinePositionCalculator linePositionCalculator);

    static Factory DefaultFactory = (reader, linePositionCalculator) => new LexemeBuilder(reader, linePositionCalculator);

    string CurrentToken { get; }

    int Position { get; }

    Option<char> Peek(int lookAhead = 0);

    Lexeme Build(IToken token);

    ILexemeBuilder Retain();

    ILexemeBuilder Discard();
}