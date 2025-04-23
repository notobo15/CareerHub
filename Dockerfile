# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy project files
COPY RecruitmentApp.sln ./
COPY RecruitmentApp.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./
RUN dotnet publish -c Release -o /out

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "RecruitmentApp.dll"]
