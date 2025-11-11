using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IRepository<PromoCode> _promocodeRepository;
        public CustomersController(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository, IRepository<PromoCode> promocodeRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _promocodeRepository = promocodeRepository;
        }
        /// <summary>
        /// Получить данные всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var customersModelList = customers.Select(x => new CustomerShortResponse()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                });
                return Ok(customersModelList);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        /// <summary>
        /// Получить данные клиента по ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                var customerResponse = new CustomerResponse()
                {
                    Id = customer.Id,
                    Email = customer.Email,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Preferences = customer.Preferences.Select(x => new PreferenceShortResponse
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList(),
                    PromoCodes = customer.PromoCodes.Select(x => new PromoCodeShortResponse
                    {
                        Id = x.Id,
                        Code = x.Code,
                        ServiceInfo = x.ServiceInfo,
                        BeginDate = x.BeginDate.ToShortDateString(),
                        EndDate = x.EndDate.ToShortDateString(),
                        PartnerName = x.PartnerName
                    }).ToList()
                };
                return Ok(customerResponse);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        /// <summary>
        /// Создать клиента
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            try
            {
                var preferences = await _preferenceRepository.GetListByIdAsync(request.PreferenceIds.ToArray());
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Preferences = preferences.ToList()
                };
                await _customerRepository.AddAsync(customer);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        /// <summary>
        /// Обновить данные клиента
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            try
            {
                var preferences = await _preferenceRepository.GetListByIdAsync(request.PreferenceIds.ToArray());
                var customer = new Customer
                {
                    Id = id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Preferences = preferences.ToList()
                };
                await _customerRepository.UpdateAsync(customer);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        /// <summary>
        /// Удалить данные клиента
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                foreach (var promocode in customer.PromoCodes)
                {
                    await _promocodeRepository.DeleteAsync(promocode.Id);
                }
                await _customerRepository.DeleteAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}