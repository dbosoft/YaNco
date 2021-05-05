#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["samples/netcore2.1/SAPWebAPI/SAPWebAPI.csproj", "samples/netcore2.1/SAPWebAPI/"]
RUN dotnet restore "samples/netcore2.1/SAPWebAPI/SAPWebAPI.csproj"
COPY . .
WORKDIR "/src/samples/netcore2.1/SAPWebAPI"
RUN dotnet build "SAPWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAPWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAPWebAPI.dll"]