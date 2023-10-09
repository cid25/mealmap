FROM node:current AS build-angular
WORKDIR /src
# Copy Angular code
COPY /Mealmap.Web .
# Build Angular app
RUN npm install
RUN npm run build -- --configuration production --output-path ../publish


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-dotnet
WORKDIR /src
# Copy C# code
COPY /Mealmap.Api Mealmap.Api/
COPY /Mealmap.Domain Mealmap.Domain/
COPY /Mealmap.Infrastructure Mealmap.Infrastructure/
# Restore as distinct layers
RUN dotnet restore Mealmap.Api/Mealmap.Api.csproj
# Build and publish a release
RUN dotnet publish Mealmap.Api/Mealmap.Api.csproj -c Release -o /publish


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-dotnet /publish .
COPY --from=build-angular /publish wwwroot/
ENTRYPOINT ["dotnet", "Mealmap.Api.dll"]
