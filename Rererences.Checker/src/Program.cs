using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

namespace References.Checker
{
  class Program
  {
    static int Main(string[] args)
    {
      Console.Out.WriteLine("Utility to check we do not have .NET 4.5 references in the code.");
      Console.Out.WriteLine("Usage:");
      Console.Out.WriteLine("  <assembly> output directory");

      if (args.Length != 1)
        return -1;

      var path = args[0];
      Console.Out.WriteLine("Scanning: " + path);

      var allAssemblies = CollectAssemblies(path).ToArray();
      Console.Out.WriteLine("Collected: {0} assemblies", allAssemblies.Count());

      var writer = new TeamCityServiceMessages().CreateWriter();

      using (var suite = writer.OpenTestSuite("AssemblyReferencesTest"))
      {
        foreach (var assembly in allAssemblies)
        {
          using (var test = suite.OpenTest(Regex.Replace(MakeRelativePath(path, assembly), @"[^a-zA-Z0-9/\\]", "_")))
          {
            AssemblyCheckResult r;
            try
            {
              r = new AssemblyChecker().ProcessAssembly(assembly);
            }
            catch (Exception e)
            {
              r = AssemblyCheckResult.Failed(e.ToString());
            }

            if (!r.Success)
            {
              test.WriteFailed(r.FailureText, "");
            }
          }
        }
      }

      return 0;
    }

    private static IEnumerable<string> CollectAssemblies(string path)
    {
      return
        Directory.EnumerateFiles(path, "*.dll").Union(
          Directory.EnumerateFiles(path, "*.exe").Union(
            Directory.EnumerateDirectories(path).SelectMany(CollectAssemblies)
            )
          );
    }


    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static String MakeRelativePath(String fromPath, String toPath)
    {
      if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
      if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

      Uri fromUri = new Uri(fromPath);
      Uri toUri = new Uri(toPath);

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }


  }
}
