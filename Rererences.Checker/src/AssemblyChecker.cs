using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace References.Checker
{
  public class AssemblyChecker
  {

    private IEnumerable<AssemblyCheckResult> CheckExtensionMethod(AssemblyDefinition d)
    {
      return from mod in d.Modules 
             from rf in mod.GetTypeReferences() 
             where rf.FullName == "System.Runtime.CompilerServices.ExtensionAttribute" 
             where rf.Scope.Name != "System.Core" 
             select AssemblyCheckResult.Failed("There is a reference for [Extension] to mscorlib");
    }

    private IEnumerable<AssemblyCheckResult> CheckTargetFrameworkAttribute(AssemblyDefinition d)
    {
      return from at in d.CustomAttributes 
             where at.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute" && at.HasConstructorArguments
             where at.ConstructorArguments.Any(arg => arg.Value.ToString().Contains("4.5")) 
             select AssemblyCheckResult.Failed("[TargetFramework] contains 4.5");
    }

    [NotNull]
    public AssemblyCheckResult ProcessAssembly(string path)
    {
      AssemblyDefinition d = AssemblyDefinition.ReadAssembly(path);
      
      return AssemblyCheckResult.Merge(
        CheckExtensionMethod(d),
        CheckTargetFrameworkAttribute(d)
        );
    }
  }
}


