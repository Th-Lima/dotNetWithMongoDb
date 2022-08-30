using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace dotNetWithMongo.Api.Data
{
    public class MongoDb
    {
        public IMongoDatabase Db { get; set; }
        public MongoDb(IConfiguration configuration)
        {
            try
            {
                var settings = MongoClientSettings.FromUrl(new MongoUrl(configuration["connectionString"]));
                var client = new MongoClient(settings);
                Db = client.GetDatabase(configuration["NomeBanco"]);
                MapClasses();
            }
            catch (Exception ex)
            {
                throw new MongoException("Não foi possível se conectar ao MongoDb", ex);
            }
        }
    }
}
