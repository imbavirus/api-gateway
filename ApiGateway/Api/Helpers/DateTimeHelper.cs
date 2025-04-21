using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiGateway.Api.Helpers;

public class DateTimeHelper : JsonConverter<DateTime>
{
    private const string DateFormat = "dd-MMM-yy HH:mm:ss"; // Specified format

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));

        var dateString = reader.GetString();

        if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        // Fallback to default parsing if your specific format fails
        if (DateTime.TryParse(dateString, out date))
        {
            return date;
        }

        // Handle error: Throw exception or return default value
        throw new JsonException($"Unable to parse \"{dateString}\" as DateTime using format {DateFormat}.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write in a standard format (ISO 8601 is recommended for APIs)
        writer.WriteStringValue(value.ToString("o", CultureInfo.InvariantCulture));
    }
}
