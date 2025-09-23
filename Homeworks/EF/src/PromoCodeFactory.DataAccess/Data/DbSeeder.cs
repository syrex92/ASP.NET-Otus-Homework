using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class DbSeeder
    {
        public static void Seed(DataContext context)
        {
            var roles = FakeDataFactory.Roles.ToList();
            context.Roles.AddRange(roles);
            context.SaveChanges();

            var employees = FakeDataFactory.Employees.ToList();
            foreach (var employee in employees)
            {
                employee.Role = roles.First(r => r.Name == employee.Role.Name);
            }
            context.Employees.AddRange(employees);

            context.Preferences.AddRange(FakeDataFactory.Preferences);

            context.Customers.AddRange(FakeDataFactory.Customers);

            context.SaveChanges();
        }        
    }
}