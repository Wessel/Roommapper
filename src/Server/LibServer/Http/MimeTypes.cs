namespace LibServer.Http;

public enum Mime {
  Json,
  Html,
  Text
};

public enum Method {
  Get,
  Post,
  Put,
  Delete,
  Patch,
  Head,
  Options,
  Connect,
  Trace
};

public static class MethodClass {
  public static string MethodToString(Method method) {
    return method switch {
      Method.Get     => "GET",
      Method.Post    => "POST",
      Method.Put     => "PUT",
      Method.Delete  => "DELETE",
      Method.Patch   => "PATCH",
      Method.Head    => "HEAD",
      Method.Options => "OPTIONS",
      Method.Connect => "CONNECT",
      Method.Trace   => "TRACE",
      _               => "GET"
    };
  }

  public static Method StringToMethod(string method) {
    return method switch {
      "GET"     => Method.Get,
      "POST"    => Method.Post,
      "PUT"     => Method.Put,
      "DELETE"  => Method.Delete,
      "PATCH"   => Method.Patch,
      "HEAD"    => Method.Head,
      "OPTIONS" => Method.Options,
      "CONNECT" => Method.Connect,
      "TRACE"   => Method.Trace,
      _         => Method.Get
    };
  }
}

public static class MimeTypes {
  public static string MimeToHeader(Mime mime) {
    return mime switch {
      Mime.Json => "application/json",
      Mime.Html => "text/html",
      _         => "text/plain"
    };
  }

  public static Mime HeaderToMime(string header) {
    return header switch {
      "application/json" => Mime.Json,
      "text/html"        => Mime.Html,
      _                  => Mime.Text
    };
  }
}
