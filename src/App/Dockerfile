FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ./App/App.csproj
# Build and publish a release
RUN dotnet publish -o out ./App/App.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "App.dll"]
