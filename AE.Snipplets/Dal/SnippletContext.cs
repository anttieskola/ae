using AE.Snipplets.Entity;
using System.Data.Entity;

namespace AE.Snipplets.Dal
{
    public class SnippletContext : DbContext
    {
        public SnippletContext()
            : base("DbConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("snipplet");
            // There is a many to many relationship between snipplet and tag so we need to create link table for this
            modelBuilder.Entity<Snipplet>().HasMany<Tag>(s => s.Tags).WithMany(t => t.Snipplets).Map(x => x.MapLeftKey("SnippletId").MapRightKey("TagId").ToTable("SnippletTag"));
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Snipplet> Snipplets  { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
