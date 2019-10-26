xcopy mssql.Dockerfile \\192.168.0.105\docker\projects\rfid\db /s /e /y
xcopy mssql-docker-create-container.bat \\192.168.0.105\docker\projects\rfid\db /s /e /y
xcopy rest-api.Dockerfile \\192.168.0.105\docker\projects\rfid\rest /s /e /y
xcopy rest-api-docker-create-container.bat \\192.168.0.105\docker\projects\rfid\rest /s /e /y
xcopy ..\DB\script\db_create_script.sql \\192.168.0.105\docker\projects\rfid\db /s /e /y

cd ..\Web\REST\RFID.REST
dotnet publish -o \\192.168.0.105\docker\projects\rfid\rest\bin /p:EnvironmentName=Development
pause