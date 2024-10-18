# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/Backend/PaymentGateway.PaymentApi/PaymentGateway.PaymentApi.csproj", "Src/Backend/PaymentGateway.PaymentApi/"]
COPY ["Src/Backend/PaymentGateway.Application/PaymentGateway.Application.csproj", "Src/Backend/PaymentGateway.Application/"]
COPY ["BuildingBlocks/Common.Helper/Common.Helper.csproj", "BuildingBlocks/Common.Helper/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
COPY ["Src/Backend/PaymentGateway.Domain/PaymentGateway.Domain.csproj", "Src/Backend/PaymentGateway.Domain/"]
COPY ["Src/Backend/PaymentGateway.Infrastructure/PaymentGateway.Infrastructure.csproj", "Src/Backend/PaymentGateway.Infrastructure/"]
RUN dotnet restore "./Src/Backend/PaymentGateway.PaymentApi/PaymentGateway.PaymentApi.csproj"
COPY . .
WORKDIR "/src/Src/Backend/PaymentGateway.PaymentApi"
RUN dotnet build "./PaymentGateway.PaymentApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PaymentGateway.PaymentApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGateway.PaymentApi.dll"]