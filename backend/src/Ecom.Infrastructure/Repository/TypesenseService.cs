using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using Typesense;
using Typesense.Setup;

namespace Ecom.Infrastructure.Repository
{
    public class TypesenseService
    {
        private readonly ITypesenseClient _client;

        public TypesenseService(IConfiguration config)
        {
            var url = config["Authentication:Typesense:Url"]!;
            var adminKey = config["Authentication:Typesense:AdminKey"]!;

            var nodes = new List<Node> { new Node(url, "443", "https") };
            var configTs = new Config(nodes, adminKey);

            var options = Options.Create(configTs);
            var httpClient = new HttpClient();

            _client = new TypesenseClient(options, httpClient);
        }

        public async Task EnsureCollectionExistsAsync()
        {
            var schema = new Schema
            {
                Name = "products",
                Fields = new List<Field>
                {
                    new Field("id",            FieldType.String,     false),
                    new Field("name",          FieldType.String,     false),
                    new Field("slug",          FieldType.String,     false),
                    new Field("brandName",     FieldType.String,     false),
                    new Field("categoryName",  FieldType.String,     false),
                    new Field("tags",          FieldType.StringArray,false),
                    new Field("price",         FieldType.Float,      false),
                    new Field("description",   FieldType.String,     false),
                    new Field("imageUrl",      FieldType.String,     false),
                    new Field("stockQuantity", FieldType.Int32,      false)
                },
                DefaultSortingField = "price"
            };

            try
            {
                await _client.CreateCollection(schema);
            }
            catch { } 
        }

        public ITypesenseClient Client => _client;
    }
}