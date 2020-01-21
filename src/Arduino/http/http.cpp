#include <Arduino.h>
#include <http.h>
#include <SoftwareSerial.h>

const String OK = "OK";
const String ERR = "ERR";
const String CLOSED = "CLOSED";
const String API_URL = "192.168.0.105";
const String API_PORT = "443";
// const String API_URL = "desktop-sb3j0h0";
// const String API_PORT = "8080";

bool Http::init()
{
	wifiSerial.begin(9600);

	bool connected = setActiveMode().success;
	// bool connected = setStaticMode().success && connectToWifi().success;
	return connected;
}

HttpResponse Http::checkAccess(String tagNumber, String accessPointSerialNumber)
{
	HttpResponse result = openConnection();
	String path = "/accesscontrol/api/tags/checkaccess?TagNumber=" + tagNumber + "&AccessPointSerialNumber=" + accessPointSerialNumber;
	String command = createGetCommandFromPath(path);
	if (result.success)
	{
		result = sendSize(command);
	}

	if (result.success)
	{
		result = processAtCommand(command, &authorized, &unAuthorized);
	}

	closeConnection();

	return result;
}

Http::Http(int txPin, int rxPin)
	: wifiSerial(txPin, rxPin)
{
}

HttpResponse Http::setStaticMode()
{
	return processAtCommand("AT+CWMODE=1", &endsWithOk, &endsWithErrorOrFail);
}

HttpResponse Http::setActiveMode()
{
	HttpResponse response = processAtCommand("AT+CWMODE=2", &endsWithOk, &endsWithErrorOrFail);
	response = processAtCommand("AT+CWSAP=\"arduino-master\",\"1234test\",6,3", &endsWithOk, &endsWithErrorOrFail);
	response = processAtCommand("AT+CIPAP=\"192.168.0.100\"", &endsWithOk, &endsWithErrorOrFail);

	return response;
}

HttpResponse Http::connectToWifi()
{
	return processAtCommand("AT+CWJAP=\"dlink\",\"9006241948\"", &endsWithOk, &endsWithErrorOrFail);
}

HttpResponse Http::openConnection()
{
	// return processAtCommand("AT+CIPSTART=\"TCP\",\"" + API_URL + "\"," + API_PORT, &endsWithOk, &endsWithErrorOrFail);
  	return processAtCommand("AT+CIPSTART=\"SSL\",\"" + API_URL + "\"," + API_PORT, &endsWithOk, &endsWithErrorOrFail);
}

HttpResponse Http::closeConnection()
{
	return processAtCommand("AT+CIPCLOSE", &endsWithOk, &endsWithErrorOrFail);
}

HttpResponse Http::sendSize(String command)
{
	int len = calculateRequestLen(command);
	return processAtCommand("AT+CIPSEND=" + String(len), &endsWithOk, &endsWithErrorOrFail);
}

static bool Http::endsWithOk(String text)
{
	return text.endsWith("OK");
}

static bool Http::endsWithErrorOrFail(String text)
{
	return text.endsWith("ERROR") || text.endsWith("ERR") || text.endsWith("FAIL");
}

static bool Http::authorized(String text)
{
	return text.indexOf("+IPD,") >= 0 && text.indexOf("200 OK") >= 0;
}

static bool Http::unAuthorized(String text)
{
	return text.indexOf("+IPD,") >= 0 && text.indexOf("200 OK") < 0;
}

HttpResponse Http::processAtCommand(String command, bool(*successPredicate)(String), bool(*failPredicate)(String))
{
	bool success = false;
	String content = "";
	wifiSerial.println(command);
	int retryCount = 10;
	String lastLine;
	bool execute = true;
	while (execute)
	{
		if (wifiSerial.available())
		{
			lastLine = wifiSerial.readStringUntil('\n');
			lastLine.trim();
			content += " " + lastLine;
		}
		execute = successPredicate(lastLine) == false && failPredicate(lastLine) == false;
		// delay(30);
	}
	if (successPredicate(lastLine))
	{
		success = true;
	}
	else if (failPredicate(lastLine))
	{
		success = false;
	}

	HttpResponse result{ success, content };
	return result;
}

int Http::calculateRequestLen(String command)
{
	return command.length() + 2; // + 2 because of /r/n added by the esp8266 firmware
}

String Http::createGetCommandFromPath(String path)
{
	return "GET " + path + " HTTP/1.1\r\nHost: " + API_URL + ":" + API_PORT + "\r\n";
}