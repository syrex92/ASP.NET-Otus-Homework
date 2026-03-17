using System.Threading.Tasks;
using Grpc.Core;
using CustomersGrpc;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System.Linq;
using System;
using Pcf.GivingToCustomer.WebHost.Mappers;

namespace Pcf.GivingToCustomer.Services;

public class CustomersGrpcService : CustomersService.CustomersServiceBase
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Preference> _preferenceRepository;

    public CustomersGrpcService(IRepository<Customer> customerRepository,
        IRepository<Preference> preferenceRepository)
    {
        _customerRepository = customerRepository;
        _preferenceRepository = preferenceRepository;
    }

    public override async Task<CustomersListResponse> GetCustomers(Empty request, ServerCallContext context)
    {
        var customers = await _customerRepository.GetAllAsync();

        var customersList = customers.Select(c => new CustomerShort
        {
            Id = c.Id.ToString(),
            Email = c.Email,
            FirstName = c.FirstName,
            LastName = c.LastName
        });

        var response = new CustomersListResponse();
        response.Customers.AddRange(customersList);

        return response;     
    }

    public override async Task<CustomerResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
    {
        var customer = await _customerRepository.GetByIdAsync(new System.Guid(request.Id));

        return new CustomerResponse
        {
            Id = customer.Id.ToString(),
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName
        };
    }

    public override async Task<CreateCustomerReply> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
    {
        //Получаем предпочтения из бд и сохраняем большой объект
        var preferences = await _preferenceRepository
            .GetRangeByIdsAsync(request.PreferenceIds.Select(p => new Guid(p)).ToList());

        Customer customer = CustomerMapper.MapFromModel(new WebHost.Models.CreateOrEditCustomerRequest()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PreferenceIds = request.PreferenceIds.Select(p => new Guid(p)).ToList()
        }, preferences);

        await _customerRepository.AddAsync(customer);

        return new CreateCustomerReply()
        {
            Id = customer.Id.ToString(),

        };
    }

    public override async Task<Empty> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
    {
        var customer = await _customerRepository.GetByIdAsync(new System.Guid(request.Id));

        if (customer == null)
            throw new System.Exception("Customer not found");

        await _customerRepository.DeleteAsync(customer);

        return new Empty();
    }

    public override async Task<Empty> EditCustomer(EditCustomerRequest request, ServerCallContext context)
    {
        var customer = await _customerRepository.GetByIdAsync(new Guid(request.Id));

        if (customer == null)
            throw new Exception("Customer not found");

        var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds.Select(p => new Guid(p)).ToList());

        CustomerMapper.MapFromModel(new WebHost.Models.CreateOrEditCustomerRequest()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PreferenceIds = request.PreferenceIds.Select(p => new Guid(p)).ToList()
        }, preferences, customer);

        await _customerRepository.UpdateAsync(customer);

        return new Empty();
    }
}