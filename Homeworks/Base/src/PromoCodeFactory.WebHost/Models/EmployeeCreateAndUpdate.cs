using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeCreateAndUpdate
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public Guid[] Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}