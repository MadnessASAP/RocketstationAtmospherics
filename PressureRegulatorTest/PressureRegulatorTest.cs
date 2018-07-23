using System;
using RocketstationAtmospherics;
using Assets.Scripts.Atmospherics;

namespace PressureRegulatorTest
{
    class PressureRegulatorTest
    {
        static void Main(string[] args)
        {
            Atmosphere input = new Atmosphere();
            Atmosphere output = new Atmosphere();

            ProcessArgs(args, input, output);

            PressureRegulator pressureRegulator = new PressureRegulator(inputAtmosphere: input, outputAtmosphere: output);
            pressureRegulator.PressureLimit = 1e6f;

            Console.WriteLine("***** Inital Conditions *****");
            Console.WriteLine(" Input: {0,8:F2} kPa {1,8:F2} K", input.PressureGassesAndLiquids, input.Temperature);
            Console.WriteLine("Output: {0,8:F2} kPa {1,8:F2} K", output.PressureGassesAndLiquids, output.Temperature);
            Console.WriteLine();

            for (int i = 0; i < 100; i++)
            {
                pressureRegulator.Tick();
                Console.WriteLine("***** Cycle " + i + "*****");
                Console.WriteLine(" Input: {0,8:F2} kPa {1,8:F2} K", input.PressureGassesAndLiquids, input.Temperature);
                Console.WriteLine("Output: {0,8:F2} kPa {1,8:F2} K", output.PressureGassesAndLiquids, output.Temperature);
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        private static void ProcessArgs(string[] args, Atmosphere input, Atmosphere output)
        {
            bool processingOutputMixture = false;
            float moles = 0f;
            float outputTemp = 295.15f;
            float inputTemp = 295.15f;
            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    if (!float.TryParse(args[i], out input.Volume)) PrintHelpAndExit();
                    else continue;
                }
                else if (!processingOutputMixture)
                {
                    switch (args[i].ToLower())
                    {
                        case "o2:":
                            if (float.TryParse(args[++i], out moles)) { input.Add(new GasMixture(oxygen: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "n2:":
                            if (float.TryParse(args[++i], out moles)) { input.Add(new GasMixture(nitrogen: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "h2:":
                            if (float.TryParse(args[++i], out moles)) { input.Add(new GasMixture(volatiles: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "x:":
                            if (float.TryParse(args[++i], out moles)) { input.Add(new GasMixture(pollutant: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "co2:":
                            if (float.TryParse(args[++i], out moles)) { input.Add(new GasMixture(carbonDioxide: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "h2o:":
                            if (float.TryParse(args[++i], out moles)) { input.Add(new GasMixture(water: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "temp:":
                            if (!float.TryParse(args[++i], out inputTemp)) PrintHelpAndExit();
                            continue;
                        default:
                            if (float.TryParse(args[i], out output.Volume)) processingOutputMixture = true;
                            else PrintHelpAndExit();
                            continue;
                    }
                }
                else
                {
                    switch (args[i].ToLower())
                    {
                        case "o2:":
                            if (float.TryParse(args[++i], out moles)) { output.Add(new GasMixture(oxygen: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "n2:":
                            if (float.TryParse(args[++i], out moles)) { output.Add(new GasMixture(nitrogen: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "h2:":
                            if (float.TryParse(args[++i], out moles)) { output.Add(new GasMixture(volatiles: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "x:":
                            if (float.TryParse(args[++i], out moles)) { output.Add(new GasMixture(pollutant: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "co2:":
                            if (float.TryParse(args[++i], out moles)) { output.Add(new GasMixture(carbonDioxide: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "h2o:":
                            if (float.TryParse(args[++i], out moles)) { output.Add(new GasMixture(water: moles)); }
                            else PrintHelpAndExit();
                            continue;
                        case "temp:":
                            if (!float.TryParse(args[++i], out outputTemp)) PrintHelpAndExit();
                            continue;
                        default:
                            PrintHelpAndExit();
                            break;
                    }
                }
            }
            if (!processingOutputMixture) PrintHelpAndExit();
            input.GasMixture.AddEnergy(input.GasMixture.HeatCapacity / 100 * inputTemp);
            output.GasMixture.AddEnergy(output.GasMixture.HeatCapacity / 100 * outputTemp);
        }

        private static void PrintHelpAndExit()
        {
            Console.WriteLine("PressureRegulatorTest InputVolume [Gas Mixture Specification] OutputVolume [Gas Mixture Specification]");
            Console.WriteLine("  InputVolume and OutputVolume specifiy the sice of their respective atmospheres in liters.");
            Console.WriteLine("  Gas Mixture Specification if composed of 0 or 1 of the following items");
            Console.WriteLine("    o2: MolesOfOxygen          # of moles of Oxygen to inlude in the mixture");
            Console.WriteLine("    n2: MolesOfNitrogen        # of moles of Nitrogen to inlude in the mixture");
            Console.WriteLine("    h2: MolesOfVolatiles       # of moles of Volatiles to inlude in the mixture");
            Console.WriteLine("    x: MolesOfPollutants       # of moles of Pollutants to inlude in the mixture");
            Console.WriteLine("    co2: MolesOfCarbonDioxide  # of moles of Carbon Dioxide to inlude in the mixture");
            Console.WriteLine("    h20: MolesOfWater          # of moles of Water to inlude in the mixture");
            Console.WriteLine("    temp: TemperatureInKelvin  Temperature of the mixture in Kelvin, if not specified will be 295.15 K");
#if DEBUG
            Console.ReadKey();
#endif
            Environment.Exit(1);
        }
    }
}
