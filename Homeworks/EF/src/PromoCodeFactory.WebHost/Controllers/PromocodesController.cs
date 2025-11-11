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
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promocodesRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        public PromocodesController(IRepository<PromoCode> promocodesRepository, IRepository<Preference> preferenceRepository)
        {
            _promocodesRepository = promocodesRepository;
            _preferenceRepository = preferenceRepository;
        }
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            try
            {
                var promocodes = await _promocodesRepository.GetAllAsync();
                var promocodesModelList = promocodes.Select(x => new PromoCodeShortResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    ServiceInfo = x.ServiceInfo,
                    BeginDate = x.BeginDate.ToString(),
                    EndDate = x.EndDate.ToString(),
                    PartnerName = x.PartnerName
                });
                return Ok(promocodesModelList);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            try
            {
                var preference = await _preferenceRepository.GetByIdAsync(new Guid(request.Preference));
                if (preference == null) throw new Exception("Preference is not found");

                foreach (var customer in preference.Customers)
                {
                    var promocode = new PromoCode
                    {
                        Id = Guid.NewGuid(),
                        BeginDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(1),
                        Code = request.PromoCode,
                        PartnerName = request.PartnerName,
                        Preference = preference,
                        Customer = customer,
                        ServiceInfo = request.ServiceInfo
                    };
                    await _promocodesRepository.AddAsync(promocode);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}