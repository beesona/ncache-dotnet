FROM  mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /app .

# Overwrite the dev appsettings with a deployable settings file.
COPY ./appsettings.Deploy.json ./appsettings.json

EXPOSE 80 443

ENTRYPOINT [ "dotnet", "ncache-dotnet.dll" ]