namespace RobotController
{
    class Program
    {
        static void Main(string[] args)
        {
            Motor motor = new Motor(1, 2, 3, 4);
            while (true) {

            motor.Drive(true, 200);
            }
        }
    }
}