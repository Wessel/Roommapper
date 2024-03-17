namespace LibHttp;

public enum Mime {
  Json,
  Html,
  Text
};

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
