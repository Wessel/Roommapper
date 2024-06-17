using LibServer.Http;
using LibServer.Router;

namespace Console.Routes;

public class MapRoute(): IRoute {

  public HttpResponse Get(HttpRequest request) {
    var response = new HttpResponse("MapRoute");
    return response;
  }
}
