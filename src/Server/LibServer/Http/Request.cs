namespace LibServer.Http;

public class HttpRequest {
  private const string LogPrefix = "\u001b[47m[ WWW ]\u001b[0m";

  public string? Route;
  public string? _httpVersion, _host, _body, _header;
  public Method? _method;

  public Method? Method => _method;
  public string? Body => _body;

  public int Parse(string raw) {
    // _body starts after 2 new lines, cast to `_body`
    var body = raw.Split("\r\n\r\n");
    _body = body.Length > 1 ? string.Join("\r\n\r\n", body.Skip(1).ToArray()) : "";

    // Split the raw request into parts (one line per part)
    var parts = raw.Split("\r\n");
    if (parts.Length == 0) return -1;

    var isHeader = true;

    for (var i = 0; i < parts.Length; i++) {
      try {
        if (i == 0) {
          if (!parts[0].Contains("HTTP")) return -1;
          var first = parts[0].Split(" ");
          _method = MethodClass.StringToMethod(first[0].ToUpper());
          Route = first[1];
          _httpVersion = first[2].Split("/")[1];
        } else {
          var components = parts[i].Split(": ");

          switch (components[0]) {
            case "_host":
              _host = components[1];
              break;
            default:
              if (parts[i] == "") isHeader = false;
              else if (isHeader) _header += parts[i] + "\r\n";
              break;
          }
        }
      }
      catch { return -1; }
    }

    return 0;
  }

  public void Print() {
    Console.WriteLine("{0} _httpVersion:\t{1}", LogPrefix, _httpVersion);
    Console.WriteLine("{0} _host:\t\t{1}", LogPrefix, _host);
    Console.WriteLine("{0} Route:\t\t{1}", LogPrefix, Route);
    Console.WriteLine("{0} _method:\t\t{1}", LogPrefix, _method);
    Console.WriteLine("{0} _headers:\n{1}", LogPrefix, _header);
    Console.WriteLine("{0} _body:\n{1}", LogPrefix, _body);
  }
}
