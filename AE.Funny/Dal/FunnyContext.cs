using AE.Funny.Entity;
using System.Data.Entity;

namespace AE.Funny.Dal
{
    public class FunnyContext : DbContext
    {
        public FunnyContext()
            : base("DbConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("funny");
            modelBuilder.Entity<Post>()
                .HasMany<Comment>(p => p.Comments)
                .WithRequired(c => c.Post)
                .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);
        }

        // "tables"
        internal DbSet<Post> Posts { get; set; }
        internal DbSet<Maintenance> Maintenances { get; set; }
    }
}
