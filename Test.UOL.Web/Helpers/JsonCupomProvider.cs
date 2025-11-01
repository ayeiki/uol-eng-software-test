using System.Text.Json;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using System.Text.Json.Serialization;

namespace Test.UOL.Web.Helpers;

file class CupomRoot
{
    [JsonPropertyName("coupons")]
    public List<CupomItem> cupons { get; set; } = new();
}

public sealed class JsonCupomProvider : ICupomProvider
{
    private readonly string _path;
    private IReadOnlyDictionary<string, CupomItem>? _cache;
    private readonly object _lock = new();

    public JsonCupomProvider(string pathToCupomJs)
    {
        _path = pathToCupomJs;
    }

    public CupomItem? GetCupom(string key)
    {
        LoadArquivo();
        return _cache!.TryGetValue(key.Trim(), out var c) ? c : null;
    }

    private void LoadArquivo()
    {
        if (_cache is not null) return;

        lock (_lock)
        {
            if (_cache is not null) return;

            if (!File.Exists(_path))
                throw new FileNotFoundException($"Arquivo de cupons não encontrado: {_path}");

            var json = File.ReadAllText(_path);
            var root = JsonSerializer.Deserialize<CupomRoot>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new CupomRoot();

            var dict = new Dictionary<string, CupomItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var it in root.cupons)
            {
                if (string.IsNullOrWhiteSpace(it.key)) continue;
                dict[it.key.Trim()] = it;
            }

            _cache = dict;
        }
    }
}