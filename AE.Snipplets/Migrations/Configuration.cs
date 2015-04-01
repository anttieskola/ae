namespace AE.Snipplets.Migrations
{
    using Dal;
    using Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

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

    internal sealed class Configuration : DbMigrationsConfiguration<SnippletContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SnippletContext db)
        {
            // C# tag
            Tag csharp;
            if (db.Tags.Any(t => t.Name == "C#"))
            {
                csharp = db.Tags.First(t => t.Name == "C#");
            }
            else
            {
                csharp = new Tag { Name = "C#" };
                csharp = db.Tags.Add(csharp);
                db.SaveChanges();
            }

            // hello world if no other snipplet
            if (db.Snipplets.Count() == 0)
            {
                Snipplet hw = new Snipplet
                {
                    Headline = "Hello world!",
                    Content = "static void Main(string[] args)\n{\n\tConsole.WriteLine(\"Hello world!\");\n}"
                };
                hw.Tags.Add(csharp);
                hw = db.Snipplets.Add(hw);
                db.SaveChanges();
            }
        }
    }
}
