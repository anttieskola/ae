namespace AE.Funny.Migrations
{
    using System.Data.Entity.Migrations;

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

    internal sealed class Configuration : DbMigrationsConfiguration<AE.Funny.Dal.FunnyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Dal.FunnyContext context)
        {
            // nothing to seed
        }
    }
}
