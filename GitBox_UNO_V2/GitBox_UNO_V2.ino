//Code for Arduino UNO. This code
//1. Take readings from pH sensors 
//2. Send them to PC (independent of PC itself)
//3. Take commands from PC and execute (pH pumps)

#define tol 0.3 //tolerance for pH
#define pumptime 1000//How long each acid/base addition last, in milliseconds
//Set the setpoints
float setpoint[3] = {2.0,6.7,7.4};

char instring[4]; //This stores the input from the Serial port
int instring_int[4]; //This stores the input from the Serial port converted to integer 

int analogPins[5] = {A0,A1,A2,A3,A4};

int pumppins[6]= {3,5,6,9,1,1};//pins for pumps for pH control
//For these pump pins, #1 = acid & #2 = base are for sensor 1,
//#3=acid & #4=base are for sensor 2, etc

int readings[6];//stores the readings from each sensor
float readinsf[6];//Actual pH readings
int slope[6];//Slopes of sensors
int intercept[6];//intercepts of sensors

int i,j;
int k = 3;//k denotes the maximum sensors in use right now
int timer; //for tracking how oftern do we send a signal to PC

void setup() { 
 for (i = 0;i<k;i++)
  {
    pinMode(analogPins[i],INPUT);
  }
   for (i = 0;i<6;i++)
  {
    pinMode(pumppins[i],OUTPUT);
  }
  Serial.begin(9600);  
  //Serial.setTimeout(50);

  //Sensor 1
  slope[0] = -232;
  intercept[0] = 4648;
  //Sensor 2
  slope[1] = -216;
  intercept[1] = 4765;
  //Sensor 3
  slope[2] = -245;
  intercept[2] = 4397;
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
  if ((int)instring[1]>48){ //check if the signal is valid?
    for (i = 0;i<4;i++)
    {
      instring_int[i] = (int)instring[i]-48; //parse the input data into 5 integer; the -48 is to convert the ascii number into integer
    }
    device_number = instring_int[2]*10+ instring_int[3];
    device_type = instring_int[1];
    device_intensity = instring_int[0];
  } 
  //We now only have 1 device to be controlled so device type doesnt matter
      for (k=1;k<7;k++){
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
 
  //Reading signals from sensors
 for (j = 0;j<k;j++)
  {
    readings[j] = analogRead(analogPins[j]); 
    //calculate actual pH 
    readingsf[j] = (readings[j] - intercept[j])/slope[j];  
    Serial.print(j+1);
    Serial.println(readingsf[j]);
    delay(200);
  }

//Controlling pumps
  if (timer > 5) {
    for (j = 0;j<k;j++) {
      if (readingsf[j] - setpoint[j] < tol) {
        digitalWrite(pumpspin[2*j],HIGH);
        delay(pumptime);
        digitalWrite(pumpspin[2*j],LOW);
      }
      if (readingsf[j] - setpoint[j] > tol){
        digitalWrite(pumpspin[2*j+1],HIGH);
        delay(pumptime);
        digitalWrite(pumpspin[2*j+1],LOW);
      }
    }
    timer = 0;
  }
  timer + = 1;
  delay(1000);
}
