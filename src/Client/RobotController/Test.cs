using System;

namespace RobotController
{
    class Program
    {
        static void Main(string[] args)
        {
            Motor motor = new Motor(1, 2, 3, 4);
            motor.drive(True, 200);
        }
    }
}
