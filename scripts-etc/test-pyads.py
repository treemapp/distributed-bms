import pyads

plc = pyads.Connection(
    "192.168.0.208.1.1",
    800,
    "192.168.0.208"
)

print("Before open")
plc.open()
print(plc._port)
print(plc.ip_address)
print("After open")
print("is_open =", plc.is_open)

import time
time.sleep(10)

print("Still alive?")
print("is_open =", plc.is_open)

plc.close()
print("Closed")