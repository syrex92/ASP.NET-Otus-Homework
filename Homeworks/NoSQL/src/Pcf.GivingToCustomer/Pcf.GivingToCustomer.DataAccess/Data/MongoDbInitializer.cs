using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoDbInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public void InitializeDb()
        {
            _database.DropCollection(nameof(Preference));
            _database.DropCollection(nameof(Customer));

            var prefs = _database.GetCollection<Preference>(nameof(Preference));
            prefs.InsertMany(FakeDataFactory.Preferences);

            var customers = _database.GetCollection<Customer>(nameof(Customer));
            customers.InsertMany(FakeDataFactory.Customers);
        }
    }
}
