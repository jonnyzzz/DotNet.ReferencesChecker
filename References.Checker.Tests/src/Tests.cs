using System;
using NUnit.Framework;
using Test.FourDotFive;
using Test.FourDotZero;
using Test.ThreeDotFive;
using Test.TwoDotZero;

namespace References.Checker.Tests
{

  [TestFixture]
  public class Tests
  {
    [Test]
    public void TestOnSelf()
    {
      this.ExtensionMethodToMock();

      Assert.That(DoTest<Tests>().Success, Is.False);
    }

    [Test]
    public void Test_2_0()
    {
      Assert.That(DoTest<ClassTwoDotZero>().Success, Is.True);
    }

    [Test]
    public void Test_3_5()
    {
      Assert.That(DoTest<ClassThreeDotFive>().Success, Is.True);
    }

    [Test]
    public void Test_4_0()
    {
      Assert.That(DoTest<ClassFourDotZero>().Success, Is.True);
    }

    [Test]
    public void Test_4_5()
    {
      var r = DoTest<ClassFourDotFive>();
      Assert.That(r.Success, Is.False);

      Assert.That(r.FailureText, Contains.Substring("[Extension] to mscorlib"));
      Assert.That(r.FailureText, Contains.Substring("[TargetFramework] contains 4.5"));
    }

    private AssemblyCheckResult DoTest<T>()
    {
      var r = new AssemblyChecker().ProcessAssembly(new Uri(typeof (T).Assembly.Location).LocalPath);
      Assert.That(r, Is.Not.Null);
      Console.Out.WriteLine(r.Success ? "Success" : r.FailureText);
      return r;
    }
  }
}
