namespace RobotControlServer;

using Routes;
using LibServer;
using LibServer.Router;
using Cassandra;

internal static class Program {
  public static void Main() {
    var cassandraSession = InitializeDatabase();

    Dictionary<string, IRoute> routes = new() {
      { "", new Root() },
      { "database", new RouteDatabase(cassandraSession) },
      { "database/metadata", new RouteMetadata(cassandraSession) },
      { "database/path", new RoutePath(cassandraSession) },
      { "database/path/plan", new RoutePlan(cassandraSession) },
      { "roomba/control", new RouteControl() }
    };

    var tcpServer = new Server(5000);
    var router = new Router(routes, "api/v1");

    // EXPERIMENTAL! Current code is only meant to be used as REST API.
    // router.RouteDirectory("D:\\Documents\\[01] Development\\[00] HTML\\[00] portfolio\\v6");

    while (tcpServer.Listening) {
      tcpServer.AwaitMessage(router.Handler);
    }
  }

  private static ISession InitializeDatabase() {
    var cluster = Cluster.Builder()
      .AddContactPoints("127.0.0.1")
      .WithPort(9042)
      .Build();
    var session = cluster.Connect();

    session.Execute(@"CREATE KEYSPACE IF NOT EXISTS roommapper
      WITH REPLICATION = { 'class' : 'NetworkTopologyStrategy', 'datacenter1' : 1 };");
    session.Execute(@"
      CREATE TABLE IF NOT EXISTS Roommapper.Maps(
      Id uuid,
      Version int,
      Name text,
      Objects text,
      Date timestamp,
      PRIMARY KEY (Id,Name)
    )");

    session.Execute(@"
      CREATE TABLE IF NOT EXISTS Roommapper.Routes(
      Id uuid,
      Path text,
      PRIMARY KEY (Id)
    )");

    return session;
  }
}
