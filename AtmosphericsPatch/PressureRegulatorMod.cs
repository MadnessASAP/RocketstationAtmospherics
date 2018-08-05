using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Harmony;
using SEModLoader;
using UnityEngine;

namespace AtmosphericsPatch
{

    public class PressureRegulatorMod : IMod
    {
        public static void Init()
        {
            Debug.Log("PressureRegulatorMod: Loaded, version: " + Assembly.GetExecutingAssembly().GetName().Version);
            var harmony = HarmonyInstance.Create("net.a5ap.pressureregulatormod");
            harmony.PatchAll();

            foreach (var method in harmony.GetPatchedMethods())
                Debug.Log("PressureRegulatorMod: Patched: " + method.FullDescription());
            Debug.Log("PressureRegulatorMod: Finished Loading");
        }
    }

    [HarmonyPatch(typeof(Assets.Scripts.Objects.Pipes.PressureRegulator), "OnAtmosphericTick")]
    class Patch_PressureRegulator_OnAtmosphericTick
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Assets.Scripts.Objects.Pipes.Device), "OnAtmosphericTick"));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_PressureRegulator_OnAtmosphericTick), "_newOnAtmosphericTick"));
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

    // This was a really not great way to do things that involved extending the PressureRegulator class and injecting it into GameObjects are they spawned.
    // It failed for many reasons but I'll leave it here as a tombstone that may be useful down the road.
#if false
    [HarmonyPatch(typeof(UnityEngine.Networking.NetworkServer), "Spawn", new Type[] { typeof(GameObject) })]
    public class Patch_NetworkServer_Spawn
    {
        [HarmonyPostfix]
        static void Prefix(GameObject obj)
        {
            if(obj.name == "StructureBackPressureRegulator" || obj.name == "StructurePressureRegulator")
            {
                Debug.Log("Spawn intercept, inserting component!");
                obj.AddComponent<PressureRegulator>();
            }
        }
        
        
        class PressureRegulator : Assets.Scripts.Objects.Pipes.PressureRegulator
        {
            public PressureRegulator()
            {
                this.Slots = new System.Collections.Generic.List<Assets.Scripts.Objects.Slot>();
            }

            public override void Awake()
            {
                Debug.Log("Hello! :-)");
                var oldPR = gameObject.GetComponent<Assets.Scripts.Objects.Pipes.PressureRegulator>();

                // Copy some variables from the old PR
                this.Slots = oldPR.Slots;
                this.BuildStates = oldPR.BuildStates;
                this.BrokenBuildStates = oldPR.BrokenBuildStates;
                this.UsedPower = oldPR.UsedPower;

                var fields = typeof(Assets.Scripts.Objects.Pipes.PressureRegulator).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (var f in fields)
                    f.SetValue(this, f.GetValue(oldPR));

                var props = typeof(Assets.Scripts.Objects.Pipes.PressureRegulator).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (var p in props)
                    if(p.GetGetMethod(true) != null && p.GetSetMethod(true) != null)
                        p.SetValue(this, p.GetValue(oldPR, null), null);

                pr = new RocketstationAtmospherics.PressureRegulator(oldPR.RegulatorType);
                Destroy(oldPR);
                base.Awake();
            }

            public override void OnAtmosphericTick()
            {
                base.OnAtmosphericTick();
                pr.PressureLimit = (float)this.Setting * 1000;
                if(operational)
                    pr.Tick();
            }

            bool operational
            {
                get => OnOff && Powered && Error != 1 &&
                    InputNetwork != null &&
                    OutputNetwork != null;
            }

            RocketstationAtmospherics.PressureRegulator pr;
        }
        
    }  
#endif
}
