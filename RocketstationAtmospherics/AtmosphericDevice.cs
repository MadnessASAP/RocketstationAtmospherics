using System;
using Assets.Scripts.Atmospherics;

namespace RocketstationAtmospherics
{
    public class AtmosphericDevice
    {
        public AtmosphericDevice(Atmosphere inputAtmosphere = null, Atmosphere outputAtmosphere = null)
        {
            InputAtmosphere = inputAtmosphere;
            OutputAtmosphere = outputAtmosphere;
        }

        public AtmosphericDevice() : this(null, null) { }

        public virtual void Tick() { }

        public Atmosphere InputAtmosphere, OutputAtmosphere;
    }
}
