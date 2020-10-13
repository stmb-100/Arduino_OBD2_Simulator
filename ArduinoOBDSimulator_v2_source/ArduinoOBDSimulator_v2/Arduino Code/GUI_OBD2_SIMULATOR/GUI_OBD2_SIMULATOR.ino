#include "mcp_can.h"

#define REC_B_SIZE 4
#define TOTAL_PARAM 11

uint32_t canId = 0x000;
const int SPI_CS_PIN = 10;

MCP_CAN CAN(SPI_CS_PIN);

int isIdeafFlag = 0;
unsigned long varArr[10] = {0};

unsigned long time, time1;

unsigned char len = 0;
unsigned char buf[8];
char str[20];
String BuildMessage = "";

char rndSpeed ;
char rndRPM1 ;
char rndRPM2 ;
char rndCoolantTemp;
char rndEngRunTime;
char rndIAT;
char rndMAF;
char rndMAP;


void float2Bytes(unsigned char* bytes_temp[4], float float_variable)
{
  union
  {
    float a;
    unsigned char bytes[4];
  } thing;
  thing.a = float_variable;
  memcpy(bytes_temp, thing.bytes, 4);
}

//mode 1 PID
//    unsigned char SupportedPID1[8] = {0x02,0x41,0x00,0xBE,0x3E,0xB0,0x13,0x00};
unsigned char SupportedPID1[8] = {0x02, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
unsigned char SupportedPID2[8] = {0x02, 0x41, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00};
unsigned char SupportedPID3[8] = {0x02, 0x41, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00};
unsigned char SupportedPID4[8] = {0x02, 0x41, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00};
unsigned char SupportedPID5[8] = {0x02, 0x41, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00};
unsigned char SupportedPID6[8] = {0x02, 0x41, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00};
unsigned char SupportedPID7[8] = {0x02, 0x41, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00};

unsigned char DistTravelled[8] = {0x03, 0x41, 0x31, 0x50, 0x00, 0x00, 0x00, 0x00};
unsigned char Throttle[8] = {0x03, 0x41, 0x11, 0x02, 0x00, 0x00, 0x00, 0x00};

unsigned char EngineSpeed[8] =  {0x04, 0x41, 0x0C, 0x4e, 0x20, 0x00, 0x00, 0x00};
unsigned char VehicleSpeed[8] = {0x03, 0x41, 0x0D, 0x50, 0x00, 0x00, 0x00, 0x00};
unsigned char CoolantTemp[8] = {0x03, 0x41, 0x05, 0x46, 0x00, 0x00, 0x00, 0x00};

unsigned char EngRunTime[8] = {0x04, 0x41, 0x1F, 0x00, 0x78, 0x00, 0x00, 0x00};
unsigned char IAT[8] = {0x03, 0x41, 0x0F, 0x60, 0x00, 0x00, 0x00, 0x00};
unsigned char MAF[8] = {0x04, 0x41, 0x10, 0x00, 0xE5, 0x00, 0x00, 0x00};
unsigned char MAP[8] = {0x03, 0x41, 0x0B, 0x22, 0x00, 0x00, 0x00, 0x00};

unsigned char FuelTankLevel[8] = {0x03, 0x41, 0x2f, 0x22, 0x00, 0x00, 0x00, 0x00};
unsigned char DistTraveledWithMIL[8] = {0x03, 0x41, 0x21, 0x22, 0x00, 0x00, 0x00, 0x00};

//mode 3 Frame
unsigned char DTCFrame[8] = {0x04, 0x43, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00};

//mode 4 Frame
unsigned char DTCClearFrame[8] = {0x01, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

//mode 9 PID
unsigned char CalibID[8] = {0x03, 0x49, 0x04, 0x22, 0x00, 0x00, 0x00, 0x00};
unsigned char CVN[8] = {0x03, 0x49, 0x06, 0x22, 0x00, 0x00, 0x00, 0x00};
unsigned char ECU_NAME[8] = {0x03, 0x49, 0x0A, 0x22, 0x00, 0x00, 0x00, 0x00};


void setup()
{
  Serial.begin(9600);

START_INIT:

  if (CAN_OK == CAN.begin(CAN_500KBPS, MCP_8MHz)) // bit rate and set crystall frequency
  {
    Serial.println("CAN BUS Shield init ok!");
  }
  else
  {
    Serial.println("CAN BUS Shield init fail");
    Serial.println("Init CAN BUS Shield again");
    delay(1000);
    goto START_INIT;
  }
  time1 = millis();
}

void loop()
{

  char str[80];
  while (1)
  {
    if (Serial.available())
    {
      int ind = 0;
      char str[10];
      unsigned char databyte[REC_B_SIZE * TOTAL_PARAM] = {0};
      delay(50);
      while (Serial.available() && (ind < (REC_B_SIZE * TOTAL_PARAM))) {
        databyte[ind] = Serial.read();
        ind++;
      }
//      for (int i = 0; i < ind ; i++)
//      {
//        sprintf(str, " %x", databyte[i]);
//        Serial.print(str);
//      }
      if (ind == (REC_B_SIZE * TOTAL_PARAM))
      {
        Serial.println("\nArduino Received Command successfully!!");
        DistTravelled[4] = databyte[3];
        DistTravelled[3] = databyte[2];


        VehicleSpeed[3] = databyte[7];

        CoolantTemp[3] = databyte[11];

        EngRunTime[4] = databyte[15];
        EngRunTime[3] = databyte[14];

        Throttle[3] =  databyte[19];

        IAT[3] = databyte[23];

        MAF[4] = databyte[27];
        MAF[3] = databyte[26];

        MAP[3] = databyte[31];

        FuelTankLevel[3] = databyte[35];

        EngineSpeed[4] = databyte[39];
        EngineSpeed[3] = databyte[38];

        (databyte[43] + databyte[43]) > 0 ? DTCFrame[2] = 1 : DTCFrame[2] = 0;
        DTCFrame[4] =  databyte[43];
        DTCFrame[3] =  databyte[42];
      }
    }

    if (CAN_MSGAVAIL == CAN.checkReceive())
    {
      CAN.readMsgBuf(&len, buf);
      canId = CAN.getCanId();
      Serial.println();
      Serial.print("<");
      Serial.print(canId, HEX);
      Serial.print(", PID ");
      Serial.print(buf[2], HEX);
      Serial.print(", ");
      for (int i = 0; i < len; i++)
      {
        Serial.print(buf[i], HEX);
        Serial.print(" ,");
      }
      Serial.print("--");
      BuildMessage = "";

      if (buf[2] == 0x05 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, CoolantTemp);
        Serial.print(" CoolantTemp");
      }
      if (buf[2] == 0x0B && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, MAP);
        Serial.print(" MAP");
      }
      if (buf[2] == 0x0C && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, EngineSpeed);
        Serial.print(" EngineSpeed");
      }
      if (buf[2] == 0x0D && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, VehicleSpeed);
        Serial.print(" VehicleSpeed");
      }
      if (buf[2] == 0x0F && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, IAT);
        Serial.print(" IAT");
      }
      if (buf[2] == 0x10 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, MAF);
        Serial.print(" MAF");
      }
      if (buf[2] == 0x1F && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, EngRunTime);
        Serial.print(" EngRunTime");
      }
      if (buf[2] == 0x31 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, DistTravelled);
        Serial.print(" DistTravelled");
      }

      if (buf[2] == 0x11 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, Throttle);
        Serial.print(" Throttle");
      }

      if (buf[2] == 0x2F && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, FuelTankLevel);
        Serial.print(" FuelTankLevel");
      }
      if (buf[2] == 0x21 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, DistTraveledWithMIL);
        Serial.print(" DistTraveledWithMIL");
      }
      if (buf[2] == 0x00 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID1);
        Serial.println("SupportedPID1");
      }
      if (buf[2] == 0x20 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID2);
        Serial.println("SupportedPID2");
      }
      if (buf[2] == 0x40 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID3);
        Serial.println("SupportedPID3");
      }
      if (buf[2] == 0x60 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID4);
        Serial.println("SupportedPID4");
      }
      if (buf[2] == 0x80 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID5);
        Serial.println("SupportedPID5");
      }
      if (buf[2] == 0xA0 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID6);
        Serial.println("SupportedPID6");
      }
      if (buf[2] == 0xC0 && buf[1] == 0x01) {
        CAN.sendMsgBuf(0x7E8, 0, 8, SupportedPID7);
        Serial.println("SupportedPID7");
      }

      //Mode 3 PID
      if (buf[1] == 0x03) {
        CAN.sendMsgBuf(0x7E8, 0, 8, DTCFrame);
        Serial.print(" DTCFrame");
      }

      //Mode 4 PID
      if (buf[1] == 0x04) {
        CAN.sendMsgBuf(0x7E8, 0, 8, DTCClearFrame);
        Serial.print(" DTCClearFrame");
        DTCFrame[2] = 0;
        DTCFrame[3] = 0;
        DTCFrame[4] = 0;
      }

            //Mode 3 PID
      if (buf[1] == 0x04) {
        CAN.sendMsgBuf(0x7E8, 0, 8, DTCFrame);
        Serial.print(" DTCFrame");
      }

      //Mode 9 PID
      if (buf[2] == 0x04 && buf[1] == 0x09) {
        CAN.sendMsgBuf(0x7E8, 0, 8, CalibID);
        Serial.print("CalibID");
      }
      if (buf[2] == 0x06 && buf[1] == 0x09) {
        CAN.sendMsgBuf(0x7E8, 0, 8, CVN);
        Serial.print("CVN");
      }
      if (buf[2] == 0x0A && buf[1] == 0x09) {
        CAN.sendMsgBuf(0x7E8, 0, 8, ECU_NAME);
        Serial.print("ECU_NAME");
      }
    }
  }
}
