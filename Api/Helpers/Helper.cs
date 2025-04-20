using System.Text.Json;

namespace ApiGateway.Helpers
{
    public static class Helper
    {

        public static void PrintObject(object obj)
        {
            string printValue = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            });
            Console.WriteLine(printValue);
        }

        public static string StringifyObject(object obj)
        {
            string printValue = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            });
            return printValue;
        }
    }
}