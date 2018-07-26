using System;
using System.Linq;
using Assets.Scripts.Atmospherics;

namespace DeviceTest
{
    public static class AtmosphereBuilder
    {
        /// <summary>
        /// Build an atmosphere from a specifier string
        /// </summary>
        /// <param name="AtmosphereSpecifier">(Volume):(Oxygen):(Nitrogen):(CarbonDioxide):(Volatiles):(Pollutant):(Water):(Temperature)</param>
        /// <returns></returns>
        public static Atmosphere BuildAtmosphere(string AtmosphereSpecifier)
        {
            float[] terms = AtmosphereSpecifier.Split(':').Select(
                s => { if(!float.TryParse(s, out float f)) throw new FormatException(); return f; }).ToArray();

            if (terms.Length != 8) throw new FormatException();

            Atmosphere a = new Atmosphere();
            a.Volume = terms[0];
            a.GasMixture = new GasMixture(terms[1], terms[2], terms[3], terms[4], terms[5], terms[6]);
            if (a.TotalMoles > 0) a.GasMixture.AddEnergy(a.GasMixture.HeatCapacity / 100 * terms[7]);
            return a;
        }
    }
}
