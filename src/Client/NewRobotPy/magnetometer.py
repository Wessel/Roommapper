import math
import py_qmc5883l
import time

# def calculate_angle(sensor):
#     """
#     Calculate the angle from 0 to 360 degrees using the QMC5883L magnetic sensor.

#     Args:
#         sensor (QMC5883L): An instance of the QMC5883L sensor.

#     Returns:
#         float: The calculated angle in degrees from 0 to 360.
#     """
#     # Get the raw magnetic sensor data
#     x, y, z = sensor.get_magnet_raw()
    
#     if x is None or y is None:
#         return None
    
#     # Calculate the angle in radians
#     angle_rad = math.atan2(y, x)
    
#     # Convert the angle to degrees
#     # angle_deg = math.degrees(angle_rad)
    
#     # Adjust the angle to the range of 0 to 360 degrees
#     # if angle_deg < 0:
#     #     angle_deg += 360
    
#     return angle_rad

# # Example usage
# if __name__ == "__main__":
    # sensor = py_qmc5883l.QMC5883L()
    
    # while True:
        
    #     angle = calculate_angle(sensor)
    #     print("Angle:", angle)
    #     time.sleep(0.1)

sensor = py_qmc5883l.QMC5883L(output_range=py_qmc5883l.RNG_8G)

while True:
    
    m = sensor.get_bearing()
    print(m)
    time.sleep(0.1)