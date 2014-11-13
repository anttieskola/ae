# Personal site and playground

## WebUI - actual site
ASP.NET MVC 5 Site.

Web.config defines development database *"aetestdb"* you run locally. Define proper connection string for *"DbConnection"* during publishing or azure settings.

## News
News fetcher from YLE rss feeds. Provides repository for WebUI. News are saved in memory and repository contains maintenance method which should be called periodically to keep news up to date.

## Insomnia
Schedules a maintenance job which will use insomnia web service to get call back to WebUI web api function. That will call maintenance method in insomnia and new job will be scheduled. This keeps site awake always.

## Users
ASP.NET Identity implemented here. Nothing new to see.

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
