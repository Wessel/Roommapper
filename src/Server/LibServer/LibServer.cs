namespace LibServer;

using LibHttp;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

public class Server {
  private const string LogPrefix = "\u001b[44m[ TCP ]\u001b[0m";
  private readonly TcpListener _listener;
  private readonly string _ip;
  public readonly bool Listening;

  public Server(int port, string ip = "127.0.0.1") {
    _ip = ip;

    // Parse the IP and initialize the TCPListener
    var localAddr = IPAddress.Parse(_ip);
    _listener = new TcpListener(localAddr, port);

    // Log the server initialization options
    Console.WriteLine("{0} Server initialized on {1}:{2}", LogPrefix, localAddr, port);

    // Start the TCPListener
    _listener.Start();

    Listening = true;
  }

  public void AwaitMessage(Func<HtmlRequest, HtmlResponse> handler) {
    // Console.WriteLine("{0} Server awaiting incoming client", LogPrefix);
    var client = _listener.AcceptTcpClient(); // .ConfigureAwait(false);

    // Get the incoming data from the user
    // Console.WriteLine("{0} Client received, listening on input stream", LogPrefix);
    var receivedBuffer = new byte[client.ReceiveBufferSize];
    var message = "";

    using (var stream = client.GetStream()) {
      // read incoming `stream`
      var bytesRead = stream.Read(receivedBuffer, 0, client.ReceiveBufferSize);
      // convert the data received into a string, append it to `message`
      message += Encoding.ASCII.GetString(receivedBuffer, 0, bytesRead);

      // Create stream writer to write back to client
      var writer = new StreamWriter(stream);

      // Parse HTML data, if it fails, return 400 Bad Request
      var request = new HtmlRequest();
      if (request.Parse(message) != 0) {
        var badReq = CreateBadRequestResponse();
        writer.Write(badReq.Build());
        writer.Flush();
        return;
      }

      // Print the parsed request
      request.Print();

      // Send the request to `handler`, then send its response back to the client
      var response = handler(request);
      WriteBaseApiHeaders(response);

      var builtResponse = response.Build();
      // Console.WriteLine("{0} Sending response to client:\n{1}", LogPrefix, builtResponse);

      try {
        writer.Write(builtResponse);
        writer.Flush();
      }
      catch (IOException) { client.Close(); }
    }

    // Filter `EndRegex` to not flood the console
    // var messageFiltered = Regex.Replace(message, endRegex, replacer);

    // Write final log message and close the client connection
    // Console.WriteLine("{0} Transaction finished, closing client connection\n\tFinal message:\t{1}", LogPrefix, messageFiltered);
    client.Close();
  }

  private void WriteBaseApiHeaders(HtmlResponse response) {
    response.SetHeader("Server", _ip);
    response.SetHeader("Date", DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss UTC"));
    response.SetHeader("Cache-Control", "max-age=0, private, must-revalidate");
  }

  private HtmlResponse CreateBadRequestResponse() {
    var badReq = new HtmlResponse("{ \"statusCode\": 400 }", statusCode:400);
    WriteBaseApiHeaders(badReq);

    return badReq;
  }
}
