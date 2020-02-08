REM MSSQL
REM xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\mssql.Dockerfile \\192.168.0.105\docker\projects\rfid\db /s /e /y
REM xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\mssql-docker-create-container.bat \\192.168.0.105\docker\projects\rfid\db /s /e /y
REM xcopy C:\Users\vasil\Documents\diplome\software\src\DB\script\db_create_script.sql \\192.168.0.105\docker\projects\rfid\db /s /e /y

REM REST
REM xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\rest-api.Dockerfile \\192.168.0.105\docker\projects\rfid\rest /s /e /y
REM xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\rest-api-docker-create-container.bat \\192.168.0.105\docker\projects\rfid\rest /s /e /y

REM CLIENT
xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\.dockerignore \\192.168.0.105\docker\projects\rfid\client /s /e /y
xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\angular.Dockerfile \\192.168.0.105\docker\projects\rfid\client /s /e /y
xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\angular-docker-create-container.bat \\192.168.0.105\docker\projects\rfid\client /s /e /y
xcopy C:\Users\vasil\Documents\diplome\software\src\Docker\nginx.conf \\192.168.0.105\docker\projects\rfid\client\ /s /e /y
xcopy C:\Users\vasil\Documents\diplome\software\src\Web\Client \\192.168.0.105\docker\projects\rfid\client\src /s /e /y /EXCLUDE:exclude.txt

REM REST PUBL
REM cd C:\Users\vasil\Documents\diplome\software\src\Web\REST\RFID.REST
REM dotnet publish -o \\192.168.0.105\docker\projects\rfid\rest\bin /p:EnvironmentName=Development

REM CLIENT PUBL
cd C:\Users\vasil\Documents\diplome\software\src\Web\Client
ng build --prod
xcopy dist\rfid \\192.168.0.105\docker\projects\rfid\client\bin /s /e /y

pause