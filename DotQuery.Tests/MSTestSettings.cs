[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
[assembly: UsesVerify]

namespace DotQuery.Tests;

[TestClass]
public partial class TestAssemblyInitializer
{
    [TestMethod]
    [Ignore]
    public void Init() {}

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        UseProjectRelativeDirectory("Snapshots");
    }
}