using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pcf.GivingToCustomer.Core.Domain
{
    public class PromoCodeCustomer : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid PromoCodeId { get; set; }
        public virtual PromoCode PromoCode { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
