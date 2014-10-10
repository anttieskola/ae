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

    internal sealed class Configuration : DbMigrationsConfiguration<UserContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UserContext db)
        {
            var users = new UserManager<AspNetUser>(new UserStore<AspNetUser>(db));
            var roles = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!roles.RoleExists("admin"))
            {
                roles.Create(new IdentityRole("admin"));
            }
            if (!roles.RoleExists("member"))
            {
                roles.Create(new IdentityRole("member"));
            }

            AspNetUser admin;
            if (!db.Users.Any(u => u.UserName == "admin@admin.admin"))
            {
                admin = new AspNetUser { UserName = "admin@admin.admin" };
                users.Create(admin, "123456");
                users.AddToRole(admin.Id, "admin");
            }
            else
            {
                admin = db.Users.First(u => u.UserName == "admin@admin.admin");
            }

            AspNetUser member;
            if (!db.Users.Any(u => u.UserName == "member@member.member"))
            {
                member = new AspNetUser { UserName = "member@member.member" };
                users.Create(member, "123456");
                users.AddToRole(member.Id, "member");
            }
            else
            {
                member = db.Users.First(u => u.UserName == "member@member.member");
            }

            db.SaveChanges();
        }
    }
}
