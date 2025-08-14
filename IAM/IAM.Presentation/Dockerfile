# Build with NET 6 (IAM is a .NET 6 service)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY IAM/IAM.Presentation/IAM.Presentation.csproj IAM/IAM.Presentation/
COPY IAM/IAM.Application/IAM.Application.csproj IAM/IAM.Application/
COPY IAM/IAM.Infrastructure/IAM.Infrastructure.csproj IAM/IAM.Infrastructure/
COPY IAM/IAM.Domain/IAM.Domain.csproj IAM/IAM.Domain/
COPY IAM/IAM.Contracts/IAM.Contracts.csproj IAM/IAM.Contracts/
RUN dotnet restore IAM/IAM.Presentation/IAM.Presentation.csproj

COPY IAM/ IAM/
WORKDIR /src/IAM/IAM.Presentation
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IAM.Presentation.dll"]
