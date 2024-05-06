namespace Console;

using Routes;
using LibServer;
using LibServer.Router;

internal static class Program {
  public static void Main() {
    Dictionary<string, IRoute> routes = new() {
      { "", new Root() },
      { "map", new MapRoute() },
      { "traverse", new TraverseRoute() },
      { "stop", new StopRoute() },
      { "home", new HomeRoute() },
    };

    var tcpServer = new Server(5000);
    var router = new Router(routes, "api/v1");

    // EXPERIMENTAL! Current code is only meant to be used as REST API.
    // router.RouteDirectory("D:\\Documents\\[01] Development\\[00] HTML\\[00] portfolio\\v6");

    while (tcpServer.Listening) {
      tcpServer.AwaitMessage(router.Handler);
    }
  }
}
