using System.Diagnostics;
using NMeCab.Specialized;

// --- Benchmark: MeCab.NET Performance ---

// Find the IPAdic dictionary
var dicDir = AppDomain.CurrentDomain.BaseDirectory;
while (!Directory.Exists(Path.Combine(dicDir, "dic", "ipadic")))
    dicDir = Directory.GetParent(dicDir)!.FullName;
dicDir = Path.Combine(dicDir, "dic", "ipadic");

Console.WriteLine($"Dictionary: {dicDir}");
Console.WriteLine();

// 1. Tagger creation benchmark
Console.WriteLine("=== Tagger Creation ===");
var sw = Stopwatch.StartNew();
for (int i = 0; i < 10; i++)
{
    using var t = MeCabIpaDicTagger.Create(dicDir);
}
sw.Stop();
Console.WriteLine($"  10 iterations: {sw.ElapsedMilliseconds}ms (avg {sw.ElapsedMilliseconds / 10.0:F1}ms per create)");
Console.WriteLine();

// 2. Parse single sentence benchmark
using var tagger = MeCabIpaDicTagger.Create(dicDir);

var sentences = new[]
{
    "日本語の形態素解析を行います",
    "東京特許許可局局長今日急遽休暇許可却下",
    "すもももももももものうち",
    "彼女は新しいレストランで美味しい料理を食べました",
    "プログラミング言語の中でC#は最も人気のある言語の一つです",
};

Console.WriteLine("=== Single Sentence Parse (10,000 iterations each) ===");
foreach (var sentence in sentences)
{
    sw.Restart();
    for (int i = 0; i < 10_000; i++)
    {
        var nodes = tagger.Parse(sentence);
    }
    sw.Stop();
    var usPerParse = sw.Elapsed.TotalMicroseconds / 10_000;
    Console.WriteLine($"  \"{sentence}\"");
    Console.WriteLine($"    {usPerParse:F1}µs/parse ({10_000_000.0 / sw.Elapsed.TotalMicroseconds:F0} parses/sec)");
}
Console.WriteLine();

// 3. Parse vs ParseToNodes comparison
Console.WriteLine("=== Parse() vs ParseToNodes() (10,000 iterations) ===");
var testSentence = "日本語の形態素解析を行います";

sw.Restart();
for (int i = 0; i < 10_000; i++)
{
    var nodes = tagger.Parse(testSentence);
    foreach (var n in nodes) { _ = n.Surface; }
}
sw.Stop();
var parseUs = sw.Elapsed.TotalMicroseconds / 10_000;

sw.Restart();
for (int i = 0; i < 10_000; i++)
{
    foreach (var n in tagger.ParseToNodes(testSentence)) { _ = n.Surface; }
}
sw.Stop();
var parseToNodesUs = sw.Elapsed.TotalMicroseconds / 10_000;

Console.WriteLine($"  Parse():        {parseUs:F1}µs/call");
Console.WriteLine($"  ParseToNodes(): {parseToNodesUs:F1}µs/call");
Console.WriteLine();

// 4. Kokoro.txt full-text benchmark (if available)
var kokoroPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kokoro.txt");
if (!File.Exists(kokoroPath))
{
    // Try to find it
    var searchDir = AppDomain.CurrentDomain.BaseDirectory;
    while (!File.Exists(Path.Combine(searchDir, "kokoro.txt")) && Directory.GetParent(searchDir) != null)
        searchDir = Directory.GetParent(searchDir)!.FullName;
    kokoroPath = Path.Combine(searchDir, "kokoro.txt");
}

if (File.Exists(kokoroPath))
{
    var lines = File.ReadAllLines(kokoroPath);
    Console.WriteLine($"=== Full Text: kokoro.txt ({lines.Length} lines) ===");
    
    sw.Restart();
    int totalNodes = 0;
    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;
        var nodes = tagger.Parse(line);
        totalNodes += nodes.Length;
    }
    sw.Stop();
    Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
    Console.WriteLine($"  Lines: {lines.Length}");
    Console.WriteLine($"  Nodes: {totalNodes}");
    Console.WriteLine($"  Throughput: {lines.Length * 1000.0 / sw.ElapsedMilliseconds:F0} lines/sec");
}
else
{
    Console.WriteLine("=== kokoro.txt not found, skipping full-text benchmark ===");
}

// 5. Memory / allocation info
Console.WriteLine();
Console.WriteLine("=== Memory ===");
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
Console.WriteLine($"  Working set: {Environment.WorkingSet / 1024 / 1024}MB");
Console.WriteLine($"  GC total memory: {GC.GetTotalMemory(true) / 1024 / 1024}MB");