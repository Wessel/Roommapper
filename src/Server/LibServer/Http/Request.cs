namespace LibServer.Http;

public class HttpRequest {
  private const string LogPrefix = "\u001b[47m[ WWW ]\u001b[0m";

  public string? Route;
  private string? _httpVersion, _host, _header;
  public readonly Dictionary<string, string> QueryString = new();

  public Method? Method { get; private set; }
  public string? Body { get; private set; }

  public int Parse(string raw) {
    // _body starts after 2 new lines, cast to `_body`
    var body = raw.Split("\r\n\r\n");
    Body = body.Length > 1 ? string.Join("\r\n\r\n", body.Skip(1).ToArray()) : "";

    // Split the raw request into parts (one line per part)
    var parts = raw.Split("\r\n");
    if (parts.Length == 0) return -1;

    var isHeader = true;

    for (var i = 0; i < parts.Length; i++) {
      try {
        if (i == 0) {
          if (!parts[0].Contains("HTTP")) return -1;
          var first = parts[0].Split(" ");
          Method = MethodClass.StringToMethod(first[0].ToUpper());
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

    var querySplit = Route.Split("?");
    if (querySplit.Length == 1) return 0;

    Route = querySplit[0];
    var pairs = querySplit[1].Split('&');

    foreach (var pair in pairs) {
      var keyValue = pair.Split('=');
      if (keyValue.Length == 2) {
        QueryString.Add(keyValue[0], keyValue[1]);
      }
    }

    return 0;
  }

  public void Print() {
    Console.WriteLine("{0} _httpVersion:\t{1}", LogPrefix, _httpVersion);
    Console.WriteLine("{0} _host:\t\t{1}", LogPrefix, _host);
    Console.WriteLine("{0} Route:\t\t{1}", LogPrefix, Route);
    Console.WriteLine("{0} _method:\t\t{1}", LogPrefix, Method);
    Console.WriteLine("{0} _headers:\n{1}", LogPrefix, _header);
    Console.WriteLine("{0} _body:\n{1}", LogPrefix, Body);
  }
}
