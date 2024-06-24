using Cassandra;
using LibParse.Json;
using LibServer.Http;
using LibServer.Router;
using RobotControlServer.JsonClasses;

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
      // Implement path planning algorithm
      // var path;

      // Return success message with the path
      return new HttpResponse($"{{\"message\":\"success\",\"path\":\"{path}\"}}");
    } catch (Exception ex) {
      // Return error message if failed for any reason
      return new HttpResponse($"{{\"message\": \"{ex.Message.Replace("\"", "\\\"")}\"}}", 400);
    }
  }
}
