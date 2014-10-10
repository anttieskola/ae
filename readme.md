# Personal site and playground

## WebUI - actual site
ASP.NET MVC 5 Site.

Web.config defines development database *"aetestdb"* you run locally. Define proper connection string for *"DbConnection"* during publishing or azure settings.

## Users
ASP.NET Identity implemented here. Nothing new to see.

## News
News fetcher from YLE rss feeds. Currently fetches news every few minutes into memory "db" and provides them to UI. Will rework with this as it will lose data and scheduled update jobs when Appdomain shuts down. Should work if make other site "ping" deployed one.

## Mpg
Miles per gallon placeholder. Have an implementation of one kind but not migrated here yet.

## Test
Testproject for cases in any of the other projects.
Remember to add:

`[assembly: InternalsVisibleTo("AE.Test")]`

Into other project's Properties -> AssemblyInfo.cs to give access to internals.

### Notes
#### Database migrations
1. Enable-Migrations -ProjectName pname
2. Add-Migration -ProjectName pname migration-name
3. Check now the configuration in app.config / web.config which project in use that what server it configures before running updates! example:
`Update-Database -ConnectionString "Data Source=localhost;Initial Catalog=aetestdb;Integrated Security=false;User Id=aetestdb;Password=aetestdb;MultipleActiveResultSets=True;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName AE.Users -Verbose`
