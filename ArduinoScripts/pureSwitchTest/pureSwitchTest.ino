const int button1Pin = D2;
const int button2Pin = D3;
const int button3Pin = D4;
const int button4Pin = D5;

uint8_t mode = 0;  // Variable to track mode

void setup() {
  Serial.begin(9600);

  pinMode(button1Pin, INPUT_PULLUP);
  pinMode(button2Pin, INPUT_PULLUP);
  pinMode(button3Pin, INPUT_PULLUP);
  pinMode(button4Pin, INPUT_PULLUP);
}

void loop() {
  // if (digitalRead(button1Pin) == LOW) {
  //   mode = 1;
  // } else if (digitalRead(button2Pin) == LOW) {
  //   mode = 2;
  // } else if (digitalRead(button3Pin) == LOW) {
  //   mode = 3;
  // }else if (digitalRead(button4Pin) == LOW){
  //   mode = 4;
  // } 

  mode = ModeDetection();
  // Just for debugging
  Serial.print("Current mode: ");
  Serial.println(mode);

  delay(100); // simple debounce
}

uint8_t ModeDetection()
{
  uint8_t mode = 0;  // Variable to track mode
  if (digitalRead(button1Pin) == LOW) {
    return mode = 1;
  } else if (digitalRead(button2Pin) == LOW) {
    return mode = 2;
  } else if (digitalRead(button3Pin) == LOW) {
    return mode = 3;
  }else if (digitalRead(button4Pin) == LOW){
    return mode = 4;
  } 
}

