docker stop rfid-mssql
docker rm rfid-mssql
docker image rm rfid-mssql

docker build -f mssql.Dockerfile -t rfid-mssql .
docker run -d -p 1433:1433 --name rfid-mssql rfid-mssql