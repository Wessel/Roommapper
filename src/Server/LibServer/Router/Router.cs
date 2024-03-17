namespace LibServer.Router;

using LibHttp;

public class Router(Dictionary<string, IRoute> routes) {
  private Dictionary<string, IRoute> Routes { get; } = routes;

  public HtmlResponse Handler(HtmlRequest request) {
    // Enumerable is considerably slower on small collections, that's why a `foreach` loop has been used.
    foreach (var route in Routes) {
      if (request.Route == route.Key) {
        return request.Method switch {
          "GET"     => route.Value.Get(request),
          "POST"    => route.Value.Post(request),
          "PUT"     => route.Value.Put(request),
          "PATCH"   => route.Value.Patch(request),
          "DELETE"  => route.Value.Delete(request),
          "HEAD"    => route.Value.Head(request),
          "OPTIONS" => route.Value.Options(request),
          _         => route.Value.MethodNotAllowed()
        };
      }
    }

    return new HtmlResponse("{ \"message\": \"Not Found\" }", statusCode:404);
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
