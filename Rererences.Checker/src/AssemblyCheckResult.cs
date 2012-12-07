using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;

namespace References.Checker
{
  public class AssemblyCheckResult
  {
    public bool Success { get; private set; }
    public string FailureText { get; private set; }

    [NotNull]
    public static AssemblyCheckResult Succeeded()
    {
      return new AssemblyCheckResult
               {
                 FailureText = null,
                 Success = true
               };
    }

    [NotNull]
    public static AssemblyCheckResult Failed(string message)
    {
      return new AssemblyCheckResult
               {
                 FailureText = message,
                 Success = false
               };
    }


    public static AssemblyCheckResult Merge(params AssemblyCheckResult[] results)
    {
      return Merge((IEnumerable<AssemblyCheckResult>) results);
    }

    public static AssemblyCheckResult Merge(params IEnumerable<AssemblyCheckResult>[] results)
    {
      return Merge(results.SelectMany(x=>x));
    }

    public static AssemblyCheckResult Merge(IEnumerable<AssemblyCheckResult> results)
    {      
      var errors = results.Where(r => !r.Success).Select(r => r.FailureText).ToArray();

      if (errors.Any())
      {
        return Failed(string.Join(", ", errors));
      }
      return Succeeded();
    }

  }
}