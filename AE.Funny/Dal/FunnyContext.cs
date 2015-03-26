using AE.Funny.Entity;
using System.Data.Entity;

namespace AE.Funny.Dal
{
    internal class FunnyContext : DbContext
    {
        internal FunnyContext()
            : base("DbConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Todo: placeholder
            base.OnModelCreating(modelBuilder);
        }

        // "tables"
        internal DbSet<FunnyPost> FunnyPosts { get; set; }
    }
}
