using Cassandra;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;
using System.Drawing;

namespace RobotControlServer.Routes;

public class RoutePlan(ISession cassandraSession): IRoute {
  /// <summary>
  ///  POST request for the database/path/plan route, generate a new path
  /// from a certain map of objects.
  /// </summary>
  public HttpResponse Get(HttpRequest request) {
    try {
      // Parse request body to Data object, give error if non-nullable fields
      // are null.
      string? id;
      if (!request.QueryString.TryGetValue("id", out id)) {
        throw new Exception("id is null");
      }


      var selectStatement =
        cassandraSession.Prepare(@"SELECT * FROM Roommapper.Maps WHERE Id = ?;").Bind(Guid.Parse(id));
      var rowSet = cassandraSession.Execute(selectStatement);

      if (rowSet.IsExhausted()) {
        throw new Exception("No map found with the given id");
      }

      var map = rowSet.First();

      // New instance of CPP with a 500x500 grid || where 0,0 = top-left and 499, 499 = bottom-right
      CoveragePathPlanner planner = new CoveragePathPlanner(500, 500);

      // Create obstacle cell list
      List<Point> obstacles = new List<Point>();

      // Add obstacle cells
      foreach (var obj in $"[{map.GetValue<string>("objects")}]".FromJson<int[][]>()) {
        obstacles.Add(new Point(obj[0], obj[1])); // Get x and y for every coordinate in the parsedbody array
      }

      List<Point> PlanPath = planner.PlanPath(obstacles);
      List<List<int>> points = new List<List<int>>();
      foreach(var point in PlanPath) {
        // push [x,y] to points
        points.Add(new List<int> {point.X, point.Y});
      }

      // Return success message with the path

      var insertStatement = cassandraSession.Prepare(@"
        INSERT INTO Roommapper.Routes(Id, path)
        VALUES (?, ?);
      ").Bind(Guid.Parse(id), points.ToJson());

      cassandraSession.Execute(insertStatement);

      // Return success message with the UUID of the inserted row
      return new HttpResponse($"{{\"message\":\"success\",\"id\":\"{id}\"}}");
      // return new HttpResponse($"{{\"message\":\"success\",\"path\":{points.ToJson()}}}");
    } catch (Exception ex) {
      // Return error message if failed for any reason
      return new HttpResponse($"{{\"message\": \"{ex.Message.Replace("\"", "\\\"")}\"}}", 400);
    }
  }

  public HttpResponse Options(HttpRequest request) {
    var response = new HttpResponse("{\"message\": \"options\"}");
    response.Headers.Add("Allow", "GET, POST, OPTIONS");
    return response;
  }
}
