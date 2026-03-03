using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Pcf.Administration.WebHost.Interfaces;
using Pcf.Administration.WebHost.Services;
using Pcf.Messages;
namespace Pcf.Administration.WebHost.Consumers
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
            Console.WriteLine($"Received PromocodeIssued event for PartnerManagerId: {context.Message.PartnerManagerId}");
            if (!context.Message.PartnerManagerId.HasValue)
            {
                return;
            }

            await _promocodeService.UpdateAppliedPromocodesAsync(context.Message.PartnerManagerId.Value);
        }
    }
}