using System.Text.Json.Serialization;

namespace StarLight_Core.Models.Utilities;

internal class LibraryJsonRule
{
    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("os")]
    public Os Os { get; set; }
}