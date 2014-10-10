using AE.Users.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Users.Dal
{
    /// <summary>
    /// my websites user db context
    /// </summary>
    public class UserContext : IdentityDbContext<AspNetUser>
    {
        public UserContext()
            : base("DbConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public static UserContext Create()
        {
            return new UserContext();
        }
    }
}
