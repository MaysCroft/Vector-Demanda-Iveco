// Arduino - Sensor de água na porta A0
const int sensorPin = A0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  int leitura = analogRead(sensorPin);  // valor entre 0 e 1023
  float percentual = (leitura / 1023.0) * 100.0;

  Serial.println(percentual);  // envia percentual para o PC
  delay(500);                  // meio segundo entre leituras
}
