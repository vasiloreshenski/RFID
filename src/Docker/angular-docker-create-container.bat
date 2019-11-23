docker stop rfid-client
docker rm rfid-client
docker image rm rfid-client

docker build -f angular.Dockerfile -t rfid-client .
docker run --rm -d -p 90:80/tcp --net rfid --ip 172.18.0.10 --name rfid-client rfid-client