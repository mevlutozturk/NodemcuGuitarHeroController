
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#define btn1 16
#define btn2 5
#define btn3 4
#define btn4 12
#define btn5 14

String send_Data ="";
String previous_sendData = "";

char command;


const char* ssid = "YOUR SSID";
const char* password = "YOUR PASSWORD";
const char* mqtt_server = "YOUR IP ADDRESS";

WiFiClient espClient;
PubSubClient client(espClient);
unsigned long lastMsg = 0;
#define MSG_BUFFER_SIZE  (50)
char msg[MSG_BUFFER_SIZE];

void setup_wifi() {

  delay(10);
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
  if ((String)topic == "Keyboard")
  {
  }
}

void reconnect() {
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    if (client.connect(clientId.c_str())) {
      Serial.println("connected");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      delay(5000);
    }
  }
}

void setup() {
  pinMode(btn1, INPUT_PULLUP);
  pinMode(btn2, INPUT_PULLUP);
  pinMode(btn3, INPUT_PULLUP);
  pinMode(btn4, INPUT_PULLUP);
  pinMode(btn5, INPUT_PULLUP);
  Serial.println("Upload Success");
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);
}

void loop() {

  if (!client.connected()) {
    reconnect();
  }


  send_Data = "";
  if(digitalRead(btn5)){
    send_Data += "1";
  }
  else{
    send_Data += "0";
  }
  if(digitalRead(btn4)){
    send_Data += "1";
  }
  else{
    send_Data += "0";
  }
  if(digitalRead(btn3)){
    send_Data += "1";
  }
  else{
    send_Data += "0";
  }
  if(digitalRead(btn2)){
    send_Data += "1";
  }
  else{
    send_Data += "0";
  }
  if(digitalRead(btn1)){
    send_Data += "1";
  }
  else{
    send_Data += "0";
  } 
  if(analogRead(A0)<200){
    send_Data += 1;
  }
  else{
    send_Data += 0;
  }
  if(analogRead(A0)>800){
    send_Data += 1;
  }
  else{
    send_Data += 0;
  }
  if (send_Data != previous_sendData){
    Serial.println(send_Data);
    client.publish("Data", send_Data.c_str());
    
  }
  previous_sendData = send_Data;
  delay(16);
  
  client.loop();
}
