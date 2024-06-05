using NUnit.Framework;
using LibParse.Json;

namespace Tests;

internal class JsonClass(string name, int age) {
  public string Name => name;
  public int Age => age;

  private readonly string _result = $"{{\"Name\":\"{name}\",\"Age\":{age}}}";

  public bool Validate(string json) {
    return _result == json;
  }
}

public class LibParseTests {
  private static readonly string Name = "John";
  private static readonly int Age = 30;
  private static readonly string Json = $"{{\"name\":\"{Name}\",\"age\":{Age},\"car\":null}}";
  private readonly JsonClass _obj = new (Name, Age);

  [SetUp]
  public void Setup() { }

  [Test]
  public void ParseToAnonymous() {
    var obj = Json.FromJson<object>();

    Assert.Multiple(() => {
        Assert.That(Name, Is.EqualTo(((Dictionary<string,object>)obj!)["name"]));
        Assert.That(Age, Is.EqualTo(((Dictionary<string,object>)obj)["age"]));
      });
  }

  [Test]
  public void ClassToJson() {
    var json = _obj.ToJson();

    Assert.That(_obj.Validate(json), Is.EqualTo(true));
  }
}
