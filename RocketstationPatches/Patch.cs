using System;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RocketstationPatches
{
    public abstract class Patch
    {
        public abstract void ApplyPatch(string filename);

        protected void LoadTargetAssembly(string filename)
        {
            Debugger.Log(1, "Patch.LoadTargetAssembly", "Loading assembly: \"" + filename + "\"\n");
            tgtAssembly = AssemblyDefinition.ReadAssembly(filename);
            tgtModule = tgtAssembly.MainModule;
            Debugger.Log(2, "Patch.LoadTargetAssembly", "Successfully loaded \"" + tgtModule.Name + "\" from \"" + tgtModule.FileName + "\"\n");
        }

        protected void WriteTargetAssembly()
        {
            Debugger.Log(1, "Patch", "Writing assembly: \"" + tgtModule.FileName + ".patched\"\n");
            tgtModule.Write(tgtModule.FileName + ".patched");
            Debugger.Log(2, "Patch.WriteTargetAssembly", "Wrote assembly: \"" + tgtModule.FileName + ".patched\"\n");
        }

        protected void SetTargetType(string fullname)
        {
            if (tgtModule == null) throw new InvalidOperationException("Must load assembly before retrieving Type!");
            tgtType = tgtModule.GetType(fullname);
            Debugger.Log(2, "Patch.SetTargetType", "Found target: " + tgtType.FullName + "\n");
        }

        protected bool AddField(FieldDefinition field, bool replace = true)
        {
            if (tgtType == null) throw new InvalidOperationException("Must target Type before adding a field!");

            FieldDefinition existingField = tgtType.Fields.Where(tf => tf.Name == field.Name).FirstOrDefault();
            if (existingField != null)
            {
                if (replace)
                {
                    Debugger.Log(2, "Patch.AddField", "Removing Field: " + existingField.FullName + "\n");
                    tgtType.Fields.Remove(existingField);
                }
                else return false;
            }
            Debugger.Log(2, "Patch.AddField", "Adding Field: " + field.FullName + "\n");

            FieldDefinition newField = new FieldDefinition(field.Name, field.Attributes, tgtModule.ImportReference(field.FieldType));
            if (existingField != null && existingField.HasCustomAttributes)
                foreach (CustomAttribute ca in existingField.CustomAttributes)
                    newField.CustomAttributes.Add(ca);
            tgtType.Fields.Add(newField);
            
            
            return true;
        }

        protected bool AddMethod(MethodDefinition method, bool replace = true)
        {
            if (tgtType == null) throw new InvalidOperationException("Must target Type before adding a method!");
            Debugger.Log(2, "Patch.AddMethod", "Adding method: " + method.FullName + "\n");

            MethodDefinition existingMethod = null;
            foreach(var m in tgtType.Methods.Where(x => x.Name == method.Name && x.Parameters.Count == method.Parameters.Count))
            {
                foreach (var param in method.Parameters)
                    if (param.ParameterType.FullName != m.Parameters
                        .Where(x => x.Sequence == param.Sequence)
                        .Select(y => y.ParameterType.FullName)
                        .FirstOrDefault()) goto NoMatch;
                existingMethod = m;
                Debugger.Log(2, "Patch.AddMethod", "Existing method match: " + existingMethod.FullName + "\n");
                break;
                NoMatch:;
            }

            if (existingMethod != null)
                if (replace) tgtType.Methods.Remove(existingMethod);
                else return false;

            MethodDefinition newMethod = new MethodDefinition(method.Name, method.Attributes, tgtModule.ImportReference(method.ReturnType));
            foreach (var p in method.Parameters)
            {
                ParameterDefinition newParameter = new ParameterDefinition(p.Name, p.Attributes, tgtModule.ImportReference(p.ParameterType));
                newMethod.Parameters.Add(newParameter);
            }
            newMethod.Body = new Mono.Cecil.Cil.MethodBody(method);
            foreach (var i in method.Body.Instructions)
                newMethod.Body.Instructions.Add(new Instruction()

            tgtType.Methods.Add(newMethod);
            Debugger.Log(2, "Patch.AddMethod", "Added method: " + newMethod.FullName + "\n");
            return true;

        }

        protected void MergeType(TypeDefinition type)
        {
            if (tgtType == null) throw new InvalidOperationException("Must target Type before merging!");

            foreach (FieldDefinition f in type.Fields)
                AddField(f);

            foreach (MethodDefinition m in type.Methods)
                AddMethod(m);
        }

        AssemblyDefinition tgtAssembly;
        ModuleDefinition tgtModule;
        TypeDefinition tgtType;
    }
}
