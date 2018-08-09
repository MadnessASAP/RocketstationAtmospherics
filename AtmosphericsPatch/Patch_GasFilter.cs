using Harmony;
using Assets.Scripts.Objects.Items;

namespace AtmosphericsPatch
{
    [HarmonyPatch(typeof(GasFilter))]
    [HarmonyPatch("CheckUsedTicks")]
    class Patch_GasFilter_CheckUsedTicks
    {
        static void Postfix(ref int ____usedTicks)
        {
            ____usedTicks = 0;
        }
    }
}
