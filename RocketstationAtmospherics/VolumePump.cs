using System;
using Assets.Scripts.Atmospherics;

namespace RocketstationAtmospherics
{
    public class VolumePump : AtmosphericDevice
    {
        public VolumePump(Atmosphere inputAtmosphere = null, Atmosphere outputAtmosphere = null) : base(inputAtmosphere, outputAtmosphere) { }

        public override void Tick()
        {
            if (InputAtmosphere == null || OutputAtmosphere == null) return;
            float p1int = InputAtmosphere.PressureGassesAndLiquidsInPa;
            float p2int;    // To be determined
            float v1int = InputAtmosphere.Volume / 1000.0f;    // Convert from liters to cubic meters
            float v2int = v1int + VolumeSetting / 1000.0f;

            // Intake
            float workExpansion = ThermodynamicHelpers.AdiabaticVolumeChange(p1int, v1int, out p2int, v2int);
            float intakeFraction = (v2int - v1int) / v2int;     // Fraction of the intake atmosphere to remove;

            float intakeMoles = InputAtmosphere.TotalMoles * intakeFraction;                // Moles taken
            float intakeEnergy = InputAtmosphere.GasMixture.TotalEnergy * intakeFraction;   // Energy fraction of the removed gas (not of the work done by expansion)
            float intakeTemperature = InputAtmosphere.Temperature;                          // Temperature of the intake gas
            float intakeHeatCapacity = intakeEnergy / intakeTemperature;                    // Total heat capacit of the intake

            // Equalization - Before the compression the intake gasses need to be combined with the output gasses to establish inital conditions
            float p2com;    // To be determined
            float v2com = OutputAtmosphere.Volume / 1000.0f;
            float v1com = v2com + VolumeSetting / 1000.0f;
            float compressionMoles = intakeMoles + OutputAtmosphere.TotalMoles;
            float compressionHeatCapacity = intakeHeatCapacity + OutputAtmosphere.GasMixture.HeatCapacity;
            float compressionEnergy = intakeEnergy + OutputAtmosphere.GasMixture.TotalEnergy;
            float compressionTemperature = compressionEnergy / compressionHeatCapacity;
            float p1com = ThermodynamicHelpers.GasLawPressure(v1com, compressionMoles, compressionTemperature);

            // Compression
            float workCompression = ThermodynamicHelpers.AdiabaticVolumeChange(p1com, v1com, out p2com, v2com);

            // Only remove the energy from the input side that's actually used for compression
            // this is less then workExpansion if output pressure < input pressure
            workExpansion -= Math.Max(workExpansion + workCompression, 0.0f);

            // Total work, the energy actually consumed by the device
            // Shouldn't be less then 0 but something somehting round error
            // Flip the sign so that positive is energy used.
            float totalWork = -(workCompression + workExpansion);

            // Finally actually shuffle around the moles and energy.
            InputAtmosphere.GasMixture.RemoveEnergy(workExpansion);
            OutputAtmosphere.Add(InputAtmosphere.Remove(intakeMoles));
            OutputAtmosphere.GasMixture.AddEnergy(-workCompression);

            // Saved power used power.
            this.UsedPower = totalWork;
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
