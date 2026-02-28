FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NotesApp.Api.csproj", "."]
RUN dotnet restore "./NotesApp.Api.csproj"
COPY . .
RUN dotnet build "NotesApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotesApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotesApp.Api.dll"]
