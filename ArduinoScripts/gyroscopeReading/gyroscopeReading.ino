/*
  by MohammedDamirchi
  Modified to include processedgZ scaling
*/

#include <MPU9250_asukiaaa.h>
#include <Adafruit_BMP280.h>

#ifdef _ESP32_HAL_I2C_H_
#define SDA_PIN 21
#define SCL_PIN 22
#endif

Adafruit_BMP280 bme; // I2C
MPU9250_asukiaaa mySensor;

float gZ;
uint8_t processedgZ;
uint8_t directRot;

void setup() {
  Serial.begin(115200);
  while (!Serial);

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

void loop() {
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
    delay(200);
  }
}
