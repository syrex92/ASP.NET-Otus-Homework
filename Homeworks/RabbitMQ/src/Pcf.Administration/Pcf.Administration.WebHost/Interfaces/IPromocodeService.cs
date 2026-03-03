using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Interfaces
{
    public interface IPromocodeService
    {
        Task UpdateAppliedPromocodesAsync(Guid value);
    }
}