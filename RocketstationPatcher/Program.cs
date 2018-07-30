using System;
using ILRepacking;
using RocketstationPatches;

namespace RocketstationPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            PressureRegulatorPatch patchPR = new PressureRegulatorPatch();

            patchPR.ApplyPatch(@"C:\SteamLibrary\steamapps\common\Stationeers\rocketstation_Data\Managed\Assembly-CSharp.dll");

            Console.WriteLine("Assembly Patched Successfully!");
            Console.Read();
        }
    }
}
