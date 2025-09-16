#include "WiFi.h"
#include <AsyncUDP.h>
#include <WiFiAP.h>
#include <MPU9250_asukiaaa.h>
#include <Adafruit_BMP280.h>
#include <MD_MAX72xx.h>

#ifdef _ESP32_HAL_I2C_H_
#define SDA_PIN 21
#define SCL_PIN 22
#endif

Adafruit_BMP280 bme; // I2C
MPU9250_asukiaaa mySensor;

float gZ;
uint8_t processedgZ;
uint8_t directRot;

void initializeGyroscope() {

#ifdef _ESP32_HAL_I2C_H_ // For ESP32
  Wire.begin(SDA_PIN, SCL_PIN);
  mySensor.setWire(&Wire);
#else
  Wire.begin();
  mySensor.setWire(&Wire);
#endif

  bme.begin();
  mySensor.beginGyro();
}

void processGyroSignal() {

  if (mySensor.gyroUpdate() == 0) {
    gZ = mySensor.gyroZ();

    // Determine direction and magnitude
    if (gZ < 0) {
      gZ = -gZ;           // take absolute value
      directRot = 0;
    } else {
      directRot = 1;
    }

    if (gZ > 20) {
      // Scale gZ into 0–255 range (assuming max meaningful = 500 deg/s)
      if (gZ > 500) gZ = 500;
      processedgZ = (uint8_t) map((long) gZ, 0, 180, 0, 255);

      // Print raw + processed values
      Serial.print("\tgyroZ: ");
      Serial.print(gZ);
      Serial.print("  |  Direção: ");
      Serial.print(directRot);
      Serial.print("  |  Processed: ");
      Serial.print(processedgZ);
      Serial.println();
    }
    else
    {
      processedgZ = 0;
    }
  }
}

#define PRINT(s, x) { Serial.print(F(s)); Serial.print(x); }
#define PRINTS(x) Serial.print(F(x))
#define PRINTD(x) Serial.println(x, DEC)

#define HARDWARE_TYPE MD_MAX72XX::PAROLA_HW
#define MAX_DEVICES	1

// SPI hardware interface
MD_MAX72XX mx = MD_MAX72XX(HARDWARE_TYPE, CS_PIN, MAX_DEVICES);

#define CLK_PIN   18 // or SCK
#define DATA_PIN  23  // or MOSI
#define CS_PIN    13  // or SS

WiFiServer server(80);

AsyncUDP udp;

const char *ssid = "ESP32_GHOST";
const char *password = "12345678";

void startServer() {
  Serial.println("Starting Server");
  // You can remove the password parameter if you want the AP to be open.
  // a valid password must have more than 7 characters
  if (!WiFi.softAP(ssid, password)) {
    log_e("Soft AP creation failed.");
    while (1);
  }
  IPAddress myIP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(myIP);
  server.begin();

  Serial.println("Server started");
}

void initializeUDP() {
  if (udp.listen(1234)) {
    Serial.print("UDP Listening on IP: ");
    Serial.println(WiFi.softAPIP());
    udp.onPacket([](AsyncUDPPacket packet) {
      Serial.print("UDP Packet Type: ");
      Serial.print(packet.isBroadcast() ? "Broadcast" : packet.isMulticast() ? "Multicast" : "Unicast");
      Serial.print(", From: ");
      Serial.print(packet.remoteIP());
      Serial.print(":");
      Serial.print(packet.remotePort());
      Serial.print(", To: ");
      Serial.print(packet.localIP());
      Serial.print(":");
      Serial.print(packet.localPort());
      Serial.print(", Length: ");
      Serial.print(packet.length());
      Serial.print(", Data: ");
      Serial.write(packet.data(), packet.length());
      Serial.println();

      if (packet.length() == 8) {
          uint8_t bitmap[8];
          memcpy(bitmap, packet.data(), 8);  // copy bytes from UDP packet
          draw(bitmap);                       // call your method
      }
      //reply to the client
     uint8_t response[2] = { processedgZ, directRot };
      
      packet.write(response, sizeof(response));
    });
  }
}

void draw(uint8_t bitmap[8]) {
  mx.clear();

  // use the bitmap
  mx.control(MD_MAX72XX::UPDATE, MD_MAX72XX::OFF);
  mx.setBuffer(7, COL_SIZE, bitmap);
  mx.control(MD_MAX72XX::UPDATE, MD_MAX72XX::ON);
}

void setup() {

  Serial.begin(115200);
  delay(1000);

  Serial.println("start");

  startServer();

  initializeUDP();

  initializeGyroscope();

  if (!mx.begin())
    PRINTS("\nMD_MAX72XX initialization failed");
  
}

void loop() {

  delay(1000);
  //Send broadcast
  processGyroSignal();

}