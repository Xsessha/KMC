
const int buttonPin = 2;     
const int ledPin    = 8;             
const int sensorPin = A0;              

const unsigned long interval      = 1000;  
const unsigned long debounceDelay = 50;     

bool ledState = false;                      
bool lastReading = HIGH;              
bool buttonState = HIGH;                    
unsigned long lastDebounceTime = 0;        
unsigned long lastMeasurement  = 0;         

void setup() {
  pinMode(buttonPin, INPUT_PULLUP);      
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  Serial.begin(9600);                  

void loop() {
  unsigned long now = millis();        

  bool reading = digitalRead(buttonPin);
  if (reading != lastReading) {
    lastDebounceTime = now;              
  }
  if ((now - lastDebounceTime) >= debounceDelay && reading != buttonState) {
    buttonState = reading;              
    if (buttonState == LOW) {              
      ledState = !ledState;
      digitalWrite(ledPin, ledState);
    }
  }
  lastReading = reading;

  if (now - lastMeasurement >= interval) {
    lastMeasurement = now;
    int temperature = map(analogRead(sensorPin), 0, 1023, -20, 40);  

    Serial.print("t=");
    Serial.print(now);
    Serial.print(";temp=");
    Serial.println(temperature);      
}
