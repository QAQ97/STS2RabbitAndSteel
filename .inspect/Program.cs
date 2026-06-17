using System;
using System.IO;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        var asmRoot = "H:/steam/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64";
        if (!Directory.Exists(asmRoot))
        {
            Console.Error.WriteLine("Directory not found: " + asmRoot);
            return;
        }

        var dllPaths = Directory.GetFiles(asmRoot, "*.dll");
        var resolver = new PathAssemblyResolver(dllPaths);
        using var mlc = new MetadataLoadContext(resolver);
        var asmPath = Path.Combine(asmRoot, "sts2.dll");
        var asm = mlc.LoadFromAssemblyPath(asmPath);
        var powerCmd = asm.GetType("MegaCrit.Sts2.Core.Commands.PowerCmd");
        if (powerCmd == null)
        {
            Console.Error.WriteLine("PowerCmd type not found");
            return;
        }

        foreach (var method in powerCmd.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic))
        {
            if (method.Name == "Apply")
            {
                Console.WriteLine(method);
                Console.WriteLine("  Params: " + string.Join(", ", method.GetParameters().Select(p => p.ParameterType.FullName + " " + p.Name)));
                Console.WriteLine("  IsGenericMethodDefinition: " + method.IsGenericMethodDefinition);
                Console.WriteLine("  ReturnType: " + method.ReturnType.FullName);
                Console.WriteLine();
            }
        }
    }
}
