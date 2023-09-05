using System.Collections.Immutable;
using Funcky.Extensions;
using Funcky.Lexer.Exceptions;
using Funcky.Lexer.Rules;
using Funcky.Lexer.Token;
using Funcky.Monads;
using static Funcky.Functional;

namespace Funcky.Lexer;

public class LexerRuleBook
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

    public LexerResult Scan(string expression)
    {
        var lexemes = _postProcess.Match(
            none: () => CreateLexemes(expression),
            some: processLexemes => processLexemes(CreateLexemes(expression)).ToImmutableList());

        return new LexerResult(lexemes, _newLexemeWalker(lexemes, _newEpsilonToken));
    }

    private ImmutableList<Lexeme> CreateLexemes(string expression)
    {
        var reader = _newLexerReader(expression);

        var lexemes = ImmutableList<Lexeme>.Empty;
        while (reader.Peek().Match(none: false, some: True))
        {
            lexemes = lexemes.Add(FindNextLexeme(reader, lexemes));
        }

        return lexemes;
    }

    private Lexeme FindNextLexeme(ILexerReader reader, ImmutableList<Lexeme> context)
        => SelectLexerRule(reader, context)
            .Match(
                none: () => HandleUnknownToken(reader, context),
                some: Identity);

    private Lexeme HandleUnknownToken(ILexerReader reader, ImmutableList<Lexeme> context)
        => throw new UnknownTokenException(reader.Peek(), CalculateCurrentLinePosition(reader.Position, context));

    private LinePosition CalculateCurrentLinePosition(int position, ImmutableList<Lexeme> context)
        => _newLinePositionCalculator(context)
            .CalculateLinePosition(position, 1);

    private Option<Lexeme> SelectLexerRule(ILexerReader reader, ImmutableList<Lexeme> context)
        => _rules
            .Where(rule => rule.IsActive(context))
            .OrderByDescending(GetRuleWeight)
            .WhereSelect(rule => rule.Match(_newLexemeBuilder(reader, _newLinePositionCalculator(context))))
            .FirstOrNone();

    private static int GetRuleWeight(ILexerRule rule)
        => rule.Weight;
}
