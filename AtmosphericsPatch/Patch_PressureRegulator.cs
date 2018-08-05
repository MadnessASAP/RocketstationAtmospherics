using System.Reflection.Emit;
using System.Collections.Generic;
using Harmony;

namespace AtmosphericsPatch
{
    [HarmonyPatch(typeof(Assets.Scripts.Objects.Pipes.PressureRegulator), "OnAtmosphericTick")]
    class Patch_PressureRegulator_OnAtmosphericTick
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Assets.Scripts.Objects.Pipes.Device), "OnAtmosphericTick"));       // call base.OnAtmosphericTick
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_PressureRegulator_OnAtmosphericTick), "_newOnAtmosphericTick")); // call our OnAtmosphericTick method
            yield return new CodeInstruction(OpCodes.Ret);
        }

        private static void _newOnAtmosphericTick(Assets.Scripts.Objects.Pipes.PressureRegulator device)
        {
            if (!device.OnOff || !device.Powered || device.Error == 1 ||
                device.InputNetwork == null || device.OutputNetwork == null) return;
            var sim = new RocketstationAtmospherics.PressureRegulator(device.RegulatorType,
                    inputAtmosphere: device.InputNetwork.Atmosphere,
                    outputAtmosphere: device.OutputNetwork.Atmosphere)
            {
                PressureLimit = (float)device.Setting * 1000
            };
            sim.Tick();
        }
    }
}
