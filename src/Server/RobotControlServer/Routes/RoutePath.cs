using Cassandra;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;
using RobotControlServer.JsonClasses;

namespace RobotControlServer.Routes;

public class RoutePath(ISession cassandraSession): IRoute {
  /// <summary>
  ///  GET request for the database/path route, return a path of a map based on given criteria.
  /// </summary>
  public HttpResponse Get(HttpRequest request) {
    try {
      // Prepare the select statement based on the given criteria
      // If no criteria is given, throw an exception.
      // Using a prepare due to the potential of SQL injection when accepting
      // any form of user data.
      var queries = new Dictionary<string, Func<string, BoundStatement>> {
        ["id"] = id => cassandraSession.Prepare(@"SELECT * FROM Roommapper.Routes WHERE Id = ?;").Bind(Guid.Parse(id)),
        ["name"] = name => cassandraSession.Prepare(@"SELECT * FROM Roommapper.Routes WHERE Id = ?;").Bind(name)
      };

      var selectStatement = queries
        .Where(query => request.QueryString.ContainsKey(query.Key))
        .Select(query => query.Value(request.QueryString[query.Key]))
        .FirstOrDefault();

      if (selectStatement == null) throw new Exception("No valid search criteria given, give one of (id, name).");

      // Execute the query and return all rows in an array
      var rowSet = cassandraSession.Execute(selectStatement);

      // Create a list to hold the row data, convert it to a JSON string
      var rows = RowsToString(rowSet);

      // Return the JSON string to the user
      return new HttpResponse(rows);
    } catch (Exception ex) {
      // Return error message if failed for any reason
      return new HttpResponse($"{{\"message\": \"{ex.Message.Replace("\"", "\\\"")}\"}}", 400);
    }
  }

  /// <summary>
  ///  POST request for the database/path route, insert a new path containing
  ///  routing data into the database.
  /// </summary>
  public HttpResponse Post(HttpRequest request) {
    try {
      // Parse request body to Data object, give error if non-nullable fields
      // are null.
      var parsedBody = request.Body?.FromJson<Data>();
      if (parsedBody?.objects == null) {
        throw new Exception("objectData is null");
      }

      if (parsedBody?.id == null) {
        throw new Exception("id is null");
      }

      var selectStatement =
        parsedBody?.id != null ? cassandraSession.Prepare(@"SELECT * FROM Roommapper.Maps WHERE Id = ?;")
            .Bind(Guid.Parse(parsedBody.id)) :
          throw new Exception("No valid search criteria given, give one of (id).");

      // Execute the query and return all rows in an array
      var rowSet = cassandraSession.Execute(selectStatement);

      if (rowSet.IsExhausted()) {
        throw new Exception("No matching map found with the given criteria.");
      }

      // Prepare the insert statement, bind the values, and execute the query
      // Using a prepare due to the potential of SQL injection when accepting
      // any form of user data.
      var uuid = Guid.Parse(parsedBody.id);

      var insertStatement = cassandraSession.Prepare(@"
        INSERT INTO Roommapper.Routes(Id, path)
        VALUES (?, ?);
      ").Bind(uuid, parsedBody.objects);

      cassandraSession.Execute(insertStatement);

      // Return success message with the UUID of the inserted row
      return new HttpResponse($"{{\"message\":\"success\",\"id\":\"{uuid}\"}}");
    }
    catch (Exception ex) {
      // Return error message if failed for any reason
      return new HttpResponse($"{{\"message\": \"{ex.Message.Replace("\"", "\\\"")}\"}}", 400);
    }
  }

  /// <summary>
  /// Deletes a given row
  /// </summary>
  public HttpResponse Delete(HttpRequest request) {
    try {
      var parsedBody = request.Body?.FromJson<Data>();

      // Prepare the select statement based on the given criteria
      // If no criteria is given, throw an exception.
      // Using a prepare due to the potential of SQL injection when accepting
      // any form of user data.
      var selectStatement =
        parsedBody?.id != null ? cassandraSession.Prepare(@"SELECT * FROM Roommapper.Routes WHERE Id = ?;")
          .Bind(Guid.Parse(parsedBody.id)) :
        throw new Exception("No valid search criteria given, give one of (id).");

      // Execute the query and return all rows in an array
      var rowSet = cassandraSession.Execute(selectStatement);

      if (rowSet.IsExhausted()) {
        throw new Exception("No rows found with the given criteria.");
      }

      // Prepare and execute delete statement
      var deleteStatement = cassandraSession.Prepare(@"DELETE FROM Roommapper.Routes WHERE Id = ?;")
        .Bind(Guid.Parse(parsedBody.id));

      cassandraSession.Execute(deleteStatement);

      return new HttpResponse($"{{\"message\":\"success\",\"entry\":{RowsToString(rowSet)}}}");

    } catch (Exception ex) {
      return new HttpResponse($"{{\"message\": \"{ex.Message.Replace("\"", "\\\"")}\"}}", 400);
    }
  }

  /// <summary>
  /// Casts a set of `RowSet` into a valid JSON string.
  /// </summary>
  /// <param name="rowSet">The `RowSet` to cast.</param>
  /// <returns>A valid JSON string formed from `rowSet`.</returns>
  private string RowsToString(RowSet rowSet) {
    return rowSet.Select(row => new RowData {
      Id = row.GetValue<Guid>("id").ToString(),
      Objects = row.GetValue<string>("path").FromJson<int[][]>(),
    }).ToList().ToJson();
  }
}
