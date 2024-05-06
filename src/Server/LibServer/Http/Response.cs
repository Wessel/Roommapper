namespace LibServer.Http;


public class HttpResponse {
  private static readonly Dictionary<int, string> StatusCodes = new() {
    {200, "200 Success"},
    {403, "403 Forbidden"},
    {404, "404 Not Found"},
    {405, "405 Method Not Allowed"},
    {500, "500 Internal Server Error"},
    {501, "501 Not Implemented"},
    {502, "502 Bad Gateway"}
  };

  private string _httpVersion = "1.1";
  private string _status, _body;
  private readonly Dictionary<string, string> _header;

  public string HttpVersion {
    get => _httpVersion;
    set => _httpVersion = value;
  }

  public string Body {
    get => _body;
    set => _body = value;
  }

  public Dictionary<string, string> Headers {
    get => _header;
  }

  public HttpResponse(string body, int statusCode = 200, Mime mime = Mime.Json) {
    _body = body;
    _header = new Dictionary<string, string>();
    _status = StatusToString(statusCode);

    if (SetHeader("Content-Type", MimeTypes.MimeToHeader(mime)) != 0) {
      throw new Exception($"Failed to set `mime` header of value `{mime}`");
    }
  }

  private static string StatusToString(int statusCode) {
    return StatusCodes.GetValueOrDefault(statusCode, "400 Bad Request");
  }

  public int SetHeader(string key, string value) {
    if (_header.TryAdd(key, value)) return 0;

    return -1;
  }

  public void SendFile(string path) {
    try {
      _body = File.ReadAllText(path);
    } catch (Exception e) {
      _status = StatusToString(500);
      Console.WriteLine(e.Message);
    }
  }

  public string Build(string linebreak = "\r\n") {
    return String.Join(linebreak,
    [
      $"HTTP/{_httpVersion} {_status}",
      $"Content-Length: {_body.Length}",
      .._header
        .Select(pair => $"{pair.Key}:{pair.Value}")
        .ToArray(),
      "",
      _body
    ]);
  }
}
