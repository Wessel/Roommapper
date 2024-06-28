using System;
using System.Diagnostics;
using System.Text;

namespace RobotController
{
  class Motor(int stepOne, int dirOne, int stepTwo, int dirTwo)
  {
    StringBuilder outputBuilder;
    ProcessStartInfo processStartInfo;
    Process process;

    public int STEP_PIN1 { get; set; } = stepOne;
    public int DIR_PIN1 { get; set; } = dirOne;
    public int STEP_PIN2 { get; set; } = stepTwo;
    public int DIR_PIN2 { get; set; } = dirTwo;

    public void Drive(bool direction, int steps)
    {
      outputBuilder = new StringBuilder();
      processStartInfo = new ProcessStartInfo
      {
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        UseShellExecute = false,
        Arguments = "../stepper.py drive",
        FileName = "/usr/bin/python3"
      };

      process = new Process
      {
        StartInfo = processStartInfo,
        // enable raising events because Process does not raise events by default
        EnableRaisingEvents = true
      };

      // attach the event handler for OutputDataReceived before starting the process
      process.OutputDataReceived += new DataReceivedEventHandler
      (
        delegate (object sender, DataReceivedEventArgs e)
        {
          // append the new data to the data already read-in
          outputBuilder.Append(e.Data);
        }
       );
      // start the process
      // then begin asynchronously reading the output
      // then wait for the process to exit
      // then cancel asynchronously reading the output
      process.Start();
      process.BeginOutputReadLine();
      process.WaitForExit();
      process.CancelOutputRead();

      // use the output
      string output = outputBuilder.ToString();

      Console.WriteLine(output);

    }
  }
}
