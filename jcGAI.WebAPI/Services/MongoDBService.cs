﻿using jcGAI.WebAPI.Objects.Config;
using jcGAI.WebAPI.Objects.NonRelational;
using jcGAI.WebAPI.Objects.NonRelational.Base;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace jcGAI.WebAPI.Services
{
    public class MongoDbService
    {
        private readonly MongoDbConfig _config;
        private readonly IMongoDatabase _mongoDbClient;

        public MongoDbService(IOptions<MongoDbConfig> configuration)
        {
            _config = configuration.Value;
            _mongoDbClient = new MongoClient(_config.ConnectionString).GetDatabase(_config.DatabaseName);
        }

        private IMongoCollection<T> GetCollection<T>(string? collectionName = null)
        {
            var collection = collectionName ?? _config.CollectionName;

            if (_mongoDbClient.ListCollectionNames().ToEnumerable().All(c => c != collection))
            {
                _mongoDbClient.CreateCollection(collection);
            }

            return _mongoDbClient.GetCollection<T>(collection);
        }

        public async Task<Guid> InsertAsync<T>(T obj) where T : BaseNonRelational
        {
            await GetCollection<T>(typeof(T).Name).InsertOneAsync(obj);

            return obj.Id;
        }

        public async Task<bool> InsertActivityAsync(int userId, byte[] file)
        {
            await GetCollection<Activities>(nameof(Activities)).InsertOneAsync(new Activities
            {
                GpxFileData = file,
                TimeStamp = DateTime.Now,
                UserId = userId
            });

            return true;
        }

        public async Task<List<Activities>> GetActivitiesAsync(int userId) => 
            await GetCollection<Activities>(nameof(Activities)).FindSync(a => a.UserId == userId).ToListAsync();

        public async Task<ActionResult<Guid>> InsertUserAsync(Users users)
        {
            await GetCollection<Users>(nameof(Users)).InsertOneAsync(users);

            return users.Id;
        }
    }
}