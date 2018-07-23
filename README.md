# RocketstationAtmospherics
A mod for Stationeers by Rocketwerkz, implement more physically accurate atmospherics devices.

Currently only 3 devices are implemented: VolumePump, Valve & PressureRegulator.

## VolumePump
Simulates a variable displacement piston pump, somewhat accurately cools the intake side and heats the output side and tracks the energy actually expended (if any) during pumping.

## Valve
Simulates a adjustable valve, can be set to allow bi- or unidirectional flow.  Simulates choked flow when the pressure differential across the valve would create supersonic conditions.

## Pressure Regulator
A derivation of the Valve simulation, unidirectional only and opens and closes itself to try and maintain set pressure in the input or output atmospheres.