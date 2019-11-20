#include <Arduino.h>
#include <SoftwareSerial.h>

class HttpResponse
{
public:
  bool success;
  String content;
};

class Http
{
public:
  bool init();
  HttpResponse checkAccess(String tagNumber, String accessPointSerialNumber);

  Http(int txPin, int rxPin);

private:
  SoftwareSerial wifiSerial;
  HttpResponse setStaticMode();
  HttpResponse connectToWifi();
  HttpResponse openConnection();
  HttpResponse closeConnection();
  HttpResponse sendSize(String command);
  static bool endsWithOk(String text);
  static bool endsWithErrorOrFail(String text);
  static bool authorized(String text);
  static bool unAuthorized(String text);
  HttpResponse processAtCommand(String command, bool (*successPredicate)(String), bool (*failPredicate)(String));
  int calculateRequestLen(String path);
  String createGetCommandFromPath(String path);
};