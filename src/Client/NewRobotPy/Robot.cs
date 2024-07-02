using System.Data;

namespace RobotController
{
    public class Robot
    {
        private Coordinate currentLocation;
        private GyroSensor gyroSensor;
        private Motor leftMotor;
        private Motor rightMotor;

        public Robot(int x, int y, int gyroSCLPin, int gyroSDAPin, int leftMotorDirectionPin, int leftMotorStepPin, int rightMotorDirectionPin, int rightMotorStepPin)
        {
            currentLocation = new Coordinate(x, y);
            gyroSensor = new GyroSensor(gyroSCLPin, gyroSDAPin);
            leftMotor = new Motor(leftMotorDirectionPin, leftMotorStepPin);
            rightMotor = new Motor(rightMotorDirectionPin, rightMotorStepPin);
        }

        public void Drive(double distance, int direction)
        {
            // Simulate driving the robot
        }

        public void Turn(double degrees, int direction)
        {
            // Simulate turning the robot
        }


        public Measurement TakeMeasurement()
        {
            double angle = gyroSensor.GetAngle();
            double distance = 0; // Simulate reading from the distance sensor
            return new Measurement(currentLocation, angle, distance);
        }
    }
}
