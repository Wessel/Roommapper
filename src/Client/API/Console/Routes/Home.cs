using LibServer.Http;
using LibServer.Router;

namespace Console.Routes;

public class HomeRoute(): IRoute {

  public HttpResponse Get(HttpRequest request) {
    var response = new HttpResponse("HomeRoute");
    return response;
  }
}
