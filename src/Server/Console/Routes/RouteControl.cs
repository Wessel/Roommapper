using System.Net.Sockets;
using System.Text;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;
using Console.JsonClasses;

namespace Console.Routes;

public class RouteControl: IRoute {
  Dictionary<string, Dictionary<string, string>> _routes = new() {
    {
      "roomba", new() {
        {
          "start", "127.0.0.1:5000/api/v1/database/metadata"
        },
        {
          "stop", "127.0.0.1:5000/api/v1/database/metadata"
        },
        {
          "map", "127.0.0.1:5000/api/v1/database/metadata"
        }
      }
    }
  };
  public HttpResponse Post(HttpRequest request) {
    var data = request.Body?.FromJson<ControlRequest>();

    if (data?.task == null) return new HttpResponse("{\"message\": \"invalid command\"}", 400);

    switch (data.task) {
      case "start":
        SendHttpRequest(_routes["roomba"]["start"], "", "POST", "/start");
        return new HttpResponse("{\"message\": \"roomba started\"}");
      case "stop":
        SendHttpRequest(_routes["roomba"]["stop"], "", "POST", "/stop");
        return new HttpResponse("{\"message\": \"roomba stopped\"}");
      case "map":
        SendHttpRequest(_routes["roomba"]["map"], "", "POST", "/map");
        return new HttpResponse("{\"message\": \"roomba mapping\"}");
      default:
        return new HttpResponse("{\"message\": \"invalid command\"}", 400);
    }
  }
  public HttpResponse Get(HttpRequest request) {
    var data = request.Body?.FromJson<object>();
    var response = new HttpResponse($"{{\"message\": \"you are at /\" }}");

    return response;
  }

  public HttpResponse Options(HttpRequest request) {
    var response = new HttpResponse("{\"message\": \"options\"}");
    response.Headers.Add("Allow", "GET, POST, OPTIONS");
    return response;
  }

  // @todo(wessel): Rewrite, move to other class
  public void SendHttpRequest(string Host, string body, string method, string route) {
    using (TcpClient client = new TcpClient()) {
      string[] host = Host.Split(":");
      client.Connect(host[0], int.Parse(host[1].Split("/")[0]));

      using (NetworkStream stream = client.GetStream()) {
        string httpRequestMessage = $"{method} {route} HTTP/1.1\r\n" +
                                    $"Host: {Host}\r\n" +
                                    $"Content-Length: {body.Length}\r\n" +
                                    "\r\n" +
                                    $"{body}";

        byte[] httpRequestMessageBytes = Encoding.ASCII.GetBytes(httpRequestMessage);

        stream.Write(httpRequestMessageBytes, 0, httpRequestMessageBytes.Length);
        stream.Flush();
      }
    }
  }

}
