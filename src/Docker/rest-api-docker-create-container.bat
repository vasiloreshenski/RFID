docker stop rfid-rest
docker rm rfid-rest
docker image rm rfid-rest

docker build -f rest-api.Dockerfile -t rfid-rest .
docker run -d -p 8080:80 --net rfid --ip 172.18.0.2 --name rfid-rest rfid-rest