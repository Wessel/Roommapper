import math
import py_qmc5883l
import time


sensor = py_qmc5883l.QMC5883L(output_range=py_qmc5883l.RNG_8G)


m = sensor.get_bearing()
print(m)
