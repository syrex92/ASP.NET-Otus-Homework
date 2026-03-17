using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    public class Mutation
    {
        public async Task<bool> DeleteCustomer(Guid id, [Service] IRepository<Customer> _customerRepository)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return false;

            await _customerRepository.DeleteAsync(customer);

            return true;
        }

        public async Task<bool> EditCustomer(Guid id, CreateOrEditCustomerRequest request, [Service] IRepository<Customer> _customerRepository,
            [Service] IRepository<Preference> _preferenceRepository)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return false;

            var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

            CustomerMapper.MapFromModel(request, preferences, customer);

            await _customerRepository.UpdateAsync(customer);

            return true;
        }

        public async Task<Guid> CreateCustomer(CreateOrEditCustomerRequest request, [Service] IRepository<Customer> _customerRepository,
            [Service] IRepository<Preference> _preferenceRepository)
        {
            //Получаем предпочтения из бд и сохраняем большой объект
            var preferences = await _preferenceRepository
                .GetRangeByIdsAsync(request.PreferenceIds);

            Customer customer = CustomerMapper.MapFromModel(request, preferences);

            await _customerRepository.AddAsync(customer);

            return customer.Id;
        }
    }
}