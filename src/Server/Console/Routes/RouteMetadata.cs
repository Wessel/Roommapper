using Cassandra;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;

namespace Console.Routes;

public class RouteMetadata(ISession cassandraSession): IRoute {
  public HttpResponse Get(HttpRequest request) {
    var response = new HttpResponse(cassandraSession.ToJson());
    return response;
  }
}
