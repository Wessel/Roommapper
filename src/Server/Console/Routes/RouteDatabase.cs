using Cassandra;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;

namespace Console.Routes;

public class RouteDatabase(ISession cassandraSession): IRoute {

  public HttpResponse Get(HttpRequest request) {
    Dictionary<string, string> resData = new() {
      // { "ClusterName", cluster.Metadata.ClusterName }
    };
    var response = new HttpResponse(cassandraSession.ToJson());
    return response;
  }

  public HttpResponse Post(HttpRequest request) {
    var response = new HttpResponse("{\"message\": \"you are at /data\"}");
    return response;
  }
}
