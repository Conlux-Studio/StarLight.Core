using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace StarLight_Core.Utilities;

// TODO: 使用源生成器以便提供 AOT 支持
/// <summary>
/// Json 工具类
/// </summary>
public static class JsonUtil
{
    private static readonly JsonSerializerOptions? Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// 使用源生成器提供的类型元数据反序列化 JSON 字符串
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <param name="jsonType">源生成器提供的类型元数据</param>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>反序列化后的对象</returns>
    /// <exception cref="ArgumentNullException">
    /// 当 <paramref name="json"/> 或 <paramref name="jsonType"/> 为 null 时抛出
    /// </exception>
    /// <exception cref="ArgumentException">当 <paramref name="json"/> 为空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">JSON 格式无效或反序列化失败时抛出</exception>
    public static T? Deserialize<T>(this string json, JsonTypeInfo<T?> jsonType)
    {
        if (json is null)
            throw new ArgumentNullException(nameof(json));
        if (jsonType is null)
            throw new ArgumentNullException(nameof(jsonType));
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON 字符串不能为 null 或空白", nameof(json));

        try
        {
            return JsonSerializer.Deserialize(json, jsonType);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"JSON 反序列化失败，类型：{typeof(T).Name}", ex);
        }
    }

    /// <summary>
    /// 使用反射机制反序列化 JSON 字符串
    /// </summary>
    /// <param name="json">要反序列化的 JSON 字符串</param>
    /// <param name="options">可选的序列化选项</param>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>反序列化后的对象</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="json"/> 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当 <paramref name="json"/> 为空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">JSON 格式无效或反序列化失败时抛出</exception>
    public static T? ToJsonEntry<T>(this string json, JsonSerializerOptions? options = null)
    {
        if (json is null)
            throw new ArgumentNullException(nameof(json));
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON 字符串不能为 null 或空白", nameof(json));

        var finalOptions = options ?? Options;
        try
        {
            return JsonSerializer.Deserialize<T>(json, finalOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"JSON 反序列化失败，类型：{typeof(T).Name}", ex);
        }
    }

    /// <summary>
    /// 将对象序列化为 JSON 字符串
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>表示对象的 JSON 字符串；若 <paramref name="obj"/> 为 null，则返回字符串 "null"</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="obj"/> 为 null 时抛出</exception>
    public static string Serialize(this object? obj)
    {
        return obj is null ? throw new ArgumentNullException(nameof(obj)) : JsonSerializer.Serialize(obj, Options);
    }

    /// <summary>
    /// 将 JSON 字符串解析为 <see cref="JsonNode"/> 对象
    /// </summary>
    /// <param name="json">要解析的 JSON 字符串</param>
    /// <returns>
    /// 表示 JSON 根节点的 <see cref="JsonNode"/> 对象；如果输入字符串为 null 或空，则返回 null
    /// </returns>
    /// <exception cref="JsonException">JSON 字符串格式无效时抛出</exception>
    public static JsonNode? ToJsonNode(this string json) => JsonNode.Parse(json);
    
    /// <summary>
    /// 将 JSON 字符串解析为 <see cref="JsonDocument"/> 对象
    /// </summary>
    /// <param name="json">要解析的 JSON 字符串</param>
    /// <returns>
    /// 表示 JSON 根节点的 <see cref="JsonDocument"/> 对象；如果输入字符串为 null 或空，则返回 null
    /// </returns>
    /// <exception cref="JsonException">JSON 字符串格式无效时抛出</exception>
    public static JsonDocument ToJsonDocument(this string json) => JsonDocument.Parse(json);
}