namespace LibServer.Router;

using Http;
using System.Text.RegularExpressions;

public class Router(Dictionary<string, IRoute> routes, string baseURI) {
  private Dictionary<string, IRoute> Routes { get; } = routes;

  public HttpResponse Handler(HttpRequest request) {
    // Enumerable is considerably slower on small collections, that's why a `foreach` loop has been used.
    foreach (var route in Routes) {
      var routePath = route.Key.Length > 0 ? $"/{baseURI}/{route.Key}" : $"/{baseURI}";
      if (routePath == Regex.Replace(request.Route, @"\/+$", "")) {
        return request.Method switch {
          Method.Get     => route.Value.Get(request),
          Method.Post    => route.Value.Post(request),
          Method.Put     => route.Value.Put(request),
          Method.Patch   => route.Value.Patch(request),
          Method.Delete  => route.Value.Delete(request),
          Method.Head    => route.Value.Head(request),
          Method.Options => route.Value.Options(request),
          _         => route.Value.MethodNotAllowed()
        };
      }
    }

    return new HttpResponse("{ \"message\": \"Not Found\" }", statusCode:404);
  }

  // EXPERIMENTAL!
  public void RouteDirectoryRecursive(string directory) {
    List<string> collectedDirectories = [];

    RecursiveDirectories(directory, collectedDirectories).ForEach(f => {
      var location = f.Replace(directory, "").Replace("\\", "/");
      var fileRoute = new StaticFile(f);
      fileRoute.InitHeaders();

      Console.WriteLine($"Adding route: {location} to {f}");
      Routes.Add(location, fileRoute);
    });
  }

  private static List<string> RecursiveDirectories(string directory, List<string> collectedDirectories) {
    try {
      Directory.GetDirectories(directory).ToList().ForEach(d => {
        Directory.GetFiles(d).ToList().ForEach(collectedDirectories.Add);
        RecursiveDirectories(d, collectedDirectories);
      });
    } catch (System.Exception e) {
      Console.WriteLine(e.Message);
    }

    return collectedDirectories;
  }
}
