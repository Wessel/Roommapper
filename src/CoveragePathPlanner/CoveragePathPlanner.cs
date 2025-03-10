using System;
using System.Collections.Generic;
using System.Drawing;

public class CoveragePathPlanner
{
  private int _width;
  private int _height;
  private List<Cell> _cells;

  public CoveragePathPlanner(int width, int height)
  {
    _width = width;
    _height = height;
    _cells = new List<Cell>();

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        _cells.Add(new Cell { X = x, Y = y, IsObstacle = false, IsVisited = false, Cost = double.MaxValue });
      }
    }
  }

  public List<Point> PlanPath(List<Point> obstacles)
  {
    // Add obstacles to the grid
    foreach (Point obstacle in obstacles)
    {
      _cells[obstacle.X + obstacle.Y * _width].IsObstacle = true;
    }

    // Initialize the start cell
    Cell startCell = _cells[0];
    startCell.Cost = 0;

    // Create a priority queue to hold the cells to be processed
    var queue = new PriorityQueue<Cell, double>();
    queue.Enqueue(startCell, 0);

    List<Point> path = new List<Point>();

    while (queue.Count > 0)
    {
      // Dequeue the cell with the lowest cost
      Cell currentCell = queue.Dequeue();

      // If the cell has not been visited before, add it to the path
      if (!currentCell.IsVisited)
      {
        path.Add(new Point(currentCell.X, currentCell.Y));
      }

      // Mark the cell as visited
      currentCell.IsVisited = true;

      // Explore the neighbors
      foreach (Cell neighbor in GetNeighbors(currentCell))
      {
        if (!neighbor.IsVisited && !neighbor.IsObstacle)
        {
          // Calculate the cost of the neighbor
          double cost = currentCell.Cost + 1;
          if (cost < neighbor.Cost)
          {
            neighbor.Cost = cost;
            neighbor.Parent = currentCell;
            queue.Enqueue(neighbor, cost);
          }
        }
      }
    }

    return path;
  }

  private List<Cell> GetNeighbors(Cell cell)
  {
    List<Cell> neighbors = new List<Cell>();

    for (int x = -1; x <= 1; x++)
    {
      for (int y = -1; y <= 1; y++)
      {
        if (x == 0 && y == 0) continue;

        int neighborX = cell.X + x;
        int neighborY = cell.Y + y;

        if (neighborX >= 0 && neighborX < _width && neighborY >= 0 && neighborY < _height)
        {
          neighbors.Add(_cells[neighborX + neighborY * _width]);
        }
      }
    }

    return neighbors;
  }
}

public class Cell
{
  public int X { get; set; }
  public int Y { get; set; }
  public bool IsObstacle { get; set; }
  public bool IsVisited { get; set; }
  public double Cost { get; set; }
  public Cell Parent { get; set; }
}
