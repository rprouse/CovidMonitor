// include the library code:
#include <Wire.h>
//#include <Adafruit_RGBLCDShield.h>
//#include <utility/Adafruit_MCP23017.h>

#include <LiquidCrystal_I2C.h>

// The shield uses the I2C SCL and SDA pins. On classic Arduinos
// this is Analog 4 and 5 so you can't use those for analogRead() anymore
// However, you can connect other I2C sensors to the I2C bus and share
// the I2C bus.
//Adafruit_RGBLCDShield lcd = Adafruit_RGBLCDShield();
LiquidCrystal_I2C lcd(0x27,20,4);

#define OFF 0x0
#define ON  0x1

const int numRows = 2;
const int numCols = 16;

String all;
String cad;
String usa;

bool recv_all;
bool recv_cad;
bool recv_usa;

byte show_cad = true;

unsigned long last_request;

const int REFRESH = 3 * 60 * 60 * 1000;

byte logo[8] =
{
  B01010,
  B10001,
  B10101,
  B01010,
  B11011,
  B00100,
  B01010,
  B11011
};

void setup() {
  // Debugging output
  Serial.begin(57600);
  lcd.init();
  // set up the LCD's number of columns and rows: 
  lcd.begin(numCols, numRows);
  lcd.backlight();
  lcd.createChar(0, logo);
  requestData();
}

void loop() 
{
  if(!recv_all || !recv_cad || !recv_usa)
    receiveData();
  else if (last_request + REFRESH < millis())
    requestData();
  else
    displayData();
}

void displayData()
{  
  lcd.clear();
  lcd.setCursor( 0, 0 );
  lcd.print(all.c_str());
  lcd.setCursor( 0, 1 );
  lcd.print((show_cad ? cad : usa).c_str());

  show_cad = !show_cad;
  delay(3000);  
}

void receiveData()
{    
  if(Serial.available())
  {
    if(!recv_all)
    {
      all = Serial.readString();
      recv_all = true;
    }
    else if(!recv_cad)
    {
      cad = Serial.readString();      
      recv_cad = true;
    }
    else if(!recv_usa)
    {
      usa = Serial.readString();      
      recv_usa = true;
    }
  }
}

void requestData()
{
  last_request = millis();
  
  lcd.setCursor( 0, 0 );
  lcd.write( (uint8_t)0 ); // Logo
  lcd.print(" Requesting");
  lcd.setCursor( 2, 1 );
  lcd.print("data...");
  recv_all = false;
  recv_cad = false;
  recv_usa = false;
  show_cad = true;
  Serial.write("?");
}
