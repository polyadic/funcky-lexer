using System.Collections.Immutable;
using Funcky.Extensions;
using Funcky.Lexer.Exceptions;
using Funcky.Lexer.Rules;
using Funcky.Lexer.Token;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Lexer;

/// <summary>
/// A lexer rule book contains all the rules and factories to lex your expresssions. It has only one Method Scan() which will return the result of the lexer.
/// </summary>
public sealed class LexerRuleBook
{
    private const int LengthOfCharacter = 1;
    private readonly ILexerReader.Factory _newLexerReader;
    private readonly ILexemeBuilder.Factory _newLexemeBuilder;
    private readonly ILexemeWalker.Factory _newLexemeWalker;
    private readonly IEpsilonToken.Factory _newEpsilonToken;
    private readonly Option<Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>>> _postProcess;

    private readonly ImmutableList<ILexerRule> _rules;

    internal LexerRuleBook(
        ILexerReader.Factory newLexerReader,
        ILexemeBuilder.Factory newLexemeBuilder,
        ILexemeWalker.Factory newLexemeWalker,
        IEpsilonToken.Factory newEpsilonToken,
        Option<Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>>> postProcess,
        ImmutableList<ILexerRule> rules)
    {
        _newLexerReader = newLexerReader;
        _newLexemeBuilder = newLexemeBuilder;
        _newLexemeWalker = newLexemeWalker;
        _newEpsilonToken = newEpsilonToken;
        _postProcess = postProcess;
        _rules = rules;
    }

    public static LexerRuleBookBuilder Builder { get; } = new();

    /// <summary>
    /// The Scan method takes a string and creates the lexemes.
    /// </summary>
    /// <param name="expression">The string which we want to lex into separate token.</param>
    /// <returns>The lexer result contains the sequence of the lexemes and a configured ILexemeWalker.</returns>
    public LexerResult Scan(string expression)
    {
        var reader = _newLexerReader(expression);
        var lexemes = _postProcess.Match(
            none: () => CreateLexemes(reader),
            some: processLexemes => processLexemes(CreateLexemes(reader)).ToImmutableList());

        return new LexerResult(lexemes, _newLexemeWalker(lexemes, _newEpsilonToken));
    }

    private ImmutableList<Lexeme> CreateLexemes(ILexerReader reader)
        => Sequence.Cycle(Unit.Value)
            .TakeWhile(_ => HasTokensLeft(reader))
            .Aggregate(LexemeAggregate.Empty, AggregateLexeme(reader))
            .Lexemes;

    private Func<LexemeAggregate, Unit, LexemeAggregate> AggregateLexeme(ILexerReader reader)
        => (result, _)
            => NextAggregate(FindNextLexeme(reader, result.Lexemes, result.CurrentLine), result);

    private static LexemeAggregate NextAggregate(Lexeme lexeme, LexemeAggregate result)
        => new(result.Lexemes.Add(lexeme), UpdateLineOnLineBreak(result, lexeme));

    private static LineAnchor UpdateLineOnLineBreak(LexemeAggregate result, Lexeme lexeme)
        => lexeme.Token is ILineBreakToken
            ? new LineAnchor(result.CurrentLine.Line + 1, lexeme.Position.EndPosition)
            : result.CurrentLine;

    private static bool HasTokensLeft(ILexerReader reader)
        => reader.Peek().Match(none: false, some: True);

    private Lexeme FindNextLexeme(ILexerReader reader, ImmutableList<Lexeme> context, LineAnchor currentLine)
        => SelectLexerRule(reader, context, currentLine)
            .GetOrElse(() => HandleUnknownToken(reader, currentLine));

    private static Lexeme HandleUnknownToken(ILexerReader reader, LineAnchor currentLine)
        => throw new UnknownTokenException(reader.Peek(), new Position(reader.Position, LengthOfCharacter, currentLine));

    private Option<Lexeme> SelectLexerRule(ILexerReader reader, ImmutableList<Lexeme> context, LineAnchor currentLine)
        => _rules
            .Where(rule => rule.IsActive(context))
            .OrderByDescending(GetRuleWeight)
            .WhereSelect(ToMatchingRule(reader, currentLine))
            .FirstOrNone();

    private Func<ILexerRule, Option<Lexeme>> ToMatchingRule(ILexerReader reader, LineAnchor currentLine)
        => rule
            => rule.Match(_newLexemeBuilder(reader, currentLine));

    private static int GetRuleWeight(ILexerRule rule)
        => rule.Weight;

    private record LexemeAggregate(ImmutableList<Lexeme> Lexemes, LineAnchor CurrentLine)
    {
        public static LexemeAggregate Empty { get; } = new(ImmutableList<Lexeme>.Empty, LineAnchor.DocumentStart);
    }
}