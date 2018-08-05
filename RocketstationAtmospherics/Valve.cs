using System;
using Assets.Scripts.Atmospherics;

namespace RocketstationAtmospherics
{
    public class Valve : AtmosphericDevice
    {
        public Valve(Atmosphere inputAtmosphere = null, Atmosphere outputAtmosphere = null, float sonicConductance = 4e-4f) : base(inputAtmosphere, outputAtmosphere)
        {
            SonicConductance = sonicConductance;
        }

        /// <summary>
        /// Simulates the flow from the InputAtmosphere to the OutputAtmosphere (or reverse if the valve is bidirectional) over Timestep seconds.
        /// </summary>
        public override void Tick()
        {
            if (InputAtmosphere == null || OutputAtmosphere == null) return;

            // Get the input/output pressures and check if the flow is going to be reversed.
            float pressureIn = InputAtmosphere.PressureGassesAndLiquidsInPa;
            float pressureOut = OutputAtmosphere.PressureGassesAndLiquidsInPa;
            bool reverseFlow = pressureOut > pressureIn;
            if (Bidirectional && reverseFlow) { float t = pressureIn; pressureIn = pressureOut; pressureOut = t; }
            else if (!Bidirectional && reverseFlow) return; // No flow in reverse

            // Get the gas density of the input atmosphere, will be needed to calculate molar flow
            float gasDensity = ThermodynamicHelpers.MolarGasDensity(
                (reverseFlow ? OutputAtmosphere : InputAtmosphere).GasMixture);

            // Iterate the flow calculations based on how accurate the simulation should be.
            for (int i = 0; i < Fidelity; i++)
            {
                if (reverseFlow)
                {
                    pressureIn = OutputAtmosphere.PressureGassesAndLiquidsInPa;
                    pressureOut = InputAtmosphere.PressureGassesAndLiquidsInPa;
                }
                else
                {
                    pressureIn = InputAtmosphere.PressureGassesAndLiquidsInPa;
                    pressureOut = OutputAtmosphere.PressureGassesAndLiquidsInPa;
                }

                float massFlow = CalculateFlowRate(pressureIn, pressureOut);
                float moleFLow = massFlow / gasDensity;
                float molesToMove = moleFLow * Timestep / Fidelity;
                if (reverseFlow)
                    InputAtmosphere.Add(OutputAtmosphere.Remove(molesToMove));
                else OutputAtmosphere.Add(InputAtmosphere.Remove(molesToMove));
            }
        }

        /// <summary>
        /// Calculates the flow rate through the valve in g / s
        /// </summary>
        /// <param name="inletPressure">Inlet pressure in Pascals</param>
        /// <param name="outletPressure">Outlet pressure in Pascals</param>
        /// <param name="criticalPressureRatio">Pressure ratio (Pout/Pin) where the flow becomes choked.  Probably shouldn't touch this!</param>
        /// <remarks>
        /// The critical pressure ratio is dependent on the Adiabatic Index of the fluid. 
        /// Since 1.4 is a safe assumption for the Adiabatic Index, 0.5283 is a safe assumption for the 
        /// Critical Pressure Ratio
        /// </remarks>
        /// <returns>Mass flow rate in g / s</returns>
        float CalculateFlowRate(float inletPressure, float outletPressure, float criticalPressureRatio = 0.5283f)
        {
            float pressureRatio = outletPressure / inletPressure;

            if (pressureRatio > criticalPressureRatio)     // Subsonic Flow
                return Open * inletPressure * SonicConductance * (float)Math.Sqrt(
                    Math.Max(1.0 - Math.Pow(    // Ensure this number does not go negative
                        (pressureRatio - criticalPressureRatio) /
                        (1 - criticalPressureRatio), 2.0), 0.0));
            else return Open * inletPressure * SonicConductance;       // Choked Flow
        }

        /// <summary>
        /// The maximum flow rate through the valve in g / Pa * s
        /// </summary>
        /// <remarks>
        /// This flow rate is achieved when the pressure ratio (Pout / Pin) across the valve is greater then the
        /// gasses critical pressure ratio and the flow threfore becomes choked (sonic).
        /// </remarks>
        public virtual float SonicConductance { get; protected set; }

        /// <summary>
        /// How open is the valve, range: 0...1 (closed -> open)
        /// </summary>
        public virtual float Open
        {
            get => open;
            set => open = Math.Max(0.0f, Math.Min(value, 1.0f));
        }
        private float open = 0.0f;

        /// <summary>
        /// Does this valve allow flow in both directions
        /// </summary>
        public virtual bool Bidirectional { get; set; } = true;

        /// <summary>
        /// How long is each tick in the simulation
        /// </summary>
        public float Timestep { get; set; } = 0.5f;

        /// <summary>
        /// Improve the accuracy of the simulation by subdividing the tick into this many steps
        /// </summary>
        public int Fidelity
        {
            get => fidelity;
            set => fidelity = Math.Max(value, 1);
        }
        private int fidelity = 10;
    }
}
