using AE.Users;
using AE.Users.Dal;
using AE.Users.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace AE.Users.Migrations
{
    /// <summary>
    /// helper to run code migrations from code
    /// </summary>
    public sealed class Run
    {
        public static void Migration()
        {
            var c = new Configuration();
            var m = new DbMigrator(c);
            m.Update();
        }
    }
        internal sealed class Configuration : DbMigrationsConfiguration<UserContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UserContext db)
        {
            // member role
            var roles = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (!roles.RoleExists("member"))
            {
                roles.Create(new IdentityRole("member"));
            }

            // first user
            if (!db.Users.Any(u => u.UserName == "antti"))
            {
                var users = new UserManager<AspNetUser>(new UserStore<AspNetUser>(db));
                var antti = new AspNetUser { UserName = "antti", Email = "antti.eskola@gmail.com", EmailConfirmed = true };
                users.Create(antti, "vaihdaTämä!");
                users.AddToRole(antti.Id, "member");
            }
            db.SaveChanges();
        }
    }
}
