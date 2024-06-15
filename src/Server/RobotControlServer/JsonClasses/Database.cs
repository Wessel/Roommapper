namespace RobotControlServer.JsonClasses;

// Data class for casting a JSON string into
public class Data {
  public string objects;
  public int version;
  public string date;
  public string id;
  public string name;
}

public class RowData {
  public string Id;
  public int[][] Objects;
  public int Version;
  public string Name;
  public DateTime Date;
}
