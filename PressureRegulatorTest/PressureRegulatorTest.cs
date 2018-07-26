using System;
using System.Linq;
using RocketstationAtmospherics;
using DeviceTest;
using Assets.Scripts.Atmospherics;

namespace PressureRegulatorTest
{
    class PressureRegulatorTest
    {
        static void Main(string[] args)
        {
            if (args.Length != 2) PrintHelpAndExit();

            PressureRegulator pressureRegulator = new PressureRegulator()
            {
                PressureLimit = 1e6f
            };
            DeviceTest.Test deviceTest;
            try { deviceTest = new DeviceTest.Test(pressureRegulator, args[0], args[1]); }
            catch (FormatException e) { Console.WriteLine(e); PrintHelpAndExit(); return; }

            Console.WriteLine("|Stp|                 Input                |                Output                |");
            Console.WriteLine("| # | Temperature|  Pressure  |    Mole    | Temperature|  Pressure  |    Mole    |");
            Console.WriteLine("+===+============+============+============+============+============+============+");
            Console.WriteLine("|{0,3}| {1}| {2}| {3}| {4}| {5}| {6}|", 0,
                deviceTest.inputHistory[0].TemperatureString,
                deviceTest.inputHistory[0].PressureString,
                deviceTest.inputHistory[0].MolesString,
                deviceTest.outputHistory[0].TemperatureString,
                deviceTest.outputHistory[0].PressureString,
                deviceTest.outputHistory[0].MolesString);

            for (int i = 1; i < 10; i++)
            {
                deviceTest.Tick();
                Console.WriteLine("|{0,3}| {1}| {2}| {3}| {4}| {5}| {6}|", i,
                    deviceTest.inputHistory[i].TemperatureString,
                    deviceTest.inputHistory[i].PressureString,
                    deviceTest.inputHistory[i].MolesString,
                    deviceTest.outputHistory[i].TemperatureString,
                    deviceTest.outputHistory[i].PressureString,
                    deviceTest.outputHistory[i].MolesString);
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        private static void PrintHelpAndExit()
        {
            Console.WriteLine("PressureRegulatorTest InputAtmosphereSpecification OutputAtmosphereSpecification");
            Console.WriteLine("  InputAtmosphereSpecification and OutputAtmosphereSpecification specifice the respective input and output atmospheres.");
            Console.WriteLine("  they are 8 floating point numbers seperated by ':' of the format:");
            Console.WriteLine("    (Volume):(Oxygen):(Nitrogen):(CarbonDioxide):(Volatiles):(Pollutant):(Water):(Temperature)");
            Console.WriteLine("    Volume is specified in Liters, gas components in moles and temperature in Kelvins.");
            Console.WriteLine("    ");
            Console.WriteLine("    Example:");
            Console.WriteLine("    PressureRegulatorTest 1000:1000:3000:0:0:0:0:295.15 500:0:0:0:0:0:0:0");
            Console.WriteLine("    Input atmosphere = 1000 L of 25%/75% mix of Oxygen/Nitrogen @ 295.15 K");
            Console.WriteLine("    Output atmosphere = 500 L empty atmosphere");
#if DEBUG
            Console.ReadKey();
#endif
            Environment.Exit(1);
        }
    }
}
