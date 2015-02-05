using AE.Mpg.Entity;
using AE.Users.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Mpg.Dal
{
    internal class MpgContext : DbContext
    {
        public MpgContext()
            : base("DbConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.HasDefaultSchema("mpg");
            //mb.Entity<AspNetUser>().HasKey<string>(i => i.Id); // tell about user id key
            //mb.Entity<AspNetUser>().ToTable("AspNetUsers", "dbo"); // set the table name to correspond real name (made by AE.User)
            //mb.Ignore<IdentityUserLogin>(); // ignore, to prevent creation
            //mb.Ignore<IdentityUserRole>(); // ignore, to prevent creation
            //mb.Ignore<IdentityUserClaim>(); // ignore, to prevent creation
            // still i commented out the AspNetUsers table from initial script as AE.Users generates it.
            // did not find out better solution to this.

            // problem: We have two contexes where we have relationships between them but generate only
            // one at a time with EF.
            base.OnModelCreating(mb);
        }

        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<Vehicle> Cars { get; set; }
        public DbSet<Fill> Fills { get; set; }
    }
}
