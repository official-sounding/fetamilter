# Stage 1: Compile TypeScript
FROM node:26-alpine AS ts-build
WORKDIR /typescript
COPY /App/typescript/package*.json ./
RUN npm ci
COPY /App/typescript/tsconfig.json ./
COPY /App/typescript/src ./src
RUN npx tsc

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy everything
COPY . ./
COPY --from=ts-build /typescript/src ./App/wwwroot/js
# Restore as distinct layers
RUN dotnet restore ./App/App.csproj
# Build and publish a release
RUN dotnet publish --no-restore -o out ./App/App.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "App.dll"]
