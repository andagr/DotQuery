// ReSharper disable NotAccessedPositionalProperty.Local
// ReSharper disable ClassNeverInstantiated.Local
namespace DotQuery.Tests;

[TestClass]
public partial class WhereBuilderTests
{
    [TestMethod]
    public async Task Should_return_parameterized_where_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV").Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV").Build().ToRawString();
        await Verify(sql);
    }

    private record Order(int Id, string Name);
}