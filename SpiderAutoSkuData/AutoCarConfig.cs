using System.Text.Json.Serialization;

namespace SpiderAutoSkuData;

public sealed class AutoCarConfig
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("result")]
    public ConfigResult? Result { get; init; }

    [JsonPropertyName("returncode")]
    public string? ReturnCode { get; init; }
}

public sealed class ConfigResult
{
    [JsonPropertyName("specid")]
    public string? SpecId { get; init; }

    [JsonPropertyName("configtypeitems")]
    public List<ConfigTypeItem> ConfigTypeItems { get; init; } = [];
}

public sealed class ConfigTypeItem
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("configitems")]
    public List<ConfigItem> ConfigItems { get; init; } = [];
}

public sealed class ConfigItem
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}
