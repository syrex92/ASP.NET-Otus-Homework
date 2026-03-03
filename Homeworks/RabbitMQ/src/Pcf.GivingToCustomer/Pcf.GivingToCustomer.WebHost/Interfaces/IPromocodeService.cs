using System;
using System.Threading.Tasks;
using Pcf.Messages;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Interfaces;

public interface IPromocodeService
{
    Task GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request);
}
