FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY bin/Release/netcoreapp3.1/publish app/

EXPOSE 80

ENTRYPOINT [ "dotnet", "app/ncache-dotnet.dll" ]