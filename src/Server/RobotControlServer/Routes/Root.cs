using LibParse.Json;
using LibServer.Http;
using LibServer.Router;

namespace RobotControlServer.Routes;

public class Root: IRoute {
  public HttpResponse Get(HttpRequest request) {
    var data = request.Body?.FromJson<object>();
    var response = new HttpResponse($"{{\"message\": \"you are at /\",\r\n{data?.ToJson()} }}");

    return response;
  }
}
