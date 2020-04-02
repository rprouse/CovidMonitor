# CovidMonitor

A simple .NET Core worker service that fetches the latest confirmed COVID-19 cases from the John Hopkins dataset and displays them on an LCD screen powered by an Arduino Nano. It will update the data every hour.

### Awaiting data

![Awaiting data](images/awaiting.jpg)

### Displaying World-Wide Data

All confirmed cases and all active cases.

![Displaying summary data](images/summary.jpg)

### Displaying Regional Data

Confirmed cases in Canada and the United States.

![Displaying regional data](images/regional.jpg)

## Running the Covid19DataProvider

The data provider runs on the desktop computer, fetches the latest COVID-19 data and sends it to the connected Arduino Nano over the serial port for display.

The data provider is hard coded to look for the Arduinon on `COM3`. Change `COM_PORT` in `Worker.cs` to your COM port. If anyone every uses this code, maybe I'll change it to a configuration option.

### Running from the command line


First we need to publish the application to a directory on our computer. I'm using `C:\Users\rob\OneDrive\bin\covid19\`.

From the root of the solution;

```bat
dotnet publish -o C:\Users\rob\OneDrive\bin\covid19\ -c Release .\Covid19DataProvider\
```

Then we can run the program from the command line.


```bat
C:\Users\rob\OneDrive\bin\covid19\Covid19DataProvider.exe
```

The program will continue running and updating the display every 15 minutes.