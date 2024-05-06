namespace LibServer.Router;

using LibServer.Http;

public interface IRoute  {
  public  HttpResponse Get(HttpRequest _) { return MethodNotAllowed(); }
  public  HttpResponse Post(HttpRequest _) { return MethodNotAllowed(); }
  public  HttpResponse Put(HttpRequest _) { return MethodNotAllowed(); }
  public  HttpResponse Patch(HttpRequest _) { return MethodNotAllowed(); }
  public  HttpResponse Delete(HttpRequest _) { return MethodNotAllowed(); }
  public  HttpResponse Head(HttpRequest _) { return MethodNotAllowed(); }
  public  HttpResponse Options(HttpRequest _) { return MethodNotAllowed(); }

  public  HttpResponse MethodNotAllowed() {
    var response = new HttpResponse("{ \"message\": \"Method Not Allowed\"}", statusCode:405);
    response.SetHeader("Content-Type", "application/json");
    return response;
  }
}
