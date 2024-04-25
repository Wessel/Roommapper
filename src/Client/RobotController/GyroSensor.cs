using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController
{
    internal class GyroSensor
    {
        private int SCLPin;
        private int SDAPin;

        public GyroSensor(int SCLPin, int SDAPin)
        {
            this.SCLPin = SCLPin;
            this.SDAPin = SDAPin;
        }

        public int GetAngle()
        {
            // Simulate reading from the sensor
            return 90;
        }
    }
}
