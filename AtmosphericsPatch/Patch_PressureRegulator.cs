using System;
using System.Reflection;
using UnityEngine;
using Harmony;

namespace AtmosphericsPatch
{

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
}
