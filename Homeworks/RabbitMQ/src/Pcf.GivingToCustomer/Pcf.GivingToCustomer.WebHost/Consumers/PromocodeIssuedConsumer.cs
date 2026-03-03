using System.Threading.Tasks;
using MassTransit;
using Pcf.Messages;
using Pcf.GivingToCustomer.WebHost.Interfaces;
using Pcf.GivingToCustomer.WebHost.Models;
using System;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromocodeIssuedConsumer : IConsumer<PromocodeIssued>
    {
        private readonly IPromocodeService _promocodeService;
        public PromocodeIssuedConsumer(IPromocodeService promocodeService)
        {
            _promocodeService = promocodeService;
        }
        public async Task Consume(ConsumeContext<PromocodeIssued> context)
        {
            Console.WriteLine($"Received PromocodeIssued event for PartnerId: {context.Message.PartnerId}");
            GivePromoCodeRequest request = new GivePromoCodeRequest()
            {
                PartnerId = context.Message.PartnerId,
                BeginDate = context.Message.BeginDate,
                EndDate = context.Message.EndDate,
                PreferenceId = context.Message.PreferenceId,
                PromoCode = context.Message.PromoCode,
                ServiceInfo = context.Message.ServiceInfo,
            };
            await _promocodeService.GivePromoCodesToCustomersWithPreferenceAsync(request);
        }
    }
}