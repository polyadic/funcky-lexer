using Funcky.Lexer.Test.LexerRules;

namespace Funcky.Lexer.Test;

public sealed class PositionWithLinesTest
{
    private const string ExampleTextWihtNewLines = "Hello\r\n\r\nThis is a test\nWe are on line four\nLine five\r\nthe end";

    [Fact]
    public void GiveALexerAndALineSeparatorThePositionsAreGivenInLineAndColumn()
    {
        var rules = WordTokenizerWithLines.GetRules().Build();

        var result = rules.Scan(ExampleTextWihtNewLines);
        var lexemes = result.Lexemes;

        // hello on line 1
        Assert.Equal(1, lexemes[0].Position.Line);
        Assert.Equal(1, lexemes[0].Position.StartColumn);
        Assert.Equal(5, lexemes[0].Position.Length);

        // This on line 3
        Assert.Equal(3, lexemes[3].Position.Line);
        Assert.Equal(1, lexemes[3].Position.StartColumn);
        Assert.Equal(4, lexemes[3].Position.Length);

        // is on line 3
        Assert.Equal(3, lexemes[5].Position.Line);
        Assert.Equal(6, lexemes[5].Position.StartColumn);
        Assert.Equal(2, lexemes[5].Position.Length);

        // end at the last line of the file
        Assert.Equal(6, lexemes[27].Position.Line);
        Assert.Equal(5, lexemes[27].Position.StartColumn);
        Assert.Equal(3, lexemes[27].Position.Length);
    }
}