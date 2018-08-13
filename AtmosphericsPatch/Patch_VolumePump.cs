using Harmony;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Networks;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AtmosphericsPatch
{
    [HarmonyPatch(typeof(VolumePump), "OnAtmosphericTick")]
    class Patch_VolumePump_OnAtmosphericTick
    {
        [HarmonyPostfix]
        static void Postfix(VolumePump __instance)
        {
            if(__instance.Powered && __instance.OnOff && __instance)
                __instance.OutputNetwork.Atmosphere.GasMixture.AddEnergy(__instance.UsedPower);
        }
    }

    [HarmonyPatch(typeof(VolumePump), "GetUsedPower")]
    class Patch_VolumePump_GetUsedPower
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_VolumePump_GetUsedPower), "_patchMethod")); // Will return used power
            yield return new CodeInstruction(OpCodes.Ret);  // Which just gets sent on back
        }

        private static float _patchMethod(VolumePump vp, CableNetwork cableNetwork)
        {
            if (vp.PowerCable != null && vp.PowerCable.CableNetwork == cableNetwork)
            {
                float usedPower = 0;
                if (vp.OnOff && vp.OutputSetting > 0 && vp.Error != 1)
                {
                    usedPower = 10;
                    if (vp.InputNetwork != null && vp.OutputNetwork != null)
                    {
                        float p1 = vp.InputNetwork.Atmosphere.PressureGassesAndLiquidsInPa;
                        if (p1 > 0)
                        {
                            float p2 = vp.OutputNetwork.Atmosphere.PressureGassesAndLiquidsInPa;
                            if (p1 < p2)
                                usedPower += RocketstationAtmospherics.ThermodynamicHelpers.AdiabaticPressureChange(
                                    p1,
                                    vp.OutputSetting / 1000,
                                    p2,
                                    out float v2);
                        }
                    }
                    vp.UsedPower = usedPower;
                    return usedPower;
                }
            }
            return -1;
        }
    }
}
