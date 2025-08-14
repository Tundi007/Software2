# Build with NET 6 (Media service)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY Media/Media/Media.Presentation.csproj Media/Media/
COPY Media/Media.Application/Media.Application.csproj Media/Media.Application/
COPY Media/Media.Infrastructure/Media.Infrastructure.csproj Media/Media.Infrastructure/
COPY Media/Media.Domain/Media.Domain.csproj Media/Media.Domain/
RUN dotnet restore Media/Media/Media.Presentation.csproj

COPY Media/ Media/
WORKDIR /src/Media/Media
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Media.Presentation.dll"]
