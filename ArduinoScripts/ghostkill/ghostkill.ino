#include "WiFi.h"
#include <AsyncUDP.h>
#include <WiFiAP.h>

#include <MD_MAX72xx.h>

#define PRINT(s, x) { Serial.print(F(s)); Serial.print(x); }
#define PRINTS(x) Serial.print(F(x))
#define PRINTD(x) Serial.println(x, DEC)

#define HARDWARE_TYPE MD_MAX72XX::PAROLA_HW
#define MAX_DEVICES	1

#define CLK_PIN   18 // or SCK
#define DATA_PIN  23  // or MOSI
#define CS_PIN    19  // or SS

WiFiServer server(80);

AsyncUDP udp;

// SPI hardware interface
MD_MAX72XX mx = MD_MAX72XX(HARDWARE_TYPE, CS_PIN, MAX_DEVICES);

// We always wait a bit between updates of the display
#define  DELAYTIME  100  // in milliseconds

const char *ssid = "ESP32_GHOST_2";
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
      packet.printf("Got %u bytes of data", packet.length());
    });
  }
}

void draw(uint8_t bitmap[8]) {
  mx.clear();

  // use the bitmap
  mx.control(MD_MAX72XX::UPDATE, MD_MAX72XX::OFF);
  mx.setBuffer(7, COL_SIZE, bitmap);
  mx.control(MD_MAX72XX::UPDATE, MD_MAX72XX::ON);
  delay(DELAYTIME);
}

void setup() {

  Serial.begin(115200);
  delay(1000);

  Serial.println("start");

  startServer();

  initializeUDP();

  if (!mx.begin())
    PRINTS("\nMD_MAX72XX initialization failed");
  
}

void loop() {

  delay(1000);
  //Send broadcast
  udp.broadcast("Anyone here?");

}