import RPi.GPIO as GPIO
from RpiMotorLib import RpiMotorLib
import time
import sys
import threading

# Motor pins
stepLeft = 22
dirLeft = 23
enableLeft = 24

stepRight = 25
dirRight = 26
enableRight = 27

def init():
    GPIO.setup(enableLeft, GPIO.OUT)
    GPIO.setup(enableRight, GPIO.OUT)

    motorLeft = RpiMotorLib.A4988Nema(dirLeft, stepLeft, (21,21,21), "DRV8825")
    motorRight = RpiMotorLib.A4988Nema(dirRight, stepRight, (21,21,21), "DRV8825")

def drive():
    GPIO.output(enableLeft, GPIO.LOW)
    motorLeft.motor_go(False, # True=Clockwise, False=Counter-Clockwise
                     "Full" , # Step type (Full,Half,1/4,1/8,1/16,1/32)
                     200, # number of steps
                     .0005, # step delay [sec]
                     False, # True = print verbose output 
                     .05) # initial delay [sec]
    GPIO.output(enableLeft, GPIO.HIGH)

    GPIO.output(enableRight, GPIO.LOW)
    motorRight.motor_go(False, # True=Clockwise, False=Counter-Clockwise
                     "Full" , # Step type (Full,Half,1/4,1/8,1/16,1/32)
                     200, # number of steps
                     .0005, # step delay [sec]
                     False, # True = print verbose output 
                     .05) # initial delay [sec]
    GPIO.output(enableRight, GPIO.HIGH)

def shutdown():
GPIO.cleanup() # clear GPIO allocations after run
    
if __name__ == '__main__':
    globals()[sys.argv[1]]()
