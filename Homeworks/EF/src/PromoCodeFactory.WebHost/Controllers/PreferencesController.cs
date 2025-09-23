using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IRepository<Preference> _preferenceRepository;

        public PreferencesController(IRepository<Preference> preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }
        /// <summary>
        /// Получить данные всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceShortResponse>>> GetPreferencesAsync()
        {
            try
            {
                var preferences = await _preferenceRepository.GetAllAsync();
                var preferencesModelList = preferences.Select(x => new PreferenceShortResponse()
                {
                    Id = x.Id,
                    Name = x.Name
                });
                return Ok(preferencesModelList);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        /// <summary>
        /// Получить данные предпочтения по ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PreferenceResponse>> GetPreferenceByIdAsync(Guid id)
        {
            try
            {
                var preference = await _preferenceRepository.GetByIdAsync(id);
                var preferenceResponse = new PreferenceResponse()
                {
                    Id = preference.Id,
                    Name = preference.Name,
                    Customers = preference.Customers.Select(x => new CustomerShortResponse
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email
                    }).ToList()
                };
                return Ok(preferenceResponse);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}