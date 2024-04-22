using System;

namespace Client.Ultrasonic.Compass
{
    class Compass
    {
        private const int SCLPin;
        private const int SDAPin;

        public Compass(int SCLPin, int SDAPin)
        {
            this.SCLPin = SCLPin;
            this.SDAPin = SDAPin;
        }
        
        public int getAzimuth(){
            return 0;
        }

    }
}
