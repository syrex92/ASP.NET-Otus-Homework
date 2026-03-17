using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    public class Query
    {
        public async Task<IEnumerable<Customer>> GetCustomers([Service] IRepository<Customer> customerRepository) =>
            await customerRepository.GetAllAsync();

        public async Task<Customer> GetCustomer(Guid id, [Service] IRepository<Customer> customerRepository) =>
            await customerRepository.GetByIdAsync(id);
    }
}