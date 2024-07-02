import RPi.GPIO as GPIO
from RpiMotorLib import RpiMotorLib
import time
import sys
import threading
import argparse

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

direction1 = 23
step1 = 22
enable1 = 24

direction2 = 14
step2 = 15
enable2 = 18

GPIO.setup(enable1, GPIO.OUT, initial=GPIO.HIGH)
GPIO.setup(enable2, GPIO.OUT, initial=GPIO.HIGH)
GPIO.output(enable1, GPIO.HIGH)
GPIO.output(enable2, GPIO.HIGH)

def run_motor1(direction, steps):
    direction = not direction
    motor1 = RpiMotorLib.A4988Nema(direction1, step1, (21, 21, 21), "DRV8825")
    motor1.motor_go(direction, "Full", steps, 0.0010, False, .05)

def run_motor2(direction, steps):
    motor2 = RpiMotorLib.A4988Nema(direction2, step2, (21, 21, 21), "DRV8825")
    motor2.motor_go(direction, "Full", steps, 0.0010, False, .05)

def drive(direction, steps):
    
    if direction == "b":
        direction = True
    elif direction == "f":
        direction = False
    else:
        print("Invalid direction")
        return
    
    try:
        steps = int(steps)
    except ValueError:
        print("Invalid steps")
        return
    
    GPIO.output(enable1, GPIO.LOW)
    GPIO.output(enable2, GPIO.LOW)

    # Create threads for each motor
    thread1 = threading.Thread(target=run_motor1, args=(direction, steps))
    thread2 = threading.Thread(target=run_motor2, args=(direction, steps))

    # Start the threads
    thread1.start()
    thread2.start()

    # Wait for both threads to complete
    thread1.join()
    thread2.join()

    GPIO.output(enable1, GPIO.HIGH)
    GPIO.output(enable2, GPIO.HIGH)

    GPIO.cleanup()
    print("Driven succesfully")
    
def turn(direction, steps):
    
    if direction == "l":
        direction = True
    elif direction == "r":
        direction = False
    else:
        print("Invalid direction")
        return
    
    try:
        steps = int(steps)
    except ValueError:
        print("Invalid steps")
        return
    
    GPIO.output(enable1, GPIO.LOW)
    GPIO.output(enable2, GPIO.LOW)
    
    # Create threads for each motor
    if direction:
        thread1 = threading.Thread(target=run_motor1, args=(not direction, steps))
        thread2 = threading.Thread(target=run_motor2, args=(direction, steps))
    else:
        thread1 = threading.Thread(target=run_motor1, args=(direction, steps))
        thread2 = threading.Thread(target=run_motor2, args=(not direction, steps))

    # Start the threads
    thread1.start()
    thread2.start()

    # Wait for both threads to complete
    thread1.join()
    thread2.join()

    GPIO.output(enable1, GPIO.HIGH)
    GPIO.output(enable2, GPIO.HIGH)

    GPIO.cleanup()
    

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Call a specific function with arguments")
    parser.add_argument('function_name', type=str, help="Name of the function to call")
    parser.add_argument('args', nargs=argparse.REMAINDER, help="Arguments for the function")
    
    args = parser.parse_args()
    
    if args.function_name == "drive":
        if len(args.args) != 2:
            print("function takes two arguments!")
        else:
            drive(args.args[0], args.args[1])
    elif args.function_name == "turn":
        if len(args.args) != 2:
            print("function takes two arguments!")
        else:
            turn(args.args[0], args.args[1])
            
