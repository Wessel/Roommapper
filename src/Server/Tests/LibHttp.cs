namespace Tests;
using LibHttp;

public class LibHttpTests {
  private static readonly string Body = "Hello, World!";
  private static readonly Method Method = Method.Get;

  [Test]
  public void TestGet() {
    const string message = "GET /tests HTTP/1.1 200 OK\r\nContent-Length: 13\r\n\r\nHello, World!";
    var request = new HtmlRequest();
    request.Parse(message);
    Assert.Multiple(() => {
      Assert.That(Body, Is.EqualTo(request.Body));
      Assert.That(Method, Is.EqualTo(request.Method));
    });
  }

  [Test]
  public void TestSend() {
    var response = new HtmlResponse("Hello, World!");

    Assert.Multiple(() => {
      Assert.That(Body, Is.EqualTo(response.Body));
      Assert.That(response.Headers, Has.Count.EqualTo(1));
    });
  }
}
