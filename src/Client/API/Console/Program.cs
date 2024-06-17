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

    var tcpServer = new Server(5050);
    var router = new Router(routes, "api/v1");

    while (tcpServer.Listening) {
      tcpServer.AwaitMessage(router.Handler);
    }
  }
}
