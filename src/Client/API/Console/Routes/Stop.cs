using LibServer.Http;
using LibServer.Router;

namespace Console.Routes;

public class StopRoute(): IRoute {

  public HttpResponse Get(HttpRequest request) {
    var response = new HttpResponse("StopRoute");
    return response;
  }
}
