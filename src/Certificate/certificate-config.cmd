cd ..
cd Web
cd REST
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\RFID.REST.pfx -p 123
dotnet dev-certs https --trust
dotnet user-secrets -p RFID.REST\RFID.REST.csproj set "Kestrel:Certificates:Development:Password" "123"

pause