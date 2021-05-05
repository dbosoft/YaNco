#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/netcore3.1/SAPWebAPI/SAPWebAPI.csproj", "samples/netcore3.1/SAPWebAPI/"]
COPY ["samples/WebApi.Shared/WebApi.Shared.csproj", "samples/WebApi.Shared/"]
COPY ["src/YaNco/YaNco.csproj", "src/YaNco/"]
COPY ["src/YaNco.Core/YaNco.Core.csproj", "src/YaNco.Core/"]
COPY ["src/YaNco.Abstractions/YaNco.Abstractions.csproj", "src/YaNco.Abstractions/"]
COPY ["src/YaNco.Primitives/YaNco.Primitives.csproj", "src/YaNco.Primitives/"]
RUN dotnet restore "samples/netcore3.1/SAPWebAPI/SAPWebAPI.csproj"
COPY . .
WORKDIR "/src/samples/netcore3.1/SAPWebAPI"
RUN dotnet build "SAPWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAPWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAPWebAPI.dll"]