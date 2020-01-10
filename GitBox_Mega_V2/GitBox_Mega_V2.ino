//Code for Arduino Mega1
//This code 
//1. Take commands from PC and execute (liquid transfer pumps). 
//2. Send voltage to gas sparger. 
//3. Send voltage to pumps for liquid transfer.
//The following time stamps are all in seconds
#define TimeStamp1 60 //Food to stomach start
#define TimeStamp2 180 //Food to stomach stop
#define TimeStamp3 1980 //stomach to intestine start
#define TimeStamp4 2100 //stomach to intestine stop
#define TimeStamp5 3900 //Intestine to colon start
#define TimeStamp6 4020 //Intestine to colon stop
#define TimeStamp7 14400 //colon to waste start
#define TimeStamp8 14520 //colon to waste stop
#define initialization_time 60000 //Time to run pump priming

char instring[4]; //This stores the input from the Serial port
int instring_int[4]; //This stores the input from the Serial port converted to integer  
int i; 
int k;
int j;//Placeholder counter

int pumppins[4]= {3,5,6,9};//pins for pumps for liquid transfer
int spargerpin = 4;

int device_number;//To store the numbering of device
int device_type;//To store the type of device
int device_intensity;//To store intensity of device

int timer = 0;//Timer for liquid transfer
int tspa = 0; // Timer for sparging gas
int prime_flag = 0;//Flag for pumps having been primed

void setup() {
 Serial.begin(9600);
  for (i=0;i<5;i++)
  {
    pinMode (pumppins[i],OUTPUT);
    pinMode (spargerpin,OUTPUT);
  }  
  //Serial.setTimeout(50);
}

void loop() {
  //This part remains the same; If the PC sends
  //a signal, parse it and execute the commands
  // clear all the input caches and variables
  for (i = 0;i<4;i++)
  {
    instring[i] = ""; 
  }
    device_number = 0;
    device_type = 0;
    device_intensity = 0;
 if (Serial.available()>0){
  Serial.readBytesUntil("\r",instring,4);
  if ((int)instring[3]>48){ //check if the signal is valid?
    for (i = 0;i<4;i++)
    {
      instring_int[i] = (int)instring[i]-48; //parse the input data into 5 integer; the -48 is to convert the ascii number into integer
    }
    device_number = instring_int[2]*10+ instring_int[3];
    device_type = instring_int[1];
    device_intensity = instring_int[0];
  } 
  if (device_type >0) {//Non-zero device types means pumps
      for (k=1;k<5;k++){
        if (k == device_number){//digital control
            if (device_intensity == 1){
              digitalWrite(pumppins[k-1],HIGH);
            }
            else {
              digitalWrite(pumppins[k-1],LOW);
            }
        }
      }
  }
  else {//Initialization process (pump priming)
  if(device_intensity >0){
  for (j = 0;j<4;j++){
  digitalWrite(pumppins[j],HIGH);
  }
 delay(initialization_time);
 for (j = 0;j<4;j++){
  digitalWrite(pumppins[j],LOW);
   } 
  }
  device_intensity = 0;
  prime_flag = 1;
 }
}
if (prime_flag > 0){
//Now, check if we need liquid transfer
  if ((timer > TimeStamp1) && (timer < TimeStamp2)){
    digitalWrite(pumppins[0],HIGH);
  }
  else {
    digitalWrite(pumppins[0],LOW);
  }
  if ((timer > TimeStamp3) && (timer < TimeStamp4)){
    digitalWrite(pumppins[1],HIGH);
  }
  else {
    digitalWrite(pumppins[1],LOW);
  }
  if ((timer > TimeStamp5) && (timer < TimeStamp6)){
    digitalWrite(pumppins[2],HIGH);
  }
  else {
    digitalWrite(pumppins[2],LOW);
  }
  if ((timer > TimeStamp7) && (timer < TimeStamp8)){
    digitalWrite(pumppins[3],HIGH);
  }
  else {
    digitalWrite(pumppins[3],LOW);
  }
  if (timer > TimeStamp8){
    timer = 0;
  }
  timer += 1;
}
//Now, handles the gas sparging
  if (tspa > 10){//reset timer if it exceeds 10 secs
    tspa = 0;
  }
  tspa += 1;
  if (tspa > 1 && tspa < 3){//between 1s and 3s, sparge
    digitalWrite(spargerpin,HIGH);
  }
  else {
    digitalWrite(spargerpin,LOW);
  }

 //After everything, wait for 1 sec
  delay(1000); 
}
