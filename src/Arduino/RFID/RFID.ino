#include <SoftwareSerial.h>
#include <http.h>
#include <SPI.h>
#include <MFRC522.h>

const String serialNumber = "arduino-CD32016BC";

const int RST_PIN = 9;
const int SS_PIN = 10;

const int RED_PIN = 2;
const int GREEN_PIN = 3;

MFRC522 rfid(SS_PIN, RST_PIN);

Http http(5, 6);

void setup() {
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  Serial.begin(9600);
  while(!Serial);
  SPI.begin();
  rfid.PCD_Init();
  http.init();
  indicate_init_led();

  rfid.PCD_SetAntennaGain(rfid.RxGain_max);
}

void loop() {
  if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {   
    Serial.println(millis());
    String tag_number = get_uid_code(rfid.uid.uidByte, rfid.uid.size);
    if(tag_number && tag_number.length() != 0) {
      if(check_access(tag_number)) {
        green_light_led();
      }
      else {
        red_light_led();
      }
      Serial.println(millis());
    }
  }
}

String get_uid_code(byte bytes[], byte size){
  String result = "";
  for(int i = 0; i < size; i++) {
    result += bytes[i];
  }
  return result;
}

bool check_access(String tagNumber) {
  HttpResponse response = http.checkAccess(tagNumber, serialNumber);
  return response.success;
}

void red_light_led() {
  digitalWrite(RED_PIN, HIGH);
  delay(1000);
  digitalWrite(RED_PIN, LOW);
}


void green_light_led() {
  digitalWrite(GREEN_PIN, HIGH);
  delay(1000);
  digitalWrite(GREEN_PIN, LOW);
}

void indicate_init_led() {
  digitalWrite(GREEN_PIN, HIGH);
  digitalWrite(RED_PIN, HIGH);
  delay(1000);
  digitalWrite(GREEN_PIN, LOW);
  digitalWrite(RED_PIN, LOW);
}
