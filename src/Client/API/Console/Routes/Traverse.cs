using LibServer.Http;
using LibServer.Router;

namespace Console.Routes;

public class TraverseRoute(): IRoute {
  public HttpResponse Get(HttpRequest request) {
    var response = new HttpResponse("TraverseRoute");
    return response;
  }
}
