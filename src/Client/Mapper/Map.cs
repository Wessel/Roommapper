using RobotController;

namespace Mapper
{
    public class Map
    {
      private Coordinate bottomLeft, bottomRight;

      public Map(Coordinate bottomLeft, Coordinate bottomRight)
      {
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
      }
    }
}
