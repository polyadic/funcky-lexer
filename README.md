﻿# Funcky.Lexer

Can you write a lexer in a single expression? Now you can.

[![Build](https://github.com/polyadic/funcky-lexer/workflows/Build/badge.svg)](https://github.com//polyadic/funcky-lexer/actions?query=workflow%3ABuild)
[![Licence: MIT](https://img.shields.io/badge/licence-MIT-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-MIT)
[![Licence: Apache](https://img.shields.io/badge/licence-Apache-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-Apache)

## Package

[![NuGet package](https://buildstats.info/nuget/Funcky.Lexer)](https://www.nuget.org/packages/Funcky.Lexer)

## Example

Here is simple working example how you would write a simple lexer for a arithmetic parser.

Define your tokens by marking them with the `IToken` interface.

```cs
public record PlusToken : IToken;
public record MinusToken : IToken;
public record MultiplicationToken : IToken;
public record DivisionToken : IToken;
```

You have to define an EpsilonToken marked with the `IEpsilonToken` interface. This will be returned by the LexerReader when you read past the end. 
This also means you should not construct this Token on your own.

```cs
public record EpsilonToken : IEpsilonToken;
```

Create the rules how you want to lex the tokens. 

```cs
LexerResult result = LexerRuleBook.Builder
    .AddRule(char.IsDigit, ScanNumber)
    .AddSimpleRule<PlusToken>("+")
    .AddSimpleRule<MinusToken>("-")
    .AddSimpleRule<MultiplicationToken>("*")
    .AddSimpleRule<DivisionToken>("/")
    .WithEpsilonToken<EpsilonToken>()
    .Build()
    .Scan("40+20*6");
```

Use the lexeme-builder to build any token you want, and the builder is taking care of the position and length of your token.

```cs
private static Lexeme ScanNumber(ILexemeBuilder builder)
    => builder.Peek().Match(none: false, some: char.IsDigit)
        ? ScanNumber(builder.Retain())
        : builder.Build(new NumberToken(int.Parse(builder.CurrentToken)));
```

The LexerResult will have a sequence of lexems which would look like this: 

```
Lexeme(NumberToken(40),       Position(0, 1, 1, 2))
Lexeme(PlusToken(),           Position(2, 1, 3, 1))
Lexeme(NumberToken(20),       Position(3, 1, 4, 2))
Lexeme(MultiplicationToken(), Position(5, 1, 6, 1))
Lexeme(NumberToken(6),        Position(6, 1, 7, 1))
```

## Create a rulebook

You always start with an empty `LexerRuleBook.Builder` which is already in its default configuration, you just have to define your rules and an epsilon token to start.

### Define lexer rules

You can add rules with the following methods:

* `AddRule(Predicate<char> symbolPredicate, Lexeme.Factory createLexeme, int weight = 0)`
* `AddSimpleRule<TToken>(string textSymbol)`
* `AddRuleWithContext(Predicate<char> symbolPredicate, Predicate<IReadOnlyList<Lexeme>> contextPredicate, Lexeme.Factory createLexeme, int weight)`
* `AddSimpleRuleWithContext<TToken>(string textSymbol, Predicate<IReadOnlyList<Lexeme>> contextPredicate, int weight)`


The Simple rules create a token of type `TToken` when they see the specified `textSymbol` the others only look at the first character to decide which rules to invoke.


The WithContext rules have an additional contextPredicate which can decide if the rule should be activated or not. 
That way you can produce different symbols with the same input, for details look at the chapter "Lexemes dependent on context"

You can implement your own rules also by implementing the ILexerRule interface yourself. 
This will be necessary if you need to look ahead more than one character and the simple rule is not enough for your use case.

* `AddRule(ILexerRule rule)`

### Define an epsilon token

You have to define an epsilon token which marks the end of your token-stream. 

```cs
LexerRuleBook.Builder
    .WithEpsilonToken<EofToken>()
```

### Define a post process function

Sometimes you don't want to deal with every Token in your parser. A simple way to remove certain nodes,
or rewrite all your lexemes is to define a post-processing function which takes the list of lexmes and transform it to another list of lexemes.

Here is a simple example for a parser who is not interested in the the whitespace after lexing.

```cs
LexerRuleBook.Builder
    .WithPostProcess(lexemes => lexemes.Where(t => t.Token.GetType() != typeof(WhiteSpaceToken)))
```

### Configure the complex szenarios

In simple lexer scenerios you should be fine with the default implemenations but if you get

You can define your own definitions for `ILexerReader`, `ILinePositionCalculator`, `ILexemeWalker`, `ILexemeBuilder` in complex scenarios where you need more control over the lexin process.

* `WithLexerReader(ILexerReader.Factory newLexerReader)`
* `WithLinePositionCalculator(ILinePositionCalculator.Factory newLinePositionCalculator)`
* `WithLexemeWalker(ILexemeWalker.Factory newLexemeWalker)`
* `WithLexemeBuilder(ILexemeBuilder.Factory newLexemeBuilder)`

And you can change implementation details explained more in the "Change the default implementations" chapter.

### Build the LexerRuleBook

You can only call the Build method if you already defined an epsilon token.
The Build function will return a LexerRuleBook which has all the information needed to transform your expressions to lexemes.

```cs
// This is valid and defines an empty lexer which will throw on any input.
LexerRuleBook.Builder
    .WithEpsilonToken<EofToken>()
    .Build();
```

## Build a lexeme

The `ILexemeBuilder` helps you to create the lexeme, it automates the position calculations.

The most important function to investigate is the `Peek` function. It gives you the current character or `Option<char>.None` if you are at the end of the expression.

```cs
Option<char> Peek(int lookAhead = 0);
```

The `Peek` function does not advance the position, to advance the `Position` there are the `Retain()` and `Discard()` methods.

* Retain() will advance the `Position` and add the current character at the end of `CurrentToken`.
* Discard() will only advance the `Position` and discard the current character.

Sometimes you want to start over with the `CurrentToken`, in that case use `Clear()`.

you can look at the underlying absolute `Position` if you want, but it usually is not necessary.

At the end you have to build a lexeme, a lexeme is the token you are constructing ant the position information (Start, End, Line etc.). 

The `Build(IToken token)` method on the `ILexemeBuilder` will construct everything for you, you just have to create the Token and return the Lexeme.

### Example for ILexemeBuilder

Here we have a realistic more complex example where we want to create NumberToken which represent a floating point number.

```cs
private static Lexeme ScanNumber(ILexemeBuilder builder)
{
    var decimalExists = false;

    // we iterate as long we see a dot or a digit.
    while (builder.Peek().Match(none: false, some: c => c is '.' || char.IsDigit(c)))
    {
        var digit = builder.Peek();
        var isDot = from d in digit select d is '.';

        // only one dot is allowed so we throw an exception if we find a second one.
        if (isDot.Match(none: false, some: Identity))
        {
            if (decimalExists)
            {
                throw new LexerException("Multiple dots in decimal number");
            }

            decimalExists = true;
        }

        builder.Retain();
    }

    // here we build the lexeme based on our double we constructed and put in a NumberToken.
    return double.TryParse(builder.CurrentToken, out var number)
        ? builder.Build(new NumberToken(number))
        : throw new Exception("Could not parse number: " + builder.CurrentToken);
}
}
```

## Line position calculation

The default line position calculation is very simple, 
you designate one ore more of your token as `ILineBreakToken` and the lexer is
automatically calculating the position of each lexeme in the result.

```cs
public record NewLineToken : ILineBreakToken;
```

### Position

The absolute positions (`StartPosition`, `EndPosition`) are zero indexed because they usually are used in a context familiar with this.
The line and column positions (`Line`,`StartColumn`,`EndColumn`) are one indexed because these positions usually are shown in the output of end users.

### this is not enough?

If you need anything more complicated you should read the chapter "Change the default implementations".

## Lexemes dependent on context

Sometimes you want to create different Token for the same input.

In such cases you want `AddRuleWithContext` on the `LexerRuleBookBuilder`, these rules can arbitrarly be activated by the lexemes which came before.

These rules have an aditional parameter, the context predicate. These rules are only activated when this predicate returns true. The predicate works on the list of the lexemes already produced.

### Example

Lets say we have want to have a token class, but we want to allow to use it as an identifier if the class token already appeared previously.

```cs
LexerRuleBook.Builder
    .AddRule(char.IsLetter, ScanIdentifier)
    .AddSimpleRuleWithContext<ClassToken>("class", context => context.All(lexeme => lexeme is not { Token: ClassToken }), 1)
```

We first define a rule which creates the identifier token, work as usual and has the default weight of 0.
Then we add a rule with context and restrict the usage of it to contexts which have no ClassToken so far. 
We give this a weight of 1 so this matches first if it is activated.

For the example input below this means the first class will be created as `ClassToken()` and the second one will be created as `IdentifierToken("class")`

```
class MySuperClass
{
    function DoSomething
    {
       let class = anything
    }
}
```

## implement ILexerRule yourself

You have even more freedom if you implement the ILexerRule yourself. The interface is very simple and for simple cases you usually only have to implement the Match function.

```cs
public interface ILexerRule
{
    int Weight { get; }
    Option<Lexeme> Match(ILexemeBuilder builder);
    bool IsActive(IReadOnlyList<Lexeme> context);
}
```

The weight just defines the precedence of the rules if more than one rule matches.

IsActive is usually true except when your rule depends on the context, and you need to deactivate it.

The Match function works very similar to the lexeme factory, when the rule matches it should return a lexeme, but if it does not it needs to return `Option<Lexeme>.None`.

**Attention**: If you return `Option<Lexeme>.None` you should only have used Peek to inspect the underyling stream. You should not change the underlying stream by advancing it with `Retain()` or `Discard()`.

This is illustrated in this simple example, where we call `Discard()` to advance the stream only when create a `Lexeme`.

```cs
    public Option<Lexeme> Match(ILexemeBuilder builder)
    {
        if (builder.Peek() is '=')
        {
            builder.Discard()

            return builder.Build(new EqualToken())
        } else {
            return Option<Lexeme>.None;
        }
```

## Change the default implementations.

The LexerRuleBook sets up the default implementations for: 

* `ILinePositionCalculator`
* `ILexerReader`
* `ILexemeWalker`
* `ILexemeBuilder`

You change them while setting up your `LexerRuleBook` the methods start with `With` and are described in the "Configure the complex szenarios" chapter.

## Create your own ILinePositionCalculator

**Scenario**: the positions are not the way I want them.

The `CalculateLinePosition` is the only method in the interface has only one method the you have to implement which is straight forward.

```cs
Position CalculateLinePosition(int absolutePosition, int length);
```

When registering the calculator you have access to the `lexemes`. The `lexemes` is a `IReadOnlyList<Lexeme>` produced before your calculator is created.

```cs
    .WithLinePositionCalculator(lexemes => new ZeroBasedPositionCalculator(lexemes))
```

## Create your own ILexerReader

**Scenario**: you want to manipulate the input before it is used by the logic.

`ILexerReader` has two methods and a property you need to implemlent.

```cs
int Position { get; }
Option<char> Peek(int lookAhead = 0);
Option<char> Read();
```

When registering the reader you have access to the `expression` as given to the `Scan` method. The `expression` is therefore a `string`.

```cs
    .WithLexerReader(expression => new TrimLexerReader(expression))
```

## Create your own ILexemeWalker

**Scenario**: you want to manipulate the result after the lexing.


`ILexemeWalker` is usually stateful (track the position and change it on `Pop`) and has two methods to implemlent.

```cs
    Lexeme Pop();
    Lexeme Peek(int lookAhead = 0);
```

When registering the reader you have access to the `lexemes` and the `newEpsilonToken` factory delegate. 
The `lexemes` is a `IReadOnlyList<Lexeme>` which is the list of all lexemes produced by the expression.
The `newEpsilonToken` is a simple facotry which returns the configured epsilon token.

Usually the lexeme walker always returns a lexeme on `Pop()` and `Peek()`, even if you read past the end. In those cases the lexeme walker returns the epsilon token.

```cs
    .WithLexemeWalker((lexemes, newEpsilonToken) => new SkipLexemeWalker(lexmes, newEpsilonToken, skippables))
```

## Create your own ILexemeBuilder

**Scenario**: you want to do anything else.

By replacing the lexeme builder you are basically taking control over everything, you can ignore most of the default implementation and do whatever you want. 

*At this point you have to ask yourself, is this the right library for you?*

The `ILexemeBuilder` is a bit more involved, if you really want to change this implementation refer to the source and the examples in the test.

When registering the lexeme builder you have access to the `reader` and the `linePositionCalculator`. Here you have full controle to change basically everything and ignore most of the parts.

```cs
    .WithLexemeBuilder((reader, linePositionCalculator) => new FakeDigitLexemeBuilder(reader, linePositionCalculator))
```
