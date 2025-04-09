using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPR.RegulatorService.Facade.Core.Helpers.Converter;

[ExcludeFromCodeCoverage]
public class CustomEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Nullable.GetUnderlyingType(typeToConvert) != null) 
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default; 

            return ReadEnumValue(ref reader, typeToConvert);
        }

        return ReadEnumValue(ref reader, typeToConvert); 
    }

    private T ReadEnumValue(ref Utf8JsonReader reader, Type typeToConvert)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString();
            if (Enum.TryParse(value, true, out T enumValue))
                return enumValue;

            throw new JsonException($"Unable to convert '{value}' to enum of type {typeToConvert}.");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            int value = reader.GetInt32();
            return (T)(object)value;
        }

        throw new JsonException($"Expected string or number for enum {typeToConvert}, but got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
