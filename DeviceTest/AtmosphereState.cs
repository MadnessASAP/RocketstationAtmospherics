using System;
using System.Diagnostics;
using Assets.Scripts.Atmospherics;

namespace DeviceTest
{
    public class AtmosphereState
    {
        public AtmosphereState(Atmosphere atmosphere)
        {
            this.Pressure = atmosphere.PressureGassesAndLiquidsInPa;
            this.Temperature = atmosphere.Temperature;
            this.MolesCount = atmosphere.TotalMoles;
            this.Energy = atmosphere.GasMixture.TotalEnergy;
        }

        private AtmosphereState() { }

        public float Pressure { get; private set; }
        public float Temperature { get; private set; }
        public float MolesCount { get; private set; }
        public float Energy { get; private set; }

        public string PressureString { get => FormatEngineering(Pressure, "Pa"); }
        public string TemperatureString { get => FormatEngineering(Temperature, "K"); }
        public string MolesString { get => FormatEngineering(MolesCount, "mol"); }
        public string EnergyString { get => FormatEngineering(Energy, "J"); }

        public int StringLength { get; set; } = 11;

        private string FormatEngineering(float num, string units)
        {
            string siPrefix = " ";
            string[] siPrefixes = { "y", "p", "n", "u", "m", " ", "k", "M", "G", "T", "P" };
            if (num != 0)
            {
                int prefixIdx = (int)Math.Floor((Math.Log10(Math.Abs(num)) - 1) / 3) + 5;
                siPrefix = siPrefixes[prefixIdx];
                num /= (float)Math.Pow(10, (prefixIdx - 5) * 3);
            }

            string postfix = " " + siPrefix + units;

            return String.Format("{0," + (StringLength - postfix.Length) + ":F" + (StringLength - (5 + postfix.Length)) + "}{1}", num, postfix);
        }
    }
}
