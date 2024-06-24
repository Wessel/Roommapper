using Cassandra;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;
using Newtonsoft.Json;
using RobotControlServer.JsonClasses;
using System.Drawing;

namespace RobotControlServer.Routes;

public class RoutePlan: IRoute {
  /// <summary>
  ///  POST request for the database/path/plan route, generate a new path
  /// from a certain map of objects.
  /// </summary>
  public HttpResponse Post(HttpRequest request) {
    try {
      // Parse request body to Data object, give error if non-nullable fields
      // are null.
      var parsedBody = request.Body?.FromJson<Data>();
      if (parsedBody?.objects == null) {
        throw new Exception("objectData is null");
      }

      // ParsedBody?.objects = [[0,1],[0,2],...]

      // New instance of CPP with a 500x500 grid || where 0,0 = top-left and 499, 499 = bottom-right
      CoveragePathPlanner planner = new CoveragePathPlanner(500, 500);

      // Add obstacles cell by cell
      foreach (var obj in parsedBody?.objects)
      {
        planner.AddObstacleCell(obj[0], obj[1]);
      }

      List<Point> PlanPath = planner.PlanPath();
      // Don't know if this is needed here or just do List<Point> path = planner.PlanPath(obstacles);?
      string path = JsonConvert.SerializeObject(PlanPath);

      // Return success message with the path
      return new HttpResponse($"{{\"message\":\"success\",\"path\":\"{path}\"}}");
    } catch (Exception ex) {
      // Return error message if failed for any reason
      return new HttpResponse($"{{\"message\": \"{ex.Message.Replace("\"", "\\\"")}\"}}", 400);
    }
  }
}
