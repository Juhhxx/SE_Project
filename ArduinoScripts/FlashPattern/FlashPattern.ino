#include <FastLED.h>

#define NUM_LEDS 16
#define LED_PIN 13
const int button1Pin = 14;
const int button2Pin = 27;
const int button3Pin = 26;
const int button4Pin = 25;
const int triggerPin = 32;


uint8_t flashMode;  // Variable to track mode

CRGB leds[NUM_LEDS];

void setup () {
  Serial.begin(9600);
  FastLED.addLeds<WS2812B, LED_PIN, GRB>(leds, NUM_LEDS);
  FastLED.setBrightness(50);
  delay(2000);

  pinMode(button1Pin, INPUT_PULLUP);
  pinMode(button2Pin, INPUT_PULLUP);
  pinMode(button3Pin, INPUT_PULLUP);
  pinMode(button4Pin, INPUT_PULLUP);

  pinMode(triggerPin, INPUT_PULLUP);
 }

void loop () {
  flashMode = ModeDetection();

  if (digitalRead(triggerPin) == LOW) //Trigger button is read here
  {
    //FLASH ITSELF
    QuickFlash(flashMode);
    QuickFlash(flashMode);
    SlowFlash(flashMode);
    delay(1000);
  }


  //  Serial.println("Mode: ");
  //  Serial.print(flashMode);
  //  Serial.println("");

  // Serial.print("B1: "); Serial.print(digitalRead(button1Pin));
  // Serial.print("  B2: "); Serial.print(digitalRead(button2Pin));
  // Serial.print("  B3: "); Serial.print(digitalRead(button3Pin));
  // Serial.print("  B4: "); Serial.println(digitalRead(button4Pin));
  // delay(200);
    
}
void QuickFlash(uint8_t mode)
{
  // Quick burst of bright white
  if (mode == -2){
    fill_solid(leds, NUM_LEDS, CRGB::White);
  }
  else if (mode == -1){
    fill_solid(leds, NUM_LEDS, CRGB::Magenta);
  }
  else if (mode == 0){
    fill_solid(leds, NUM_LEDS, CRGB::Yellow);
  }
  else if (mode == 1){
    fill_solid(leds, NUM_LEDS, CRGB::Green);
  }
  else if (mode == 2){
    fill_solid(leds, NUM_LEDS, CRGB::Aquamarine);
  }
  //fill_solid(leds, NUM_LEDS, CRGB::White);
  //fill_solid(leds, NUM_LEDS, CHSV(0,255,0));
  FastLED.setBrightness(255); // max brightness for flash
  FastLED.show();
  delay(100); // very short flash

  // Fade out smoothly
  for(int b = 255; b >= 0; b -= 70) {
    FastLED.setBrightness(b);
    FastLED.show();
    delay(10);
  }

  // Restore normal brightness for the rest of your animations
  FastLED.setBrightness(50);
  FastLED.clear();
  FastLED.show();
}

void SlowFlash(uint8_t mode)
{
  // Quick burst of bright white
  if (mode == -2){
    fill_solid(leds, NUM_LEDS, CRGB::White);
  }
  else if (mode == -1){
    fill_solid(leds, NUM_LEDS, CRGB::Purple);
  }
  else if (mode == 0){
    fill_solid(leds, NUM_LEDS, CRGB::Yellow);
  }
  else if (mode == 1){
    fill_solid(leds, NUM_LEDS, CRGB::Green);
  }
  else if (mode == 2){
    fill_solid(leds, NUM_LEDS, CRGB::Aquamarine);
  }
  FastLED.setBrightness(255); // max brightness for flash
  FastLED.show();
  delay(100); // very short flash

  // Fade out smoothly
  for(int b = 255; b >= 0; b -= 5) {
    FastLED.setBrightness(b);
    FastLED.show();
    delay(10);
  }

  // Restore normal brightness for the rest of your animations
  FastLED.setBrightness(50);
  FastLED.clear();
  FastLED.show();
}
int Trigger(uint8_t trigger)
{
  if (trigger == 1)
  {
    QuickFlash(flashMode);
    QuickFlash(flashMode);
    SlowFlash(flashMode);
    delay(1000);
    trigger = 0;
    return trigger;
  }
}
uint8_t ModeDetection()
{
  if (digitalRead(button1Pin) == LOW) {
    return -1;
  } else if (digitalRead(button2Pin) == LOW) {
    return 0;
  } else if (digitalRead(button3Pin) == LOW) {
    return 1;
  } else if (digitalRead(button4Pin) == LOW) {
    return 2;
  }
  return -2; // default if no button is pressed
}



 