FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Books.API/Books.API.csproj", "Books.API/"]
RUN dotnet restore "Books.API/Books.API.csproj"
COPY . .
WORKDIR "/src/Books.API"
RUN dotnet build "Books.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Books.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Books.API.dll"]
