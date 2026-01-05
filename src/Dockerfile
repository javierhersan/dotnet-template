# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy project files
COPY API/API.csproj src/API/
COPY Application/Application.csproj src/Application/
COPY Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY Domain/Domain.csproj src/Domain/

# Restore dependencies for API
RUN dotnet restore src/API/API.csproj

# Copy the rest of the source
COPY . ./src

# Publish API
RUN dotnet publish src/API/API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Generate HTTPS dev certificate **inside build stage**
RUN mkdir /https \
    && dotnet dev-certs https -ep /https/cert.pfx -p PASSWORD

# RUN dotnet dev-certs https --trust

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=https://+;http://+
ENV ASPNETCORE_ENVIRONMENT=Local
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=PASSWORD

COPY --from=build /app/publish .
COPY --from=build /https/cert.pfx /https/cert.pfx

EXPOSE 443
EXPOSE 80

ENTRYPOINT ["dotnet", "API.dll"]
