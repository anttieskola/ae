# Personal site and playground

## WebUI - actual site
ASP.NET MVC 5 Site.
* Web Api
* View Controllers
* Views
...

Web.config defines development database *"antti"* you run locally. Define proper connection string for *"DbConnection"* during publishing or azure settings.

## Insomnia
Schedules a maintenance job (Quartz library) which will use insomnia web service to get call back to web api function. That will call maintenance method in insomnia and new job will be scheduled. This keeps site awake always except when Azure shuts down free sites once a day.

## News
News fetcher and repository. This during maintenance job will fetch news from YLE RSS feeds. They will be parsed and processed for storage into DB. Currently it will keep them there until they one month old. News are provided for the front-end by web api found in WebUI.

## Funny
Funny picture fetcher and repository. Same as news during specific times via maintenance job this will fetch hottest funny posts from Reddit with few comments. Then it will try to get direct links to pictures from Imgur. That data we succeed to gather is stored into DB. DB will hold currently around hundred of those. End result in front-end should be quick and easy browsing of those, hence why I am doing this. Sorry for both services for bypassing adds.

## Mpg
Fuel consumption for cars. Currently just for my car :) Repository storing vehicles, fuel types and fill ups in database. Provided for the front-end via web api and repository use in controllers. Front-end uses highcharts.js for fancy graphics. Management of all is done via views that require authenticated user so not visible to public.

## Users
ASP.NET Identity implemented here. Nothing special.

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
`Update-Database -ConnectionString "Data Source=localhost;Initial Catalog=antti;Integrated Security=false;User Id=antti;Password=todo;MultipleActiveResultSets=True;" -ConnectionProviderName "System.Data.SqlClient" -ProjectName AE.Users -Verbose`
