using System;
using System.Collections.Generic;
using System.Drawing;

class Program
{
  static void Main(string[] args)
  {
    // Create a 20x20 grid
    CoveragePathPlanner planner = new CoveragePathPlanner(20, 20);

    // Add some obstacles to the grid
    List<Point> obstacles = new List<Point>();

    // Add a 2x2 obstacle at (4, 4)
    for (int x = 4; x < 6; x++)
    {
      for (int y = 4; y < 6; y++)
      {
        obstacles.Add(new Point(x, y));
      }
    }

    // Add a 3x3 obstacle at (10, 10)
    for (int x = 10; x < 13; x++)
    {
      for (int y = 10; y < 13; y++)
      {
        obstacles.Add(new Point(x, y));
      }
    }

    // Plan the path
    List<Point> path = planner.PlanPath(obstacles);

    // Print the grid
    for (int y = 0; y < 20; y++)
    {
      for (int x = 0; x < 20; x++)
      {
        if (obstacles.Contains(new Point(x, y)))
        {
          Console.Write("X "); // Obstacle
        }
        else if (path.Contains(new Point(x, y)))
        {
          Console.Write("P "); // Path
        }
        else
        {
          Console.Write(". "); // Empty cell
        }
      }
      Console.WriteLine();
    }

    // Print the path
    Console.WriteLine("Path:");
    foreach (Point point in path)
    {
      Console.WriteLine($"({point.X}, {point.Y})");
    }

    Console.ReadLine();
  }
}
