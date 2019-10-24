FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/FollowUP.Api/FollowUP.Api.csproj", "src/FollowUP.Api/"]
COPY ["src/FollowUP.Infrastructure/FollowUP.Infrastructure.csproj", "src/FollowUP.Infrastructure/"]
COPY ["src/FollowUP.Core/FollowUP.Core.csproj", "src/FollowUP.Core/"]
RUN dotnet restore "src/FollowUP.Api/FollowUP.Api.csproj"
COPY . .
WORKDIR "/src/src/FollowUP.Api"
RUN dotnet build "FollowUP.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FollowUP.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FollowUP.Api.dll"]