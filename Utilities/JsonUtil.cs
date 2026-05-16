using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace StarLight_Core.Utilities;

/// <summary>
/// 
/// </summary>
public static class JsonUtil
{
    private static readonly JsonSerializerOptions? Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// JSON 序列化
    /// </summary>
    /// <param name="json"></param>
    /// <param name="jsonType"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? Deserialize<T>(this string json, JsonTypeInfo<T?> jsonType) => JsonSerializer.Deserialize(json, jsonType);

    /// <summary>
    /// JSON 反序列化
    /// </summary>
    /// <param name="json"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static T? ToJsonEntry<T>(this string json, JsonSerializerOptions? options = null)
    {
        if (json == null) 
            throw new ArgumentNullException(nameof(json));
        var finalOptions = options ?? Options;
        try
        {
            return JsonSerializer.Deserialize<T>(json, finalOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("JSON 反序列化失败", ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(this object obj) => JsonSerializer.Serialize(obj, Options);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static JsonNode? ToJsonNode(this string json) => JsonNode.Parse(json);
}