// ReSharper disable NotAccessedPositionalProperty.Local
// ReSharper disable ClassNeverInstantiated.Local

namespace DotQuery.Tests;

[TestClass]
public partial class FromBuilderTests
{
    [TestMethod]
    public async Task Should_return_parameterized_from_statement()
    {
        var sql = DotQuery.From<Order>().Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_from_statement()
    {
        var sql = DotQuery.From<Order>().Build().ToRawString();
        await Verify(sql);
    }

    private record Order(int Id, string Name);
}
