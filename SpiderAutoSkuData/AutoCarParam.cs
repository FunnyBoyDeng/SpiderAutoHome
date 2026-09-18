using System.Text.Json.Serialization;

namespace SpiderAutoSkuData;

public sealed class AutoCarParam
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("result")]
    public ParamResult? Result { get; init; }

    [JsonPropertyName("returncode")]
    public string? ReturnCode { get; init; }
}

public sealed class ParamResult
{
    [JsonPropertyName("specid")]
    public string? SpecId { get; init; }

    [JsonPropertyName("paramtypeitems")]
    public List<ParamTypeItem> ParamTypeItems { get; init; } = [];
}

public sealed class ParamTypeItem
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("paramitems")]
    public List<ParamItem> ParamItems { get; init; } = [];
}

public sealed class ParamItem
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}
