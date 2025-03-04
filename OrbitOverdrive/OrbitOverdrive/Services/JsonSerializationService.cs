using System.Text.Json;

namespace OrbitOverdrive.Services;

/// <summary>
/// Service for serializing and deserializing JSON.
/// Uses camel-casing for property names.
/// </summary>
public static class JsonSerializationService
{
    private static readonly JsonSerializerOptions? Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The JSON string representation of the object.</returns>
    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }
    
    /// <summary>
    /// Deserializes a JSON string to an object.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <returns>The deserialized object.</returns>
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}
