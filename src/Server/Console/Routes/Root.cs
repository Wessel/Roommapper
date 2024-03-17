using LibParse.Json;

namespace Console.Routes;

using LibHttp;
using LibServer.Router;
using LibParse;

internal class RequestData {
  public int Id;
}

public class Root: IRoute {
  public HtmlResponse Get(HtmlRequest request) {
    // var data = request.Body?.FromJson<RequestData>();
    var data = request.Body?.FromJson<object>();
    var response = new HtmlResponse($"{{\"message\": \"you are at /\",\r\n{data?.ToJson()} }}");
    return response;
  }
}
