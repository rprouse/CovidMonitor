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

const int numRows = 2;
const int numCols = 16;

String all = "";  // Total confirmed cases
String act = "";  // Total active cases
String cad = "";  // Confirmed cases in Canada
String usa = "";  // Confirmed cases in USA

bool recv_data;
byte show_all = true;

#define VIRUS  0
#define SKULL  1
#define MARKER 2
#define GLOBE  3

byte virus[8] =
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

byte skull[8] =
{
  B10001,
  B01110,
  B10101,
  B01110,
  B01110,
  B00000,
  B01110,
  B10001
};

byte marker[8] =
{
  B01110,
  B01110,
  B11011,
  B11111,
  B01110,
  B01110,
  B00100,
  B00100
};

byte globe[8] =
{
  B00000,
  B01110,
  B11101,
  B11101,
  B10101,
  B01110,
  B00000,
  B00000
};

void setup() {
  // Debugging output
  Serial.begin(57600);
  lcd.init();
  // set up the LCD's number of columns and rows: 
  lcd.begin(numCols, numRows);
  lcd.backlight();
  lcd.createChar(VIRUS, virus);
  lcd.createChar(SKULL, skull);
  lcd.createChar(MARKER, marker);
  lcd.createChar(GLOBE, globe);
  requestData();
}

void loop() 
{
  receiveData();
  if(recv_data)
    displayData();
}

void displayData()
{  
  lcd.clear();
  
  if(show_all)
  {
    lcd.setCursor( 0, 0 );
    lcd.write( (uint8_t)GLOBE );
    lcd.setCursor( 2, 0 );
    lcd.print(all.c_str());
    lcd.setCursor( 2, 1 );
    lcd.print(act.c_str());
  }
  else
  {
    lcd.setCursor( 0, 0 );
    lcd.write( (uint8_t)MARKER );
    lcd.setCursor( 2, 0 );
    lcd.print(cad.c_str());
    lcd.setCursor( 2, 1 );
    lcd.print(usa.c_str());    
  }

  show_all = !show_all;
  delay(5000);  
}

void receiveData()
{    
  if(Serial.available())
  {
    String data = Serial.readString();
    if(data.startsWith("ALL"))
      all = data;
    else if(data.startsWith("ACT"))
      act = data;
    else if(data.startsWith("CAD"))
      cad = data;
    else if(data.startsWith("USA"))
      usa = data;
      
    recv_data = true;
    Serial.write("r");
  }
}

void requestData()
{  
  lcd.setCursor( 0, 0 );
  lcd.write( (uint8_t)VIRUS );
  lcd.print(" Awaiting");
  lcd.setCursor( 2, 1 );
  lcd.print("COVID-19 data");
  recv_data = false;
  show_all = true;
}
