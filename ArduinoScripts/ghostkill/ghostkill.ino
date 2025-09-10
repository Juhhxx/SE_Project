#include <MD_MAX72xx.h>

#define PRINT(s, x) { Serial.print(F(s)); Serial.print(x); }
#define PRINTS(x) Serial.print(F(x))
#define PRINTD(x) Serial.println(x, DEC)

#define HARDWARE_TYPE MD_MAX72XX::PAROLA_HW
#define MAX_DEVICES	1

#define CLK_PIN   8  // or SCK
#define DATA_PIN  10  // or MOSI
#define CS_PIN    9  // or SS

// SPI hardware interface
MD_MAX72XX mx = MD_MAX72XX(HARDWARE_TYPE, CS_PIN, MAX_DEVICES);

// We always wait a bit between updates of the display
#define  DELAYTIME  100  // in milliseconds

void setup()
{
  Serial.begin(57600);

  PRINTS("\n[MD_MAX72XX Test & Demo]");

  if (!mx.begin())
    PRINTS("\nMD_MAX72XX initialization failed");
  
}

void draw(uint8_t bitmap[8])
{
  mx.clear();

  // use the bitmap
  mx.control(MD_MAX72XX::UPDATE, MD_MAX72XX::OFF);
  mx.setBuffer(7, COL_SIZE, bitmap);
  mx.control(MD_MAX72XX::UPDATE, MD_MAX72XX::ON);
  delay(DELAYTIME);
}

void loop()
{
   uint8_t arrow[COL_SIZE] =
  {
    0b11111111,
    0b00011100,
    0b00111110,
    0b01111111,
    0b00011100,
    0b00011100,
    0b00111110,
    0b00111100
  };

  draw(arrow);

}