using System;
using System.Threading.Tasks;
using System.Device.Gpio;

namespace RobotController
{
  class Motor
  {
    public int STEP_PIN { get; set; }
    public int DIR_PIN { get; set; }
    public int ENABLE_PIN { get; set; }
    private GpioPin stepPin;
    private GpioPin directionPin;
    private GpioPin enablePin;
    private PinValue stepPinValue;
    private PinValue directionPinValue;
    private PinValue enablePinValue;


    public Motor(int sTEP_PIN, int dIR_PIN)
    {
      STEP_PIN = sTEP_PIN;
      DIR_PIN = dIR_PIN;
    }

    public Motor(int sTEP_PIN, int dIR_PIN, int eNABLE_PIN = 0)
    {
      STEP_PIN = sTEP_PIN;
      DIR_PIN = dIR_PIN;
      ENABLE_PIN = eNABLE_PIN;
    }

    public void InitGPIO()
    {
      var gpio = new GpioController();

      stepPin = gpio.OpenPin(STEP_PIN);
      stepPin.SetPinMode(PinMode.Output);
      stepPinValue = PinValue.Low;
      stepPin.Write(stepPinValue);

      directionPin = gpio.OpenPin(DIR_PIN);
      directionPin.SetPinMode(PinMode.Output);
      directionPinValue = PinValue.Low;
      directionPin.Write(directionPinValue);

      if (ENABLE_PIN != 0)
      {
        enablePin = gpio.OpenPin(ENABLE_PIN);
        enablePin.SetPinMode(PinMode.Output);
        enablePinValue = PinValue.High;
        enablePin.Write(enablePinValue);
      }
    }

    private void OneStep()
    {
      var signal = Task.Run(async delegate { await Task.Delay(TimeSpan.FromMilliseconds(1)); });
      var pavza = Task.Run(async delegate { await Task.Delay(TimeSpan.FromMilliseconds(1)); });

      stepPinValue = PinValue.High;
      stepPin.Write(stepPinValue);
      signal.Wait();
      stepPinValue = PinValue.Low;
      stepPin.Write(stepPinValue);
      pavza.Wait();
    }

    public void Step(int steps)
    {
      if (ENABLE_PIN != 0)
      {
        enablePinValue = PinValue.Low;
        enablePin.Write(enablePinValue);
      }

      if (steps <= 0) { directionPinValue = PinValue.Low; }
      else { directionPinValue = PinValue.High; }
      directionPin.Write(directionPinValue);

      for (int i = 0; i < Math.Abs(steps); i++)
      {
        OneStep();
      }

      if (ENABLE_PIN != 0)
      {
        enablePinValue = PinValue.High;
        enablePin.Write(enablePinValue);
      }
    }
  }
}
