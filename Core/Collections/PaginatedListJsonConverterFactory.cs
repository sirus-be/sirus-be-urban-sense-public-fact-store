using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Collections
{
    class PaginatedListJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            var type = typeToConvert;
            if (!type.IsGenericTypeDefinition)
                type = type.GetGenericTypeDefinition();

            return type == typeof(PaginatedList<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var genericType = typeToConvert.GenericTypeArguments[0];
            var converter = typeof(PaginatedListJsonConverter<>).MakeGenericType(genericType);
            return (JsonConverter)Activator.CreateInstance(converter);
        }
    }
}
