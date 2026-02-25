# MeCab.NET

A Japanese morphological analysis engine for .NET, modernized for .NET 10+.

> **Fork of [NMeCab](https://github.com/komutan/NMeCab)** by Tsuyoshi Komuta, with contributions from [MeCab.DotNet](https://github.com/kekyo/MeCab.DotNet) by Kouji Matsui.  
> Maintained by [Prismify](https://github.com/prismify-co).

📖 日本語ドキュメントは [README.md](./README.md) をご覧ください。

---

## What is MeCab.NET?

MeCab.NET is a pure C# implementation of the [MeCab](https://taku910.github.io/mecab/) morphological analyzer. It segments Japanese text into tokens and provides part-of-speech tags, readings, pronunciation, and more — with no native dependencies.

```
Input:  "日本語の形態素解析を行います"
Output: 日本語 / 名詞 / ニホンゴ
        の     / 助詞
        形態素 / 名詞 / ケイタイソ
        解析   / 名詞 / カイセキ
        を     / 助詞
        行い   / 動詞 / オコナイ
        ます   / 助動詞
```

## Quick Start

### 1. Install

```bash
dotnet add package LibNMeCab.IpaDicBin
```

This installs both the core library and the IPAdic dictionary.

### 2. Parse text

```csharp
using NMeCab.Specialized;

// Create a tagger with the bundled IPAdic dictionary
using var tagger = MeCabIpaDicTagger.Create();

// Parse a sentence
var nodes = tagger.Parse("日本語の形態素解析を行います");

foreach (var node in nodes)
{
    Console.WriteLine($"{node.Surface}\t{node.PartsOfSpeech}\t{node.Reading}");
}
```

### Output

```
日本語  名詞    ニホンゴ
の      助詞    ノ
形態素  名詞    ケイタイソ
解析    名詞    カイセキ
を      助詞    ヲ
行い    動詞    オコナイ
ます    助動詞  マス
```

## API Reference

### Taggers

| Tagger | Dictionary | Node Type |
|--------|-----------|-----------|
| `MeCabTagger` | Any (generic) | `MeCabNode` |
| `MeCabIpaDicTagger` | IPAdic | `MeCabIpaDicNode` |
| `MeCabUniDic21Tagger` | UniDic 2.1 | `MeCabUniDic21Node` |
| `MeCabUniDic22Tagger` | UniDic 2.2+ | `MeCabUniDic22Node` |

### MeCabIpaDicNode Properties

| Property | Description | IPAdic CSV Index |
|----------|-------------|-----------------|
| `Surface` | Surface form (表層形) | — |
| `PartsOfSpeech` | Part of speech (品詞) | 0 |
| `PartsOfSpeechSection1` | POS subcategory 1 (品詞細分類1) | 1 |
| `PartsOfSpeechSection2` | POS subcategory 2 (品詞細分類2) | 2 |
| `PartsOfSpeechSection3` | POS subcategory 3 (品詞細分類3) | 3 |
| `ConjugatedForm` | Conjugated form (活用形) | 4 |
| `Inflection` | Inflection type (活用型) | 5 |
| `OriginalForm` | Base/dictionary form (原形) | 6 |
| `Reading` | Reading in katakana (読み) | 7 |
| `Pronounciation` | Pronunciation in katakana (発音) | 8 |

### Advanced Features

```csharp
// N-Best results (multiple parsing candidates)
var results = tagger.ParseNBest("すもももももももものうち");
foreach (var nodes in results.Take(3))
{
    // Each 'nodes' is a different parsing candidate
}

// Soft segmentation (with probabilities)
var softNodes = tagger.ParseSoftWakachi("すもももももももものうち", theta: 0.75f);
foreach (var node in softNodes)
{
    Console.WriteLine($"{node.Surface}: {node.Prob:P}");
}

// Custom dictionary directory
using var custom = MeCabIpaDicTagger.Create(dicDir: "/path/to/custom/dic");
```

## Requirements

- .NET 10.0 or later

## License

Dual licensed under **GPL-2.0-or-later** OR **LGPL-2.1-or-later**.

See [COPYING](./COPYING), [GPL](./GPL), and [LGPL](./LGPL) for details.

### Third-party

- **MeCab** — Taku Kudo & NTT (GPL / LGPL / BSD)
- **NMeCab** — Tsuyoshi Komuta (GPL / LGPL)
- **MeCab.DotNet** — Kouji Matsui (GPL / LGPL)
- **IPAdic** — NAIST (BSD-like)
