using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

public class CS2ExamplePatcher
{

    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Game.dll" };

    // ************************************************************
    // Main entry point called by BepInEx when preload patching
    // ************************************************************

    public static void Patch(AssemblyDefinition assembly)
    {
        Console.WriteLine("Applying CS2 Example Patch");
        ModuleDefinition module = assembly.MainModule;
        var ai = module.Types.First(c => c.Name == "PostFacilityAISystem");
        Console.WriteLine($" publicize AI system structs in {ai}");
        // Advanced example showing hot to get to nested types via reflection
        MakeTypePublic(ai.NestedTypes.First(d => d.Name == "PostFacilityAction"));
        MakeTypePublic(ai.NestedTypes.First(d => d.Name == "PostFacilityTickJob"));
    }

    // Called after the patching process and after scripts are compiled.
    // Used to link references between both assemblies
    // Return true if successful
    public static bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        return true;
    }

    // ************************************************************
    // Helper functions to allow us to access and change
    // variables or properties that are otherwise unavailable.
    // Should illustrate how to get some basics done, for more
    // details, check the relevant BepInEx or Harmony docs.
    // ************************************************************

    private static TypeDefinition MakeTypePublic(TypeDefinition type, bool recursive = false)
    {
        SetTypeToPublic(type);
        foreach (var field in type.Fields)
            SetFieldToPublic(field);
        foreach (var method in type.Methods)
            SetMethodToPublic(method);
        if (recursive == false) return type;
        foreach (var nested in type.NestedTypes)
            MakeTypePublic(nested);
        return type;
    }

    private static void SetTypeToPublic(TypeDefinition type)
    {
        type.IsPublic = true;
        type.IsNotPublic = false;
    }

    private static void SetFieldToPublic(FieldDefinition field)
    {
        field.IsFamily = false;
        field.IsPrivate = false;
        field.IsPublic = true;
    }

    private static void SetMethodToPublic(MethodDefinition field, bool force = false)
    {
        // Leave protected virtual methods alone to avoid
        // issues with others inheriting from it, as it gives
        // a compile error when protection level mismatches.
        // Unsure if this changes anything on runtime though?
        if (!field.IsFamily || !field.IsVirtual || force) {
            field.IsFamily = false;
            field.IsPrivate = false;
            field.IsPublic = true;
        }
    }

    private static void SetMethodToVirtual(MethodDefinition method)
    {
        method.IsVirtual = true;
    }

    // ************************************************************
    // ************************************************************

}