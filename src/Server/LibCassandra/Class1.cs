using System;
using Cassandra;

namespace LibCassandra;

public class QueryBuilder {
  private readonly StringWriter _query = new StringWriter();

  public string Build() {
    return _query.ToString();
  }

  public QueryBuilder Select(string table, string[] columns) {
    _query.Write("SELECT ");
    if (columns.Length == 0) _query.Write("*");
    else _query.Write(string.Join(", ", columns));
    _query.Write(" FROM ");
    _query.Write(table);
    return this;
  }

  public QueryBuilder Where(string column, string value) {
    _query.Write(" WHERE ");
    _query.Write(column);
    _query.Write(" = ");
    _query.Write(value);
    return this;
  }

  public QueryBuilder And(string column, string value) {
    _query.Write(" AND ");
    _query.Write(column);
    _query.Write(" = ");
    _query.Write(value);
    return this;
  }

  public QueryBuilder Or(string column, string value) {
    _query.Write(" OR ");
    _query.Write(column);
    _query.Write(" = ");
    _query.Write(value);
    return this;
  }

  public QueryBuilder Limit(int limit) {
    _query.Write(" LIMIT ");
    _query.Write(limit);
    return this;
  }

  public QueryBuilder AllowFiltering() {
    _query.Write(" ALLOW FILTERING");
    return this;
  }

  public QueryBuilder CreateKeyspace(string name) {
    _query.Write("CREATE KEYSPACE ");
    _query.Write(name);
    return this;
  }

  public QueryBuilder WithReplication(string strategy, string replication) {
    _query.Write(" WITH REPLICATION = { 'class' : '");
    _query.Write(strategy);
    _query.Write("', 'replication_factor' : ");
    _query.Write(replication);
    _query.Write(" }");
    return this;
  }

  public QueryBuilder CreateTable(string name) {
    _query.Write("CREATE TABLE ");
    _query.Write(name);
    return this;
  }

  public QueryBuilder WithColumns(string[] columns) {
    _query.Write(" (");
    _query.Write(string.Join(", ", columns));
    _query.Write(")");
    return this;
  }

  public QueryBuilder PrimaryKey(string[] columns) {
    _query.Write(", PRIMARY KEY (");
    _query.Write(string.Join(", ", columns));
    _query.Write(")");
    return this;
  }

  public QueryBuilder WithOptions(string options) {
    _query.Write(" WITH ");
    _query.Write(options);
    return this;
  }

  public QueryBuilder DropKeyspace(string name) {
    _query.Write("DROP KEYSPACE ");
    _query.Write(name);
    return this;
  }

  public QueryBuilder DropTable(string name) {
    _query.Write("DROP TABLE ");
    _query.Write(name);
    return this;
  }

  public QueryBuilder IfExists() {
    _query.Write(" IF EXISTS");
    return this;
  }

  public QueryBuilder Update(string table) {
    _query.Write("UPDATE ");
    _query.Write(table);
    return this;
  }

  public QueryBuilder Set(string column, string value) {
    _query.Write(" SET ");
    _query.Write(column);
    _query.Write(" = ");
    _query.Write(value);
    return this;
  }

  public QueryBuilder Delete(string table) {
    _query.Write("DELETE FROM ");
    _query.Write(table);
    return this;
  }

  public QueryBuilder Truncate(string table) {
    _query.Write("TRUNCATE ");
    _query.Write(table);
    return this;
  }

  public QueryBuilder InsertInto(string table) {
    _query.Write("INSERT INTO ");
    _query.Write(table);
    return this;
  }

  public QueryBuilder Values(string[] columns, string[] values) {
    _query.Write(" (");
    _query.Write(string.Join(", ", columns));
    _query.Write(") VALUES (");
    _query.Write(string.Join(", ", values));
    _query.Write(")");
    return this;
  }

  public QueryBuilder Batch(string type) {
    _query.Write(type);
    _query.Write(" BATCH");
    return this;
  }

  public QueryBuilder UsingTimestamp(long timestamp) {
    _query.Write(" USING TIMESTAMP ");
    _query.Write(timestamp);
    return this;
  }
}

public struct ConnectionConfig {
  public string Host;
  public int Port;
  public string? Username;
  public string? Password;
  public string? ContactPoints;
  public string? WithCloudSecureConnectionBundle;
  public string? LoadBalancingPolicy;
}

public class Executer {
  private readonly Cluster _cluster;
  private ISession? _session;

  public Executer(ConnectionConfig config) {
    var builder = Cluster.Builder();

    if (!string.IsNullOrEmpty(config.Host))
      builder.AddContactPoint(config.Host);

    if (config.Port > 0)
      builder.WithPort(config.Port);

    if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
      builder.WithCredentials(config.Username, config.Password);

    if (!string.IsNullOrEmpty(config.ContactPoints))
      builder.AddContactPoints(config.ContactPoints.Split(','));

    if (!string.IsNullOrEmpty(config.WithCloudSecureConnectionBundle))
      builder.WithCloudSecureConnectionBundle(config.WithCloudSecureConnectionBundle);

    if (!string.IsNullOrEmpty(config.LoadBalancingPolicy))
      builder.WithLoadBalancingPolicy(new TokenAwarePolicy(new DCAwareRoundRobinPolicy(config.LoadBalancingPolicy)));

    _cluster = builder.Build();
  }

  public Executer Connect() {
    _session = _cluster.Connect();
    return this;
  }

  public void Execute(QueryBuilder query) {
    var rowset = _session?
      // .Execute("SELECT * FROM system_schema.keyspaces")
      .Execute(query.Build());
      // .Select(row => row.GetValue<string>("keyspace_name"));

    foreach (var name in rowset) {
      Console.WriteLine("- {0}", name);
    }
  }
}
