using System.Reflection;
using Harmony;
using UnityEngine;
using SEModLoader;
using Assets.Scripts.Objects.Pipes;

namespace AtmosphericsPatch
{

    public class PressureRegulatorPatch : MonoBehaviour, IMod
    {
        public static void Init()
        {
            var harmony = HarmonyInstance.Create("net.a5ap.pressureregulatormod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(PressureRegulator))]
    [HarmonyPatch("OnAtmosphericTick")]
    public class PressureRegulatorAtmosphericTickPatch
    {
        static bool Prefix(PressureRegulator __instance)
        {
            if (!__instance.Powered || !__instance.OnOff || __instance.Error == 1) return false;
            if (__instance.InputNetwork == null || __instance.OutputNetwork == null) return false;
            var prSim = new RocketstationAtmospherics.PressureRegulator(__instance.RegulatorType, sonicConductance: 4e-4f,
                inputAtmosphere: __instance.InputNetwork.Atmosphere, outputAtmosphere: __instance.OutputNetwork.Atmosphere);
            prSim.PressureLimit = (float)__instance.Setting * 1000;
            prSim.Tick();

            return false;
        }
    }
}
