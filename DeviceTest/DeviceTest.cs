using System;
using System.Collections.Generic;
using RocketstationAtmospherics;
using Assets.Scripts.Atmospherics;

namespace DeviceTest
{
    public class Test
    {
        public Test(AtmosphericDevice device, Atmosphere inputAtmosphere, Atmosphere outputAtmosphere)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (inputAtmosphere == null) throw new ArgumentNullException("inputAtmosphere");
            if (outputAtmosphere == null) throw new ArgumentNullException("outputAtmosphere");

            this.device = device;
            this.InputAtmosphere = inputAtmosphere;
            this.OutputAtmosphere = outputAtmosphere;

            inputHistory.Add(new AtmosphereState(InputAtmosphere));
            outputHistory.Add(new AtmosphereState(OutputAtmosphere)); 

            device.InputAtmosphere = InputAtmosphere;
            device.OutputAtmosphere = OutputAtmosphere;
        }

        public Test(AtmosphericDevice device, string inputSpecifier, string outputSpecifier) : 
            this(device, AtmosphereBuilder.BuildAtmosphere(inputSpecifier), AtmosphereBuilder.BuildAtmosphere(outputSpecifier))
        { }

        public void Simulate(int ticks)
        {
            if (ticks <= 0) throw new ArgumentOutOfRangeException("ticks", "Argument must be greater then 0!");
            for (int i = 0; i < ticks; i++) Tick(); ;
        }

        public void Tick()
        {
            device.Tick();
            inputHistory.Add(new AtmosphereState(InputAtmosphere));
            outputHistory.Add(new AtmosphereState(OutputAtmosphere));
        }

        public List<AtmosphereState> inputHistory = new List<AtmosphereState>();
        public List<AtmosphereState> outputHistory = new List<AtmosphereState>();

        public Atmosphere InputAtmosphere, OutputAtmosphere;

        AtmosphericDevice device;
    }
}
