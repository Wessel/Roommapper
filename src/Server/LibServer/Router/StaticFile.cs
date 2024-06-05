namespace LibServer.Router;

using LibServer.Http;

class StaticFile(string file) : IRoute  {
  private string File { get; } = file;
  private string _mime = "text/html";
  public HttpResponse Get(HttpRequest req) {
    var response = new HttpResponse("");

    response.SetHeader("Content-Type", _mime);
    response.SendFile(File);
    return response;
  }

  public void InitHeaders() {
    switch (File.Split('.').Last())  {
      case "js": _mime = "application/javascript";  break;
      case "css": _mime = "text/css";  break;
      case "html": _mime = "text/html";  break;
      case "json": _mime = "application/json";  break;
    };
  }
}
