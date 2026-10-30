using System.Net;
using System.Text.Json;
namespace PersianScreenTranslator;
internal static class Translator
{
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(12) };
    private static readonly Dictionary<string,string> Cache = new(StringComparer.Ordinal);
    public static async Task<string> EnglishToPersianAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        lock (Cache) if (Cache.TryGetValue(input, out var cached)) return cached;
        var url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=fa&dt=t&q=" + Uri.EscapeDataString(input);
        using var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var parts = doc.RootElement[0];
        var result = new System.Text.StringBuilder();
        foreach (var part in parts.EnumerateArray())
            if (part.ValueKind == JsonValueKind.Array && part.GetArrayLength() > 0 && part[0].ValueKind == JsonValueKind.String)
                result.Append(part[0].GetString());
        var translated = result.Length > 0 ? result.ToString() : input;
        lock (Cache) Cache[input] = translated;
        return translated;
    }
}
