using RobotController;
using System.Drawing;

namespace Mapper
{
    public class Map
    {
      private Coordinate bottomLeft, bottomRight;

      public Map(Coordinate bottomLeft, Coordinate bottomRight)
      {
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
        Console.Write(testPathPlanner);
      }

    public List<Point> testPathPlanner()
    {
      CoveragePathPlanner planner = new CoveragePathPlanner(500, 500);

      List<Obstacle> obstacles = new List<Obstacle>
      {
        new Obstacle { X = 100, Y = 100, Width = 50, Height = 50 },
        new Obstacle { X = 200, Y = 200, Width = 100, Height = 100 },
        new Obstacle { X = 300, Y = 300, Width = 150, Height = 150 }
      };

      return planner.PlanPath(obstacles);
    }

  }
}
