using System;
using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects.Pipes;

namespace RocketstationAtmospherics
{
    public class PressureRegulator : Valve
    {
        public PressureRegulator(RegulatorType regulatorType = RegulatorType.Upstream, float sonicConductance = 8e-4f,
            float PressureLimit = 50000, float throttleRange = 50000,
            Atmosphere inputAtmosphere = null, Atmosphere outputAtmosphere = null, float timestep = 0.5f, int fidelity = 10)
        {
            SonicConductance = sonicConductance;
            PressureLimit = pressureLimit;
            ThrottleRange = throttleRange;
            InputAtmosphere = inputAtmosphere;
            OutputAtmosphere = outputAtmosphere;
            Timestep = timestep;
            Fidelity = fidelity;
            RegulatorType = regulatorType;
            Open = 1.0f;
        }

        /// <summary>
        /// Pressure Regulators are always unidirectional.
        /// </summary>
        public override bool Bidirectional { get => false; }

        /// <summary>
        /// How "open" the pressure regulator is, decreases as it approaches the pressure limit.  THis can also be set to a max open setting.
        /// </summary>
        public override float Open
        {
            get
            {
                switch (RegulatorType)
                {
                    case RegulatorType.Upstream:
                        if (OutputAtmosphere == null) return open;
                        return open * Math.Max(0.0f, Math.Min((PressureLimit - OutputAtmosphere.PressureGassesAndLiquidsInPa) / ThrottleRange, 1.0f));
                    case RegulatorType.Downstream:
                        if (InputAtmosphere == null) return open;
                        return open * Math.Max(0.0f, Math.Min((InputAtmosphere.PressureGassesAndLiquidsInPa - PressureLimit) / ThrottleRange, 1.0f));
                    default: return open;
                }
            }
            set => open = Math.Max(0.0f, Math.Min(value, 1.0f));
        }
        private float open = 1.0f;

        /// <summary>
        /// Pressure to regulate to.
        /// </summary>
        public float PressureLimit
        {
            get => pressureLimit;
            set => pressureLimit = Math.Max(value, 0.0f);
        }
        float pressureLimit = 50000;

        /// <summary>
        /// How close to get to the target pressure before beginning to throttle the flow (Pascals)
        /// </summary>
        public float ThrottleRange
        {
            get => throttleRange;
            set => throttleRange = Math.Max(value, 0.0f);
        }
        float throttleRange = 50000;

        public RegulatorType RegulatorType { get; set; }
    }
}
