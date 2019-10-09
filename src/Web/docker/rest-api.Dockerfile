FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY ./bin .
ENTRYPOINT ["dotnet", "RFID.REST.dll", "--environment=Development"]