int pinoledred = 12;
int pinoledgreen = 11;
int pinosensor = 8;


void setup() {
  pinMode(pinosensor, INPUT);
  pinMode(pinoledgreen, OUTPUT);
  pinMode(pinoledred, OUTPUT);
  digitalWrite(pinoledred, LOW);
}

void loop() {
  if (digitalRead(pinosensor) == LOW) {
    digitalWrite(pinoledred, HIGH);
    digitalWrite(pinoledgreen, LOW);
  } else {
    digitalWrite(pinoledred, LOW);
    digitalWrite(pinoledgreen, HIGH);
  }
}

