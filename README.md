﻿# Funcky.Lexer

Can you write a lexer in a single expression? Now you can.

[![Build](https://github.com/polyadic/funcky-lexer/workflows/Build/badge.svg)](https://github.com//polyadic/funcky-lexer/actions?query=workflow%3ABuild)
[![Licence: MIT](https://img.shields.io/badge/licence-MIT-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-MIT)
[![Licence: Apache](https://img.shields.io/badge/licence-Apache-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-Apache)

## Packages

* **Funcky.Lexer**
  [![NuGet package](https://buildstats.info/nuget/Funcky.Lexer)](https://www.nuget.org/packages/Funcky.Lexer)


## Example

Define your tokens by marking them with the `IToken` interface.

```cs
public record PlusToken : IToken;
public record MinusToken : IToken;
public record MultiplicationToken : IToken;
public record DivisionToken : IToken;
```

You have to define an EpsilonToken marked with the `IEpsilonToken` interface. This will be returned by the LexerReader when you read past the end.

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

Use the lexeme-builder to build any token you want.

```cs
private static Lexeme ScanNumber(ILexemeBuilder builder)
    => builder.Peek().Match(none: false, some: char.IsDigit)
        ? ScanNumber(builder.Retain())
        : builder.Build(new NumberToken(int.Parse(builder.CurrentToken)));
```

## Create a rulebook

You always start with an empty `LexerRuleBook.Builder`

## Build a lexeme

## Line position calculation

## Lexemes dependent on context

## Non-default implementations


