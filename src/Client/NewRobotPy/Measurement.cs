using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController
{
  internal class Measurement
  {
    private Coordinate location;
    private double angle;
    private double distance;

    public Measurement(Coordinate location, double angle, double distance)
    {
      this.location = location;
      this.angle = angle;
      this.distance = distance;
    }
  }
}
