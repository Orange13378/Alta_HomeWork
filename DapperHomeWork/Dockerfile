FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DapperHomeWork/DapperHomeWork.csproj", "DapperHomeWork/"]
RUN dotnet restore "DapperHomeWork/DapperHomeWork.csproj"
COPY . .
WORKDIR "/src/DapperHomeWork"
RUN dotnet build "DapperHomeWork.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DapperHomeWork.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DapperHomeWork.dll"]