using System;
using System.Threading;
using System.Device.Gpio;
using System.Diagnostics;

namespace RobotController
{
  class Ultrasonic
  {
    private int ECHOPIN { get; set; }
    private int TRIGGERPIN { get; set; }

    private GpioPin echoPin;
    private GpioPin triggerPin;

    private GpioController gpio;

    Ultrasonic(int ECHO, int TRIGGER)
    {
      this.ECHOPIN = ECHO;
      this.TRIGGERPIN = TRIGGER;

      this.gpio = new GpioController();
    }
    public void Initialize()
    {
      // Set trigger pin as output and set it high
      this.gpio.OpenPin(this.TRIGGERPIN, PinMode.Output);
      this.gpio.Write(this.TRIGGERPIN, PinValue.High);

      // Set echo pin as input
      this.gpio.OpenPin(this.ECHOPIN, PinMode.Input);

      Thread.Sleep(500); // delay 500ms
    }
    public int MeasurePulseWidth(int pin, PinValue level)
    {
      Stopwatch stopwatch = new Stopwatch();

      // Wait for the signal to go high
      while (this.gpio.Read(pin) != PinValue.High)
      {
        // timeout after 50ms
        if (stopwatch.ElapsedMilliseconds > 50)
          return 50000;
      }

      stopwatch.Start();

      // Wait for the signal to go low
      while (this.gpio.Read(pin) != PinValue.Low)
      {
        // timeout after 50ms
        if (stopwatch.ElapsedMilliseconds > 50)
          return 50000;
      }

      stopwatch.Stop();

      return ((int)stopwatch.Elapsed.TotalMilliseconds * 1000) / 50; // convert to microseconds / 50 = cm
    }

  }
}
