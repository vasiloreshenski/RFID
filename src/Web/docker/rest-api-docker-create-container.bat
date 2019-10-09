docker stop rfid-rest
docker rm rfid-test
docker image rm rfid-test

docker build -f rest-api.Dockerfile -t rfid-rest .
docker run -d -p 8080:80 --name rfid-rest rfid-rest