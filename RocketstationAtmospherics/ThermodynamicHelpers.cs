using System;
using Assets.Scripts.Atmospherics;

namespace RocketstationAtmospherics
{
    public static class ThermodynamicHelpers
    {
        /// <summary>
        /// Calculates the molar density of a given gas mixture in g/mol.
        /// </summary>
        /// <param name="mix">GasMixture to have its desnity calculated.</param>
        /// <returns>Gas density (g/mol)</returns>
        public static float MolarGasDensity(GasMixture mix)
        {
            return (mix.Oxygen.Quantity / mix.TotalMolesGassesAndLiquids * 31.9988f) +
                (mix.Nitrogen.Quantity / mix.TotalMolesGassesAndLiquids * 28.0134f) +
                (mix.CarbonDioxide.Quantity / mix.TotalMolesGassesAndLiquids * 44.0095f) +
                (mix.Volatiles.Quantity / mix.TotalMolesGassesAndLiquids * 2.01588f) +
                (mix.Pollutant.Quantity / mix.TotalMolesGassesAndLiquids * 70.906f) +
                (mix.Water.Quantity / mix.TotalMolesGassesAndLiquids * 18.0153f);
        }

        /// <summary>
        /// Calculate the pressure of a gas using the ideal gas law
        /// </summary>
        /// <param name="volume">Gas volume (cubic meters)</param>
        /// <param name="moles"># of moles of gas</param>
        /// <param name="temperature">Temperature of gas (Kelvin)</param>
        /// <returns></returns>
        public static float GasLawPressure(float volume, float moles, float temperature)
            => moles * temperature * Constants.GasConstant / volume;

        /// <summary>
        /// Calculate a volume change using an adiabatic process. Returns the energy used.
        /// </summary>
        /// <param name="p1">Inital pressure (Pascals)</param>
        /// <param name="v1">Inital volume (Cubic meters)</param>
        /// <param name="p2">Final calculated pressure (Pascals)</param>
        /// <param name="v2">Final volume (Cubic meters)</param>
        /// <returns>Work Performed (Joules)</returns>
        public static float AdiabaticVolumeChange(float p1, float v1, out float p2, float v2)
        {
            // Calculate the constant K used in this process
            float K = p1 * (float)Math.Pow(v1, Constants.AdiabaticIndex);

            // Calculate Final Pressure
            p2 = K / (float)Math.Pow(v2, Constants.AdiabaticIndex);

            // Calculate work done by the process
            float work = (float)(K / (1 - Constants.AdiabaticIndex) *
                (Math.Pow(v2, 1 - Constants.AdiabaticIndex) - Math.Pow(v1, 1 - Constants.AdiabaticIndex)));

            // Flip the sign so that energy consumed is positive
            return -work;
        }

        /// <summary>
        /// Calculate a pressure change using an adiabatic process. Returns the energy used.
        /// </summary>
        /// <param name="p1">Inital pressure (Pascals)</param>
        /// <param name="v1">Inital volume (Cubic meters)</param>
        /// <param name="p2">Final pressure (Pascals)</param>
        /// <param name="v2">Final calculated volume (Cubic meters)</param>
        /// <returns>Work Performed (Joules)</returns>
        public static float AdiabaticPressureChange(float p1, float v1, float p2, out float v2)
        {
            if (p2 == 0) { v2 = float.PositiveInfinity; return 0; }
            
            // Calculate the constant K used in this process
            float K = p1 * (float)Math.Pow(v1, Constants.AdiabaticIndex);

            // Calculate Final Volume
            v2 = (float)Math.Pow(K / p2, 1/Constants.AdiabaticIndex);

            // Calculate work done by the process
            float work = (float)(K / (1 - Constants.AdiabaticIndex) *
                (Math.Pow(v2, 1 - Constants.AdiabaticIndex) - Math.Pow(v1, 1 - Constants.AdiabaticIndex)));

            // Flip the sign so that energy consumed is positive
            return -work;
        }
    }

    public static class Constants
    {
        public const float AdiabaticIndex = 1.4f;
        public const float GasConstant = 8.3144598f;
    }
}
