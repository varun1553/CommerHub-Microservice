# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Update this line to handle .slnx
COPY CommerceHub.slnx ./

# Copy project files (this stays the same)
COPY CommerceHub.API/*.csproj ./CommerceHub.API/
COPY CommerceHub.Application/*.csproj ./CommerceHub.Application/
COPY CommerceHub.Domain/*.csproj ./CommerceHub.Domain/
COPY CommerceHub.Infrastructure/*.csproj ./CommerceHub.Infrastructure/

# Restore using the .slnx file
RUN dotnet restore "CommerceHub.API/CommerceHub.API.csproj"

# Copy everything else
COPY . .

# Publish the API
RUN dotnet publish "CommerceHub.API/CommerceHub.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CommerceHub.API.dll"]