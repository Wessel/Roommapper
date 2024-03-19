namespace LibServer.Router;

using LibHttp;

public class Router(Dictionary<string, IRoute> routes) {
  private Dictionary<string, IRoute> Routes { get; } = routes;

  public HtmlResponse Handler(HtmlRequest request) {
    // Enumerable is considerably slower on small collections, that's why a `foreach` loop has been used.
    foreach (var route in Routes) {
      if (request.Route == route.Key) {
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
