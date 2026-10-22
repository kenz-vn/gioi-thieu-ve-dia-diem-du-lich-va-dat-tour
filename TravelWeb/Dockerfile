# ==========================================
# DOCKERFILE CHO WEBSITE DU LICH (TRAVELWEB)
# ==========================================

# Giai doan 1: Bien dich ma nguon
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TravelWeb.csproj", "./"]
RUN dotnet restore "TravelWeb.csproj"
COPY . .
RUN dotnet publish "TravelWeb.csproj" -c Release -o /app/publish

# Giai doan 2: Moi truong runtime gon nhe
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5230
ENV ASPNETCORE_URLS=http://+:5230
ENTRYPOINT ["dotnet", "TravelWeb.dll"]
