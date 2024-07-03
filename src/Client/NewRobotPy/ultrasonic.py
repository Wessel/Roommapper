import serial
import time

def read_distance(serial_port):
    # Open serial port
    ser = serial.Serial(serial_port, 9600, timeout=1)
    time.sleep(2)  # Wait for the serial connection to initialize
    
    # Command to read distance
    read_distance_cmd = [0x22, 0x00, 0x00, 0x22]
    
    # Send the command
    ser.write(bytearray(read_distance_cmd))
    
    # Read the response
    response = ser.read(4)
    
    # Close serial port
    ser.close()
    
    if len(response) == 4:
        distance = (response[1] << 8) + response[2]
        return distance
    else:
        return None

if __name__ == "__main__":
    port = '/dev/serial0'  # Replace with your actual serial port
    

            
    distance = read_distance(port)
    print(distance)