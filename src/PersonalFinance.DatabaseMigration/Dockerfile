# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/PersonalFinance.DatabaseMigration/PersonalFinance.DatabaseMigration.csproj", "PersonalFinance.DatabaseMigration/"]

# Restore dependencies
RUN dotnet restore "PersonalFinance.DatabaseMigration/PersonalFinance.DatabaseMigration.csproj"

# Copy source code
COPY ["src/PersonalFinance.DatabaseMigration/", "PersonalFinance.DatabaseMigration/"]

# Build the project
WORKDIR "/src/PersonalFinance.DatabaseMigration"
RUN dotnet build "PersonalFinance.DatabaseMigration.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PersonalFinance.DatabaseMigration.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "PersonalFinance.DatabaseMigration.dll"]
