using System;
using System.Collections.Generic;
using System.Drawing;

public class CoveragePathPlanner
{
  private int _width;
  private int _height;
  private List<Cell> _cells;
  private List<Obstacle> _obstacles;

  public CoveragePathPlanner(int width, int height)
  {
    _width = width;
    _height = height;
    _cells = new List<Cell>();
    _obstacles = new List<Obstacle>();

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        _cells.Add(new Cell { X = x, Y = y, IsObstacle = false, IsVisited = false, Cost = double.MaxValue });
      }
    }
  }

  public void AddObstacleRectangle(int x, int y, int width, int height)
  {
    _obstacles.Add(new Obstacle { X = x, Y = y, Width = width, Height = height, IsRectangle = true });
  }

  public void AddObstacleCell(int x, int y)
  {
    _obstacles.Add(new Obstacle { X = x, Y = y, Width = 1, Height = 1, IsRectangle = false });
  }

  public List<Point> PlanPath()
  {
    // Add obstacles to the grid
    foreach (Obstacle obstacle in _obstacles)
    {
      foreach (Cell cell in _cells)
      {
        if (obstacle.IsRectangle)
        {
          if (cell.IsWithinObstacle(obstacle))
          {
            cell.IsObstacle = true;
          }
        }
        else
        {
          if (cell.X == obstacle.X && cell.Y == obstacle.Y)
          {
            cell.IsObstacle = true;
          }
        }
      }
    }

    // Initialize the start cell
    Cell startCell = _cells[0];
    startCell.Cost = 0;

    // Create a priority queue to hold the cells to be processed
    var queue = new PriorityQueue<Cell, double>();
    queue.Enqueue(startCell, 0);

    while (queue.Count > 0)
    {
      // Dequeue the cell with the lowest cost
      Cell currentCell = queue.Dequeue();

      // If the cell is the goal, construct the path
      if (currentCell.X == _width - 1 && currentCell.Y == _height - 1)
      {
        List<Point> path = new List<Point>();
        while (currentCell != null)
        {
          path.Add(new Point(currentCell.X, currentCell.Y));
          currentCell = currentCell.Parent!;
        }
        return path;
      }

      // Mark the cell as visited
      currentCell.IsVisited = true;

      // Explore the neighbors
      foreach (Cell neighbor in currentCell.GetNeighbors(_cells))
      {
        if (!neighbor.IsVisited && !neighbor.IsObstacle)
        {
          // Calculate the cost of the neighbor
          double cost = currentCell.Cost + 1;
          if (cost < neighbor.Cost)
          {
            neighbor.Cost = cost;
            neighbor.Parent = currentCell;
            queue.Enqueue(neighbor, cost + currentCell.Heuristic(_width, _height));
          }
        }
      }
    }

    // If no path is found, return an empty list
    return new List<Point>();
  }
}

public class Obstacle
{
  public int X { get; set; }
  public int Y { get; set; }
  public int Width { get; set; }
  public int Height { get; set; }
  public bool IsRectangle { get; set; }
}

public class Cell
{
  public int X { get; set; }
  public int Y { get; set; }
  public bool IsObstacle { get; set; }
  public bool IsVisited { get; set; }
  public double Cost { get; set; }
  public Cell? Parent { get; set; }

  public bool IsWithinObstacle(Obstacle obstacle)
  {
    return X >= obstacle.X && X < obstacle.X + obstacle.Width &&
           Y >= obstacle.Y && Y < obstacle.Y + obstacle.Height;
  }

  public IEnumerable<Cell> GetNeighbors(List<Cell> cells)
  {
    for (int x = -1; x <= 1; x++)
    {
      for (int y = -1; y <= 1; y++)
      {
        if (x == 0 && y == 0) continue;

        int neighborX = X + x;
        int neighborY = Y + y;

        if (neighborX >= 0 && neighborX < cells.Count / cells[0].Y && neighborY >= 0 && neighborY < cells[0].Y)
        {
          yield return cells[neighborX + neighborY * cells.Count / cells[0].Y];
        }
      }
    }
  }

  public double Heuristic(int width, int height)
  {
    // Manhattan distance heuristic
    return Math.Abs(X - (width - 1)) + Math.Abs(Y - (height - 1));
  }
}
