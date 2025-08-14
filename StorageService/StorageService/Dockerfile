# Build with .NET 6 SDK
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy csproj files first for caching
COPY StorageService/StorageService/StorageService.Presentation.csproj StorageService/StorageService/
COPY StorageService/StorageService.Application/StorageService.Application.csproj StorageService/StorageService.Application/
COPY StorageService/StorageService.Infrastructure/StorageService.Infrastructure.csproj StorageService/StorageService.Infrastructure/
COPY StorageService/StorageService.Domain/StorageService.Domain.csproj StorageService/StorageService.Domain/
COPY StorageService/StorageService.Contracts/StorageService.Contracts.csproj StorageService/StorageService.Contracts/

RUN dotnet restore StorageService/StorageService/StorageService.Presentation.csproj

# copy the rest and publish
COPY StorageService/ StorageService/
WORKDIR /src/StorageService/StorageService
RUN dotnet publish -c Release -o /app/publish

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StorageService.Presentation.dll"]
