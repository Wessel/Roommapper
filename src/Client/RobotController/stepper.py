import RPi.GPIO as GPIO
from RpiMotorLib import RpiMotorLib
import time
import sys

direction= 23 # Direction (DIR) GPIO Pin
step = 22 # Step GPIO Pin

def drive():
    mymotortest = RpiMotorLib.A4988Nema(direction, step, (21,21,21), "DRV8825")

    mymotortest.motor_go(False, "Full", 200, 0.0005, False, .05)

    GPIO.cleanup()
    
if __name__ == '__main__':
    globals()[sys.argv[1]]()