using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class PreferenceResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public List<CustomerShortResponse> Customers { get; set; }
    }
}