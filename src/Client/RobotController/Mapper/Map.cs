namespace RobotController
{
  internal class Map
  {
    private Coordinate bottomLeft, topRight;

    public Map(Coordinate bottomLeft, Coordinate topRight)
    {
      this.bottomLeft = bottomLeft;
      this.topRight = topRight;
    }
  }
}
