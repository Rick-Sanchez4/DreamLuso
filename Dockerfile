# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["DreamLuso.sln", "./"]
COPY ["DreamLuso.WebAPI/DreamLuso.WebAPI.csproj", "DreamLuso.WebAPI/"]
COPY ["DreamLuso.Application/DreamLuso.Application.csproj", "DreamLuso.Application/"]
COPY ["DreamLuso.Data/DreamLuso.Data.csproj", "DreamLuso.Data/"]
COPY ["DreamLuso.Domain/DreamLuso.Domain.csproj", "DreamLuso.Domain/"]
COPY ["DreamLuso.Domain.Core/DreamLuso.Domain.Core.csproj", "DreamLuso.Domain.Core/"]
COPY ["DreamLuso.Security/DreamLuso.Security.csproj", "DreamLuso.Security/"]
COPY ["DreamLuso.IoC/DreamLuso.IoC.csproj", "DreamLuso.IoC/"]

# Restore dependencies
RUN dotnet restore "DreamLuso.sln"

# Copy all source files
COPY . .

# Build and publish
WORKDIR "/src/DreamLuso.WebAPI"
RUN dotnet build "DreamLuso.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "DreamLuso.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=build /app/publish .

# Create directory for uploads
RUN mkdir -p wwwroot/images/properties

# Expose port (Render usa 8080 por padrão)
EXPOSE 8080

# Set environment variable for port
ENV ASPNETCORE_URLS=http://+:8080

# Set entry point
ENTRYPOINT ["dotnet", "DreamLuso.WebAPI.dll"]

