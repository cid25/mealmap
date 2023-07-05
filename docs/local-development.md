# Setting up for development

## Integration Tests

The test suite contains a set of integration tests, which are tests run against external dependencies and therefore slower in their execution.
To execute them one requires an database on an instance of Microsoft SQL (SQL Server, Azure SQL), which is what the application targets as production database.

The easiest way to provide that is running a container with the Developer Edition of MSSQL on the local machine.

`docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MealIntTest!" -p 1433:1433 --name mealmapsql --hostname mealmapsql -d mcr.microsoft.com/mssql/server:2022-latest`

The configuration in `appsettings.Development.json` of `Mealmap.API` match the credentials given in the command, so adapt that if your database requires different ones.
