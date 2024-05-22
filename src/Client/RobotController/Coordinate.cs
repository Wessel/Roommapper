using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController
{
    public class Coordinate
    {
        private int x;
        private int y;
        private Boolean accesible;
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
