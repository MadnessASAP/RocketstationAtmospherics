using SEModLoader;
using UnityEngine;
using Harmony;

namespace AtmosphericsPatch
{
    public class AtmosphericsMod : IMod
    {
        public static void Init()
        {
            Debug.Log("PressureRegulatorMod: Loaded, version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            var harmony = HarmonyInstance.Create("net.a5ap.pressureregulatormod");
            harmony.PatchAll();

            foreach (var method in harmony.GetPatchedMethods())
                Debug.Log("PressureRegulatorMod: Patched: " + method.FullDescription());
            Debug.Log("PressureRegulatorMod: Finished Loading");
        }
    }
}
