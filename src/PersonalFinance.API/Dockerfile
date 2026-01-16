FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/PersonalFinance.API/PersonalFinance.API.csproj", "src/PersonalFinance.API/"]
COPY ["src/PersonalFinance.Application/PersonalFinance.Application.csproj", "src/PersonalFinance.Application/"]
COPY ["src/PersonalFinance.Domain/PersonalFinance.Domain.csproj", "src/PersonalFinance.Domain/"]
COPY ["src/PersonalFinance.Infrastructure/PersonalFinance.Infrastructure.csproj", "src/PersonalFinance.Infrastructure/"]
RUN dotnet restore "src/PersonalFinance.API/PersonalFinance.API.csproj"
COPY . .
WORKDIR "/src/src/PersonalFinance.API"
RUN dotnet build "PersonalFinance.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PersonalFinance.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonalFinance.API.dll"]
