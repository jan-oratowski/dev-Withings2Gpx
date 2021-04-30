using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sportsy.Data.Models;

namespace Sportsy.Data.Database
{
    public static class SportsyInit
    {
        public static void Initialize(SportsyContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User{Name="jasio"},
            };
            foreach (var u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();
        }
    }
}
