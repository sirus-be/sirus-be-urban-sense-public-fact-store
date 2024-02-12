using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Collections
{
    public class PaginatedListJsonConverter<T> : JsonConverter<PaginatedList<T>>
    {
        public override PaginatedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int pageIndex = 0;
            int pageSize = 0;
            int totalItems = 0;
            var items = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    if (propertyName == "pageIndex")
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.Number)
                            pageIndex = reader.GetInt32();
                    }
                    else if (propertyName == "pageSize")
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.Number)
                            pageSize = reader.GetInt32();
                    }
                    else if (propertyName == "totalItems")
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.Number)
                            totalItems = reader.GetInt32();
                    }
                    else if (propertyName == "items")
                    {
                        reader.Read();
                        items = JsonSerializer.Deserialize<List<T>>(ref reader);
                    }
                }
            }

            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            if (pageSize <= 0)
            {
                pageSize = items.Count;
            }
            if (totalItems < items.Count)
            {
                totalItems = items.Count;
            }

            return new PaginatedList<T>(pageIndex, pageSize, totalItems, items);
        }

        public override void Write(Utf8JsonWriter writer, PaginatedList<T> value, JsonSerializerOptions options)
        {
            var valueCollection = value as IEnumerable;

            writer.WriteStartObject();
            writer.WriteNumber("pageIndex", value.PageIndex);
            writer.WriteNumber("pageSize", value.PageSize);
            writer.WriteNumber("totalItems", value.Count);
            writer.WriteStartArray("items");
            foreach (var valueItem in valueCollection)
                JsonSerializer.Serialize(writer, valueItem);
            writer.WriteEndArray();
            writer.WriteEndObject();

        }
    }
}