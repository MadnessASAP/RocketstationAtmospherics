using System;
using Assets.Scripts.Atmospherics;

namespace RocketstationAtmospherics
{
    public class VolumePump : AtmosphericDevice
    {
        public VolumePump(Atmosphere inputAtmosphere = null, Atmosphere outputAtmosphere = null) : base(inputAtmosphere, outputAtmosphere) { }


        /// <summary>
        /// Cycle the volume pump moving VolumeSetting liters from the input to the output
        /// </summary>
        public override void Tick()
        {
            // Bail now if there's nothing to do.
            if (InputAtmosphere == null || OutputAtmosphere == null || InputAtmosphere.PressureGassesAndLiquidsInPa < 1 || VolumeSetting == 0) return;

            // Pull the mixture from the input side.
            GasMixture mix = InputAtmosphere.Remove(InputAtmosphere.TotalMoles * VolumeSetting / (VolumeSetting + InputAtmosphere.Volume)) ?? new GasMixture();

            float work = 0;     // Work done in Joules
            float p1 = InputAtmosphere.PressureGassesAndLiquidsInPa;    // Initial Pressure
            float p2 = OutputAtmosphere.PressureGassesAndLiquidsInPa;   // Final Pressure

            // If the output has a lower pressure then the input no work needs to be done and the mixture can simply be moved.
            if (p1 < p2)
            {
                float v1 = VolumeSetting;   // Inital volume
                float v2;                   // Final volume. Not used but potentially could be if a second stage of compression INTO the Output side gets implemented.

                // Compression consumes energy, calculate that here
                work = ThermodynamicHelpers.AdiabaticPressureChange(p1, v1, p2, out v2);
            }

            // Add the consumed energy to the mixture as heat
            mix.AddEnergy(work);

            // And feed that mixture into the output
            OutputAtmosphere.Add(mix);

            this.UsedPower = work;
        }
        
        /// <summary>
        /// How man literes per second to flow;
        /// </summary>
        public float VolumeSetting {
            get => volumeSetting;
            set => volumeSetting = Math.Max(0.0f, Math.Min(value, 100.0f));
        }
        private float volumeSetting;

        public float UsedPower { get; protected set; }
    }
}
