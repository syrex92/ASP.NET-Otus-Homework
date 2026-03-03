using System;
using System.Linq;
using System.Threading.Tasks;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.WebHost.Interfaces;

namespace Pcf.Administration.WebHost.Services;

public class PromocodeService: IPromocodeService
{
    private readonly IRepository<Employee> _employeeRepository;

    public PromocodeService(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    public async Task UpdateAppliedPromocodesAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            throw new Exception($"Employee with id {id} not found");

        employee.AppliedPromocodesCount++;

        await _employeeRepository.UpdateAsync(employee);
    }
}