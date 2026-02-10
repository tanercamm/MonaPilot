FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MonaPilot.API/MonaPilot.API.csproj", "MonaPilot.API/"]
RUN dotnet restore "MonaPilot.API/MonaPilot.API.csproj"

COPY . .
WORKDIR "/src/MonaPilot.API"
RUN dotnet build "MonaPilot.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonaPilot.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5001
ENV ASPNETCORE_URLS=https://+:5001
ENTRYPOINT ["dotnet", "MonaPilot.API.dll"]
