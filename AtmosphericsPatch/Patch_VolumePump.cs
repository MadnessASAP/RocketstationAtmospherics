using Harmony;
using Assets.Scripts.Objects.Pipes;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AtmosphericsPatch
{
    [HarmonyPatch(typeof(VolumePump), "OnAtmosphericTick")]
    class Patch_VolumePump_OnAtmosphericTick
    {
        [HarmonyTranspiler]
        IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Assets.Scripts.Objects.Pipes.Device), "OnAtmosphericTick"));       // call base.OnAtmosphericTick
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_VolumePump_OnAtmosphericTick), "_patchMethod")); // call our OnAtmosphericTick method
            yield return new CodeInstruction(OpCodes.Ret);
        }

        private static void _patchMethod(VolumePump vp)
        {
            if(vp.OnOff && vp.Powered && vp.Error != 1 && vp.InputNetwork != null && vp.OutputNetwork != null)
            {
                var sim = new RocketstationAtmospherics.VolumePump(vp.InputNetwork.Atmosphere, vp.OutputNetwork.Atmosphere)
                {
                    VolumeSetting = vp.OutputSetting,
                };

                sim.Tick();

                vp.UsedPower = sim.UsedPower;
            }
        }
    }

    [HarmonyPatch(typeof(VolumePump), "GetUsedPower")]
    class Patch_VolumePump_GetUsedPower
    {
        [HarmonyTranspiler]
        IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_VolumePump_GetUsedPower), "_patchMethod")); // Will return used power
            yield return new CodeInstruction(OpCodes.Ret);  // Which just gets sent on back
        }

        private static float _patchMethod(VolumePump vp)
        {
            return vp.UsedPower;
        }
    }
}
