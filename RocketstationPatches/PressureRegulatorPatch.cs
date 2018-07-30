using System;
using System.Linq;
using Mono.Cecil;
using RocketstationAtmospherics;
using Assets.Scripts.Objects.Pipes;

namespace RocketstationPatches
{
    public class PressureRegulatorPatch : Patch
    {
        public override void ApplyPatch(string filename)
        {
            LoadTargetAssembly(filename);

            using (AssemblyDefinition srcAssembly = AssemblyDefinition.ReadAssembly(this.GetType().Assembly.Location))
            {
                TypeDefinition srcType = srcAssembly.MainModule.GetType(this.GetType().FullName).NestedTypes.Where(nt => nt.Name == typeof(PressureRegulator).Name).First();
                SetTargetType(typeof(Assets.Scripts.Objects.Pipes.PressureRegulator).FullName);
                MergeType(srcType);
                WriteTargetAssembly();
            }
        }

        private class PressureRegulator : SettableAtmosDevice
        {
            public override void OnAtmosphericTick()
            {
                base.OnAtmosphericTick();
                if (this.OnOff && this.Powered && this.Error != 1)
                {
                    if (this.InputNetwork != null && this.OutputNetwork != null)
                    {
                        simPR.PressureLimit = (float)Setting * 1000;
                        simPR.Tick();
                    }
                }
            }

            public override void CheckConnections()
            {
                base.CheckConnections();

                simPR = new RocketstationAtmospherics.PressureRegulator(RegulatorType, inputAtmosphere: InputNetwork.Atmosphere, outputAtmosphere: OutputNetwork.Atmosphere);
            }

            public RegulatorType RegulatorType;

            private RocketstationAtmospherics.PressureRegulator simPR;
        }
    }
}
