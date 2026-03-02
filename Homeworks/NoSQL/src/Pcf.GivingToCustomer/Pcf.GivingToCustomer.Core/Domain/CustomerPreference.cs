using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pcf.GivingToCustomer.Core.Domain
{
    public class CustomerPreference
    {
        [BsonRepresentation(BsonType.String)]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid PreferenceId { get; set; }
        public virtual Preference Preference { get; set; }
    }
}