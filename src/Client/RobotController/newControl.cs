using System.Diagnostics;
using System.Text;

namespace MotorController;

class ChildProcess {
  StringBuilder outputBuilder = new();
  ProcessStartInfo processStartInfo;
  Process process;

  void Spawn() {
    // Spawn a new ChildProcess
    processStartInfo = new ProcessStartInfo {
      CreateNoWindow = true,
      RedirectStandardOutput = true,
      RedirectStandardInput = true,
      UseShellExecute = false,
      Arguments = "./stepper.py drive",
      FileName = "/usr/bin/python3"
    };

    process = new Process {
      StartInfo = processStartInfo,
      // enable raising events because Process does not raise events by default
      EnableRaisingEvents = true
    };

    // attach the event handler for OutputDataReceived before starting the process
    process.OutputDataReceived += new DataReceivedEventHandler(
      delegate (object sender, DataReceivedEventArgs e) {
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

  }
}
