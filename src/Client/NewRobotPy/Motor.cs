using System.Diagnostics;
using System.Text;

namespace RobotController
{
  class Motor
  {
    StringBuilder outputBuilder;
    ProcessStartInfo processStartInfo;
    Process process;

    public int STEP_PIN1 { get; set; }
    public int DIR_PIN1 { get; set; }

    public int STEP_PIN2 { get; set; }
    public int DIR_PIN2 { get; set; }

    public Motor(int sTEP_PIN1, int dIR_PIN1, int sTEP_PIN2, int dIR_PIN2)
    {
      this.STEP_PIN1 = sTEP_PIN1;
      this.DIR_PIN1 = dIR_PIN1;

      this.STEP_PIN2 = sTEP_PIN2;
      this.DIR_PIN2 = dIR_PIN2;

    }

   public void Drive(bool direction, int steps)
   {
outputBuilder = new StringBuilder();

processStartInfo = new ProcessStartInfo();
processStartInfo.CreateNoWindow = true;
processStartInfo.RedirectStandardOutput = true;
processStartInfo.RedirectStandardInput = true;
processStartInfo.UseShellExecute = false;
processStartInfo.Arguments = "/home/wouter/roommapper/src/Client/RobotController/stepper.py drive f 200";
processStartInfo.FileName = "/home/wouter/roommapper/src/Client/NewRobotPy/.venv/bin/python3";


process = new Process();
process.StartInfo = processStartInfo;
// enable raising events because Process does not raise events by default
process.EnableRaisingEvents = true;
// attach the event handler for OutputDataReceived before starting the process
process.OutputDataReceived += new DataReceivedEventHandler
(
    delegate(object sender, DataReceivedEventArgs e)
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