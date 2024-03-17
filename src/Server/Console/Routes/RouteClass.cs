using LibParse;

namespace Console.Routes;

using LibServer.Router;
using LibHttp;
using LibParse.Json;

public class RouteClass : IRoute {

  public HtmlResponse Get(HtmlRequest request) {
    // var data = request.Body?.FromJson<RequestData>();
    var response = new HtmlResponse("test\nyes");
    response.Body = response.ToJson();
    return response;
  }
}
