namespace LibServer.Router;

using LibHttp;

public interface IRoute  {
  public  HtmlResponse Get(HtmlRequest _) { return MethodNotAllowed(); }
  public  HtmlResponse Post(HtmlRequest _) { return MethodNotAllowed(); }
  public  HtmlResponse Put(HtmlRequest _) { return MethodNotAllowed(); }
  public  HtmlResponse Patch(HtmlRequest _) { return MethodNotAllowed(); }
  public  HtmlResponse Delete(HtmlRequest _) { return MethodNotAllowed(); }
  public  HtmlResponse Head(HtmlRequest _) { return MethodNotAllowed(); }
  public  HtmlResponse Options(HtmlRequest _) { return MethodNotAllowed(); }

  public  HtmlResponse MethodNotAllowed() {
    var response = new HtmlResponse("{ \"message\": \"Method Not Allowed\"}", statusCode:405);
    response.SetHeader("Content-Type", "application/json");
    return response;
  }
}
