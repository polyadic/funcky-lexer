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
    private readonly ILexerReader.Factory _newLexerReader;
    private readonly ILinePositionCalculator.Factory _newLinePositionCalculator;
    private readonly ILexemeBuilder.Factory _newLexemeBuilder;
    private readonly ILexemeWalker.Factory _newLexemeWalker;
    private readonly IEpsilonToken.Factory _newEpsilonToken;
    private readonly Option<Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>>> _postProcess;

    private readonly ImmutableList<ILexerRule> _rules;

    internal LexerRuleBook(
        ILexerReader.Factory newLexerReader,
        ILinePositionCalculator.Factory newLinePositionCalculator,
        ILexemeBuilder.Factory newLexemeBuilder,
        ILexemeWalker.Factory newLexemeWalker,
        IEpsilonToken.Factory newEpsilonToken,
        Option<Func<IEnumerable<Lexeme>, IEnumerable<Lexeme>>> postProcess,
        ImmutableList<ILexerRule> rules)
    {
        _newLexerReader = newLexerReader;
        _newLinePositionCalculator = newLinePositionCalculator;
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
    /// <returns>The lexer result contains the sequence of the lexemes and and a configured ILexemeWalker.</returns>
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
            .TakeWhile(_ => reader.Peek().Match(none: false, some: True))
            .Aggregate(ImmutableList<Lexeme>.Empty, (lexemes, _) => lexemes.Add(FindNextLexeme(reader, lexemes)));

    private Lexeme FindNextLexeme(ILexerReader reader, ImmutableList<Lexeme> context)
        => SelectLexerRule(reader, context)
            .GetOrElse(() => HandleUnknownToken(reader, context));

    private Lexeme HandleUnknownToken(ILexerReader reader, ImmutableList<Lexeme> context)
        => throw new UnknownTokenException(reader.Peek(), CalculateCurrentLinePosition(reader.Position, context));

    private Position CalculateCurrentLinePosition(int position, ImmutableList<Lexeme> context)
        => _newLinePositionCalculator(context)
            .CalculateLinePosition(position, 1);

    private Option<Lexeme> SelectLexerRule(ILexerReader reader, ImmutableList<Lexeme> context)
        => _rules
            .Where(rule => rule.IsActive(context))
            .OrderByDescending(GetRuleWeight)
            .WhereSelect(ToMatchingRule(reader, context))
            .FirstOrNone();

    private Func<ILexerRule, Option<Lexeme>> ToMatchingRule(ILexerReader reader, ImmutableList<Lexeme> context)
        => rule
            => rule.Match(_newLexemeBuilder(reader, _newLinePositionCalculator(context)));

    private static int GetRuleWeight(ILexerRule rule)
        => rule.Weight;
}
