namespace Console;

using Routes;
using LibServer;
using LibServer.Router;

internal static class Program {
  private static readonly Dictionary<string, IRoute> Routes = new() {
    { "/", new Root() },
    { "/routeclass", new RouteClass() }
  };

  public static void Main() {
    var tcpServer = new Server(5000);
    var router = new Router(Routes);

    // EXPERIMENTAL! Current code is only meant to be used as REST API.
    // router.RouteDirectory("D:\\Documents\\[01] Development\\[00] HTML\\[00] portfolio\\v6");

    while (tcpServer.Listening) {
      tcpServer.AwaitMessage(router.Handler);
    }
  }
}
