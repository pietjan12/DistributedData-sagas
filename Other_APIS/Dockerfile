FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY Customer_API/*.csproj .
RUN dotnet restore --verbosity n

# Copy everything else and build
COPY Customer_API/. .
RUN dotnet build -c Release -o out

from build AS publish
RUN dotnet publish -c Release -o out

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "Customer_API.dll"]