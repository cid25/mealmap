# Setting up for development

## Integration Tests

The test suite contains a set of integration tests, which are tests run against external dependencies and therefore slower in their execution.
To execute them one requires an database on an instance of Microsoft SQL (SQL Server, Azure SQL), which is what the application targets as production database.

The easiest way to provide that is running a container with the Developer Edition of MSSQL on the local machine.

`docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MealIntTest!" -p 1433:1433 --name mealmapsql --hostname mealmapsql -d mcr.microsoft.com/mssql/server:2022-latest`

The configuration in `appsettings.Development.json` of `Mealmap.API` match the credentials given in the command, so adapt that if your database requires different ones.

## Launching for Local Testing

While running a MSSQL database (see Integration Tests above) and having it configured for the API, do the following to launch the application for manual local testing:

- Use Visual Studio to launch the .NET Core API using the `api_only` profile
- Use a terminal and run `ng serve --ssl` to serve the Angular application via the dev proxy
- Use Visual Studio Code and one of the given profiles to launch the browser of your choice in debug mode,
  which will automatically navigate to the URL of the Angular app
