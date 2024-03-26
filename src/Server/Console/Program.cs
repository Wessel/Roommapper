namespace Console;

using Routes;
using LibServer;
using LibServer.Router;

using LibCassandra;

internal static class Program {
  private static readonly Dictionary<string, IRoute> Routes = new() {
    { "/", new Root() },
    { "/routeclass", new RouteClass() }
  };

  public static void Main() {
    var tcpServer = new Server(5000);
    var router = new Router(Routes);

    var databaseConfig = new ConnectionConfig {
      Host = "127.0.0.1",
      Port = 9042
    };
    var databaseConnection = new Executer(databaseConfig).Connect();
    var query = new QueryBuilder()
      .Select("eve.stablediffusion", [ "*" ])
      .Where("id", "eed6b996-65dd-4920-a0a4-a11495ce8cb6");

    databaseConnection.Execute(query);


    // var cluster = Cluster.Builder()
    //   .AddContactPoints("127.0.0.1")
    //   .WithPort(9042)
    //   .Build();
    // var session = cluster.Connect();
    // Console.WriteLine("Connected to cluster: " + cluster.Metadata.ClusterName);
    //
    // var keyspaceNames = session
    //   .Execute("SELECT * FROM system_schema.keyspaces")
    //   .Select(row => row.GetValue<string>("keyspace_name"));
    //
    // foreach (var name in keyspaceNames) {
    //   Console.WriteLine("- {0}", name);
    // }

    // EXPERIMENTAL! Current code is only meant to be used as REST API.
    // router.RouteDirectory("D:\\Documents\\[01] Development\\[00] HTML\\[00] portfolio\\v6");

    while (tcpServer.Listening) {
      tcpServer.AwaitMessage(router.Handler);
    }
  }
}
